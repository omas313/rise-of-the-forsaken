using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BattleParticipant : MonoBehaviour
{
    public abstract string Name { get; }
    public abstract bool IsDead { get; }
    public abstract CharacterStats CharacterStats { get; }
    public AttackDefinition[] Attacks => attacks;

    
    [SerializeField] protected AttackDefinition[] attacks;


    public abstract IEnumerator PerformAction(List<PartyMember> playerParty, List<Enemy> enemies);
    public abstract IEnumerator Die();
    public abstract IEnumerator ReceiveAttack(AttackDefinition attack);
}

