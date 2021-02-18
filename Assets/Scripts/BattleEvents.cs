using System;

public static class BattleEvents
{
    public static event Action<Enemy> EnemyTargetSelected;
    public static event Action<PartyMember> PartyMemberSelected;
    public static event Action<PartyMember, PartyMember> PartyMembersLinked;
    public static event Action<PartyMember, PartyMember> PartyMembersUnlinked;
    public static event Action<MagicAttackDefinition> MagicAttackSelected;
 
    public static void InvokeEnemyTargetSelected(Enemy enemy) => EnemyTargetSelected?.Invoke(enemy);
    public static void InvokePartyMemberSelected(PartyMember member) => PartyMemberSelected?.Invoke(member);
    public static void InvokePartyMembersLinked(PartyMember member1, PartyMember member2) => PartyMembersLinked?.Invoke(member1, member2);
    public static void InvokePartyMemberUnlinked(PartyMember member1, PartyMember member2) => PartyMembersUnlinked?.Invoke(member1, member2);
    public static void InvokeMagicAttackSelected(MagicAttackDefinition magicAttack) => MagicAttackSelected?.Invoke(magicAttack);
}