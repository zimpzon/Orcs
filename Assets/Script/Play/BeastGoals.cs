// Slaying goals, the post-game after Scary Earl: once he's beaten, every beast needs Goal kills (counted from his
// defeat - ScaryEarlFight.MarkDefeated clears the counts) to be "decimated": a "BEAST DECIMATED!" announcement
// (GameManager.OnEnemyKill), a gold tile with a crown in the bestiary, and the hover text says so. Before his defeat
// none of this shows. Flaps (the very first beast) counts as decimated from the start - "enough".
public static class BeastGoals
{
    public const long Goal = 100;

    public static bool Active => SaveGame.Members.ScaryEarlDefeated;

    // Flaps: everyone has killed more than enough of them by then.
    public static bool AlwaysDone(ActorTypeEnum type) => type == ActorTypeEnum.BatWhite;

    public static bool IsBeast(ActorTypeEnum type) => type != ActorTypeEnum.None && type != ActorTypeEnum.Any;

    public static bool IsMastered(ActorTypeEnum type)
        => Active && IsBeast(type) && (AlwaysDone(type) || SaveGame.Members.GetBeastKills(type) >= Goal);

    // The kill that reaches the goal right now (announce it once).
    public static bool JustMastered(ActorTypeEnum type)
        => Active && IsBeast(type) && !AlwaysDone(type) && SaveGame.Members.GetBeastKills(type) == Goal;
}
