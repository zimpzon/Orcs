using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum MysteryReward
{
    FasterTime = 0, FasterIncome = 1, FixedIncome = 2, Last,
}

public class QuestionmarkScript : MonoBehaviour
{
    public GameObject Popup;
    public TextMeshProUGUI Text;
    public Button Button;
    public GameObject RadialRoot;
    public Image RadialImage;
    public TextMeshProUGUI MoneyMultiplierText;
    private Vector3 _pulseTextOriginalScale;

    public Color TextColorEnabled;
    public Color TextColorDisabled;

    private void Awake()
    {
        StartCoroutine(Think());
        GameEvents.OnSaveWiped += OnSaveWiped;
    }

    void OnSaveWiped(GameEvents.SaveWipeReason reason)
    {
        StopAllCoroutines();
        StartCoroutine(Think());
    }

    private static string GetText(TimeSpan timeLeft)
        => $"?\n<size=-10>{timeLeft.Minutes:00}:{timeLeft.Seconds:00}</size>";

    public void OnButtonClick()
    {
        _buttonClicked = true;
    }

    bool _buttonClicked = false;
    int _lastDisplayedSeconds = -1;

    float _lastRealTime;

    IEnumerator Think()
    {
        // When we get here game was (re)started. Could be reload or gone for the night, a month, a year.
        while (true)
        {
            RadialRoot.SetActive(false);

            if (SaveGame.Members.QuestionMarkRealTimeLeft <= 0)
            {
                // Either first run or a cycle completed (will set QuestionMarkRealTimeStart to -1).
                // If we just loaded a save we just continue from there.
                //long cycleSeconds = 3;
                long cycleSeconds = UnityEngine.Random.Range(60 * 50, (60 * 60) - 1);
                SaveGame.Members.QuestionMarkRealTimeLeft = cycleSeconds;

                _lastRealTime = G.D.RealTime;
            }

            // Prepare countdown
            Button.interactable = false;
            Text.color = TextColorDisabled;

            // Countdown
            #region Countdown
            while (true)
            {
                yield return null;

                float realTimeDelta = G.D.RealTime - _lastRealTime;
                _lastRealTime = G.D.RealTime;

                SaveGame.Members.QuestionMarkRealTimeLeft -= realTimeDelta;

                if (SaveGame.Members.QuestionMarkRealTimeLeft <= 0)
                    break; // Move to awaiting collection.

                var timeSpan = TimeSpan.FromSeconds(SaveGame.Members.QuestionMarkRealTimeLeft);
                if (timeSpan.Seconds == _lastDisplayedSeconds)
                    continue;

                _lastDisplayedSeconds = timeSpan.Seconds;
                Text.text = GetText(timeSpan);
            }
            #endregion

            // Prepare collection
            #region
            _buttonClicked = false;

            // Pop in with punch-like bounce
            gameObject.transform.localScale = Vector3.one;

            LeanTween.scale(gameObject, Vector3.one * 1.1f, 0.5f)
                .setEaseOutQuad().setLoopPingPong();

            Text.text = "?\n<size=-10><i>Ready!";
            Text.color = TextColorEnabled;
            Button.interactable = true;
            #endregion

            // Awaiting collection
            while (true)
            {
                if (_buttonClicked)
                    break;

                yield return null;
            }

            // Collected
            LeanTween.cancel(gameObject);
            gameObject.transform.localScale = Vector3.one;

            bool isFirst = ++SaveGame.Members.MysteryCollected == 1;
            MysteryReward mysteryReward = GetRandomReward(isFirst);

            yield return BeginReward(mysteryReward);

            // Mark begin of new cycle.
            SaveGame.Members.QuestionMarkRealTimeLeft = -1;
        }

        MysteryReward GetRandomReward(bool isFirst)
        {
            int randomReward = UnityEngine.Random.Range(0, (int)MysteryReward.Last);
            MysteryReward selectedReward = isFirst switch
            {
                true => MysteryReward.FasterIncome,
                false => (MysteryReward)randomReward,
            };
            return selectedReward;
        }

        IEnumerator BeginReward(MysteryReward reward)
        {
            if (reward == MysteryReward.FasterTime)
            {
                yield return FasterTimeCo();
            }
            else if (reward == MysteryReward.FasterIncome)
            {
                yield return FasterIncomeCo();
            }
            else if (reward == MysteryReward.FixedIncome)
            {
                yield return FixedIncomeCo();
            }
            else
            {
                ShowMessage($"Error, unknown reward: {reward}");
            }
        }

        void InitRadial()
        {
            Text.text = "";
            RadialRoot.SetActive(true);
        }

        void SetRadialProgress(float startRealTime, float endRealTime)
        {
            float realTime = G.D.RealTime;

            float duration = endRealTime - startRealTime;
            float elapsed = realTime - startRealTime;
            float t = Mathf.Clamp01(elapsed / duration);

            RadialImage.fillAmount = 1.0f - t;
        }

        IEnumerator FasterTimeCo()
        {
            InitRadial();

            BeginPulseText();

            float startRealTime = G.D.RealTime;
            float endRealTime = startRealTime + (60 * 5) + 5;
            //float endRealTime = startRealTime + 5;
            ShowMessage("Time runs 33% faster for 5 minutes and 5 seconds!");

            PlayerUpgrades.Data.TimeScale = 1.33f;

            while (G.D.RealTime < endRealTime)
            {
                SetRadialProgress(startRealTime, endRealTime);
                yield return null;
            }

            PlayerUpgrades.Data.TimeScale = 1.0f;
        }

        IEnumerator FasterIncomeCo()
        {
            InitRadial();

            float startRealTime = G.D.RealTime;
            //float endRealTime = startRealTime + 5;
            float endRealTime = startRealTime + 60;
            ShowMessage("X10 income for 1 minute!");

            MoneyMultiplierText.text = "X10";
            MoneyMultiplierText.gameObject.SetActive(true);
            BeginPulseText();

            PlayerUpgrades.Data.PassiveIncomeTempMultiplier = 10.0f;

            while (G.D.RealTime < endRealTime)
            {
                SetRadialProgress(startRealTime, endRealTime);
                yield return null;
            }

            PlayerUpgrades.Data.PassiveIncomeTempMultiplier = 1.0f;
            MoneyMultiplierText.gameObject.SetActive(false);
            StopPulseText();
        }

        void BeginPulseText()
        {
            _pulseTextOriginalScale = MoneyMultiplierText.gameObject.transform.localScale;

            LeanTween.scale(MoneyMultiplierText.gameObject, Vector3.one * 1.4f, 0.15f)
                .setEaseInOutSine()
                .setLoopPingPong();
        }

        void StopPulseText()
        {
            LeanTween.cancel(MoneyMultiplierText.gameObject);
            MoneyMultiplierText.gameObject.transform.localScale = _pulseTextOriginalScale;
        }

        IEnumerator FixedIncomeCo()
        {
            long numberOfSeconds = 60 * 5;
            Decimal256 reward = GameManager.Instance.TotalPassiveIncome * (Decimal256)numberOfSeconds;
            reward += 1000;

            ShowMessage($"300X income! ${Format256.Format(reward)}");
            reward += 100;

            GameManager.Instance.AddMoney(reward);
            yield return null;
        }

        void ShowMessage(string msg)
        {
            var pos = GameManager.ArenaBounds.center + Vector2.down * 3;
            FloatingTextSpawner.Instance.Spawn(
                pos,
                $"<size=+1>MYSTERY COLLECTED</size>\n<color=yellow>{msg}</color>",
                Color.white,
                speed: 0.5f,
                timeToLive: 14.0f,
                fontStyle: TMPro.FontStyles.Bold);
        }
    }
}
