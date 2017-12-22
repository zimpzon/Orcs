using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System;
using System.Collections.Generic;

public class Server : MonoBehaviour
{
    public static Server Instance;

    public string TitleId = "5388"; // 272 - W..T..H..!!!
    public object LastResult; // Simple and effective.

    // Debug switches --->
    [NonSerialized]
    public bool SimulateConnectionLoss = false;
    const PlayFabErrorCode FakeErrorCode = PlayFabErrorCode.InternalServerError;

    [NonSerialized]
    public float AdditionalGlobalLatency = 0.0f;

    const bool OfflineMode = false; // When working without an internet connection
    // <--- Debug switches

    void Awake()
    {
        Instance = this;
        PlayFabSettings.TitleId = TitleId;
    }

    void DoCustomLogin(Action<LoginResult> onsuccess, Action<PlayFabError> onError)
    {
        LoginWithCustomIDRequest request = new LoginWithCustomIDRequest();
        request.TitleId = TitleId;
        request.CustomId = SaveGame.Members.UserId;
        request.CreateAccount = true;

        PlayFabClientAPI.LoginWithCustomID(request, onsuccess, onError);
    }

    void DoAndroidLogin(Action<LoginResult> onsuccess, Action<PlayFabError> onError)
    {
        LoginWithAndroidDeviceIDRequest request = new LoginWithAndroidDeviceIDRequest();
        request.TitleId = TitleId;
        request.AndroidDeviceId = SystemInfo.deviceUniqueIdentifier;
        request.OS = SystemInfo.operatingSystem;
        request.AndroidDevice = SystemInfo.deviceModel;
        request.CreateAccount = true;

        PlayFabClientAPI.LoginWithAndroidDeviceID(request, onsuccess, onError);
    }

    //void DoKongregateLogin(Action<LoginResult> onsuccess, Action<PlayFabError> onError)
    //{
    //    LoginWithKongregateRequest request = new LoginWithKongregateRequest();
    //    request.TitleId = TitleId;
    //    request.
    //    request.AndroidDeviceId = SystemInfo.deviceUniqueIdentifier;
    //    request.OS = SystemInfo.operatingSystem;
    //    request.AndroidDevice = SystemInfo.deviceModel;
    //    request.CreateAccount = true;

    //    PlayFabClientAPI.LoginWithAndroidDeviceID(request, onsuccess, onError);
    //}

    public IEnumerator DoServerColdStart()
    {
        yield return DoLoginCo();
        yield return GetAllStats();
    }

    Dictionary<string, uint?> statVersions = new Dictionary<string, uint?>();

    public IEnumerator GetAllStats()
    {
        GetPlayerStatisticsRequest req = new GetPlayerStatisticsRequest
        {
        };

        Action<Action<GetPlayerStatisticsResult>, Action<PlayFabError>> apiCall = (onsuccess, onError) =>
        {
            PlayFabClientAPI.GetPlayerStatistics(req, onsuccess, onError);
        };

        yield return ExecuteApiCallWithRetry(apiCall, busyIndicatorAfterSec: 0.0f, messageBoxAfterSec: 4.0f);
        var result = (GetPlayerStatisticsResult)LastResult;
        if (result != null)
        {
            foreach (var stat in result.Statistics)
            {
                statVersions[stat.StatisticName] = stat.Version;
            }
        }
    }

    public IEnumerator UpdateStat(string name, int value)
    {
        // Don't spam if client is not logged in (offline). Some score might also be posted before login completes and the SDK throws then.
        if (!PlayFabClientAPI.IsClientLoggedIn())
            yield break;

        UpdatePlayerStatisticsRequest req = new UpdatePlayerStatisticsRequest();
        StatisticUpdate stat = new StatisticUpdate
        {
            Version = statVersions.ContainsKey(name) ? statVersions[name] : null,
            StatisticName = name,
            Value = value,
        };
        req.Statistics = new List<StatisticUpdate> { stat };

        Action<Action<UpdatePlayerStatisticsResult>, Action<PlayFabError>> apiCall = (onsuccess, onError) =>
        {
            PlayFabClientAPI.UpdatePlayerStatistics(req, onsuccess, onError);
        };

        yield return ExecuteApiCallWithRetry(apiCall, busyIndicatorAfterSec: 0.0f, messageBoxAfterSec: 4.0f);
    }

    public IEnumerator GetAllPlayerData()
    {
        GetPlayerCombinedInfoRequest req = new GetPlayerCombinedInfoRequest();
        req.InfoRequestParameters = new GetPlayerCombinedInfoRequestParams();
        req.InfoRequestParameters.GetPlayerProfile = true;
        req.InfoRequestParameters.GetPlayerStatistics = true;
        req.InfoRequestParameters.GetTitleData = true;
        req.InfoRequestParameters.GetUserData = true;
        req.InfoRequestParameters.GetUserInventory = true;
        req.InfoRequestParameters.GetUserReadOnlyData = true;
        req.InfoRequestParameters.GetUserVirtualCurrency = true;

        Action<Action<GetPlayerCombinedInfoResult>, Action<PlayFabError>> apiCall = (onsuccess, onError) =>
        {
            PlayFabClientAPI.GetPlayerCombinedInfo(req, onsuccess, onError);
        };

        yield return ExecuteApiCallWithRetry(apiCall, busyIndicatorAfterSec: 0.0f, messageBoxAfterSec: 4.0f);
    }

