using System;

public static class BattleEvents
{
    public static event Action<Enemy> EnemyTargetSelected;
    public static event Action<PartyMember> PartyMemberSelected;
 
    public static void InvokeEnemyTargetSelected(Enemy enemy) => EnemyTargetSelected?.Invoke(enemy);
    public static void InvokePartyMemberSelected(PartyMember member) => PartyMemberSelected?.Invoke(member);


}