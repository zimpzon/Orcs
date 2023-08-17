using Assets.Script.Actors.Spawning;
using System.Collections;
using UnityEngine;

public static class Chapter1BossIntro
{
    static Chapter1Controller C;

    public static IEnumerator Run(Chapter1Controller controller, bool skipIntroduction)
    {
        C = controller;

        C.Hounds.transform.LeanSetPosX(C.HoundsHiddenX);

        LeanTween.color(GameManager.Instance.Floor.gameObject, C.ColorBoss, 0.5f);
        LeanTween.color(GameManager.Instance.FloorFilter.gameObject, C.FilterBoss, 0.5f);

        SpawnUtil.FleeAllActors();
        G.D.PlayerScript.SetPuppet(Vector3.zero, Vector2.right);
        G.D.PlayerScript.DisableToggledEffects();

        C.Boss.transform.position = PositionUtility.RightMidOut;
        C.Boss.gameObject.SetActive(true);

        yield return IntroduceBoss(skip: skipIntroduction);

        yield break;
    }

    static IEnumerator IntroduceBoss(bool skip)
    {
        bool bossIsAtStartPosition = false;
        LeanTween.moveLocal(C.Boss.gameObject, PositionUtility.RightMidOut + C.BossOffsetY + Vector2.left * 5, 1.5f).setOnComplete(() => bossIsAtStartPosition = true);

        if (!skip)
        {
            MusicManagerScript.Instance.StopMusic();

            while (!bossIsAtStartPosition)
                yield return null;

            yield return C.Boss.Speak("Well, what do we have here...", 3.0f);

            LeanTween.color(GameManager.Instance.Floor.gameObject, C.ColorHounds, 2.5f);
            LeanTween.color(GameManager.Instance.FloorFilter.gameObject, C.FilterHounds, 2.5f);

            yield return C.Boss.Speak("Merely a plaything for my hounds!", 3.0f);
            LeanTween.moveX(C.Hounds, C.HoundsShownX, 0.25f);
            yield return C.Boss.Speak("Hounds! Feast on this intruder!", 2.0f);

            G.D.PlayerScript.OverheadText.text = "FETCH!";
            G.D.PlayerScript.OverheadText.color = Color.white;
            G.D.PlayerScript.OverheadText.enabled = true;
            Vector2 uiPos = GameManager.Instance.UiPositionFromWorld(G.D.PlayerPos + Vector3.up * 1.5f);
            G.D.PlayerScript.OverheadText.GetComponent<RectTransform>().anchoredPosition = uiPos;

            var saw = GameObject.Instantiate<GameObject>(C.SawbladeProto, C.gameObject.transform);
            saw.transform.position = G.D.PlayerPos;

            bool waitingForSaw = true;
            LeanTween.move(saw, (Vector3)PositionUtility.RightMidOut + Vector3.left * 2 + Vector3.down * 3, 2.0f).setOnComplete(() => waitingForSaw = false).setDestroyOnComplete(true);

            AudioManager.Instance.PlayClip(AudioManager.Instance.AudioData.PlayerThrowBomb);

            while (waitingForSaw)
                yield return null;

            yield return C.Boss.Speak("", 0.0f);

            LeanTween.moveX(C.Hounds, C.HoundsHiddenX, 0.25f);

            C.Boss.ForcedScaleX = -1;
            G.D.PlayerScript.OverheadText.enabled = false;

            yield return new WaitForSeconds(1.0f);

            var repeatingClip = AudioManager.Instance.RepeatingSawblade;
            repeatingClip.StartClip(AudioManager.Instance.AudioData.Chainsaw, volumeScale: 0.8f);

            yield return new WaitForSeconds(3.0f);

            repeatingClip.StopClip();

            LeanTween.color(GameManager.Instance.Floor.gameObject, C.ColorBoss, 2.5f);
            LeanTween.color(GameManager.Instance.FloorFilter.gameObject, C.FilterBoss, 2.5f);

            yield return C.Boss.Speak("...", 3.0f);
            yield return C.Boss.Speak("", 0.0f);

            C.Boss.ForcedScaleX = null;

            MusicManagerScript.Instance.PlayGameMusic(GameManager.Instance.CurrentGameModeData.Music);
        }
    }
}