    public IEnumerator GetRollData()
    {
        ExecuteCloudScriptRequest req = new ExecuteCloudScriptRequest();
        req.FunctionName = "helloWorld";
        req.FunctionParameter = "Whoop";

        Action<Action<ExecuteCloudScriptResult>, Action<PlayFabError>> apiCall = (onSuccess, onError) =>
        {
            PlayFabClientAPI.ExecuteCloudScript(req, onSuccess, onError);
        };

        yield return ExecuteApiCallWithRetry(apiCall, busyIndicatorAfterSec: 3.0f, messageBoxAfterSec: 5.0f);
    }

    public IEnumerator HelloWorld()
    {
        ExecuteCloudScriptRequest req = new ExecuteCloudScriptRequest();
        req.FunctionName = "helloWorld";
        yield break;
    }

    public IEnumerator DoLoginCo()
    {
        Action<Action<LoginResult>, Action<PlayFabError>> apiCall;

        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                apiCall = DoAndroidLogin;
                break;

            case RuntimePlatform.WebGLPlayer:
                apiCall = DoCustomLogin;
                break;

            default:
                apiCall = DoCustomLogin;
                break;
        }

        yield return ExecuteApiCallWithRetry(apiCall, busyIndicatorAfterSec: 0.0f, messageBoxAfterSec: 4.0f);
    }

    IEnumerator ExecuteApiCallWithRetry<TResult>(
        Action<Action<TResult>, Action<PlayFabError>> apiAction,
        float busyIndicatorAfterSec = 1.0f,
        float messageBoxAfterSec = 4.0f,
        float fakeApiLatency = 0.0f,
        int fakeFailureCount = 0)
    {
        LastResult = null;

        //if (busyIndicatorAfterSec <= 0)
        //    BusyScript.Instance.Show();

        float startTime = Time.time;
        float timeWaited = 0;
        int attempts = 0;
        TResult result = default(TResult);
        int fakeFailuresLeft = fakeFailureCount;

        while (true)
        {
            attempts++;
            if (attempts > 5)
                break;

            bool callComplete = false;
            bool callSuccess = false;
            float apiCallRetryTime = 2.0f;

            Action<TResult> onSuccess = callResult =>
            {
                Debug.Log(callResult);
                result = callResult;
                callComplete = true;
                callSuccess = true;
            };

            Action<PlayFabError> onError = error =>
            {
                string fullMsg = error.ErrorMessage;
                if (error.ErrorDetails != null)
                    foreach (var pair in error.ErrorDetails)
                        foreach (var eachMsg in pair.Value)
                            fullMsg += "\n" + pair.Key + ": " + eachMsg;

                Debug.Log(fullMsg);
                callComplete = true;
            };

            float fakeLatency = fakeApiLatency + AdditionalGlobalLatency;
            if (fakeLatency > 0.0f)
                yield return new WaitForSeconds(fakeLatency);

            if (fakeFailuresLeft > 0 || SimulateConnectionLoss)
            {
                fakeFailuresLeft--;
                PlayFabError fakeError = new PlayFabError();
                fakeError.Error = FakeErrorCode;
                fakeError.ErrorMessage = "Fake error for testing";
                fakeError.HttpCode = 404;
                onError(fakeError);
            }
            else
            {
                apiAction(onSuccess, onError);
            }

            while (!callComplete)
            {
                yield return null;
                timeWaited = Time.time - startTime;

                // Ensure indicator shown after initial delay
                //if (timeWaited > busyIndicatorAfterSec)
                //    BusyScript.Instance.Show();
            }

            if (callSuccess)
                break;

            timeWaited = Time.time - startTime;
            if (timeWaited >= messageBoxAfterSec)
            {
                //BusyScript.Instance.Hide();
                //string message = "Hov, der er noget galt med forbindelsen!\n\nTryk <#ffffff>OK</color> for at prøve igen"; // i8n
                //var wait = MessageBox.Instance.Show(message, MessageBox.Buttons.Ok, fadeInBackground: false);
                //yield return wait;
            }

            //if (timeWaited >= busyIndicatorAfterSec)
            //    BusyScript.Instance.Show();

            // Wait a bit so user can't spam retry
            yield return new WaitForSeconds(apiCallRetryTime);
        }

        //        BusyScript.Instance.Hide();

        float timeTotal = Time.time - startTime;
        Debug.LogFormat("API ms: {0}", timeTotal);
        LastResult = result;
        yield return result; // For CoroutineWithData
    }
}
