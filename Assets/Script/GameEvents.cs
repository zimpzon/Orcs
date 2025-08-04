public static class GameEvents
{
    public enum SaveWipeReason { UserWipe, Ascended, };

    public static event System.Action<SaveWipeReason> OnSaveWiped;

    public static void RaiseSaveWiped(SaveWipeReason reason)
    {
        OnSaveWiped?.Invoke(reason);
    }
}
