using System.Collections;

public static class Chapter1BossFight
{
    static Chapter1Controller C;

    public static IEnumerator Run(Chapter1Controller controller)
    {
        C = controller;

        yield return null;
    }
}
