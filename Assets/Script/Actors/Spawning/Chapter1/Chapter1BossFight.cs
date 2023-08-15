using System.Collections;
using UnityEngine;

public static class Chapter1BossFight
{
    static Chapter1Controller C;

    public static IEnumerator Run(Chapter1Controller controller)
    {
        C = controller;

        C.HpBar.Show();

        // enable player
        G.D.PlayerScript.StopPuppet(moveToBegin: true);
        G.D.PlayerScript.TryEnableToggledEffects();

        while (G.D.PlayerScript.IsPuppet)
            yield return null;

        const float BossSpeed = 5.0f;

        yield return Chapter1BossUtil.MoveBoss(C.Boss.transform, Vector2.zero + C.BossOffsetY, BossSpeed);
        yield return C.Boss.Speak("Spiralus Alottomus!", pause: 1, sound: false);

        C.Boss.StartCoroutine(Chapter1BossUtil.FireballSpiral(C.Boss.transform));
    }
}
