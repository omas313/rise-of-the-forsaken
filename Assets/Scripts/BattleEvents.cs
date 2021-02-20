using System;
using UnityEngine;

public static class BattleEvents
{
    public static event Action<Enemy> EnemyTargetSelected;
    public static event Action<PartyMember> PartyMemberSelected;
    public static event Action<MagicAttackDefinition> MagicAttackSelected;
    public static event Action<int, Vector3> DamageReceived;
    public static event Action<PartyMember, PartyMember> RequestedPartyMembersLink;
    public static event Action<PartyMember, PartyMember> RequestedPartyMembersUnlink;
    public static event Action<PartyMember, PartyMember> PartyMembersLinked;
    public static event Action<PartyMember, PartyMember> PartyMembersUnlinked;
    public static event Action<PartyMember> PartyMemberIsCasting;
    public static event Action<PartyMember> PartyMemberFinishedCasting;

 
    public static void InvokeEnemyTargetSelected(Enemy enemy) => EnemyTargetSelected?.Invoke(enemy);
    public static void InvokePartyMemberSelected(PartyMember member) => PartyMemberSelected?.Invoke(member);
    public static void InvokeMagicAttackSelected(MagicAttackDefinition magicAttack) => MagicAttackSelected?.Invoke(magicAttack);
    public static void InvokeDamageReceived(int damage, Vector3 position) => DamageReceived?.Invoke(damage, position);
    public static void InvokeRequestedPartyMembersLink(PartyMember member1, PartyMember member2) => RequestedPartyMembersLink?.Invoke(member1, member2);
    public static void InvokeRequestedPartyMembersUnlink(PartyMember member1, PartyMember member2) => RequestedPartyMembersUnlink?.Invoke(member1, member2);
    public static void InvokePartyMembersLinked(PartyMember member1, PartyMember member2) => PartyMembersLinked?.Invoke(member1, member2);
    public static void InvokePartyMembersUnlinked(PartyMember member1, PartyMember member2) => PartyMembersUnlinked?.Invoke(member1, member2);
    public static void InvokePartyMemberIsCasting(PartyMember member) => PartyMemberIsCasting?.Invoke(member);
    public static void InvokePartyMemberFinishedCasting(PartyMember member) => PartyMemberFinishedCasting?.Invoke(member);
}