using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemy : BattleParticipant
{

    public override string Name => _name;
    public override CharacterStats CharacterStats => _stats;
    public override bool IsDead => _stats.CurrentHP <= 0;

    [SerializeField] float _chanceToAttackWeakest = 0.8f;
    [SerializeField] string _name;
    [SerializeField] int _speed;
    [SerializeField] int _startingHp;
    [SerializeField] CharacterStats _stats;

    public override IEnumerator ReceiveAttack(AttackDefinition attack)
    {
        // do animations and other stuff
        yield return new WaitForSeconds(0.25f);
        _stats.ReduceCurrentHP(attack.Damage);
    }

    public override IEnumerator PerformAction(List<PartyMember> playerParty, List<Enemy> enemies)
    {
        var target = GetTarget(playerParty); target = playerParty
            .OrderBy(pm => pm.CharacterStats.CurrentHP)
            .FirstOrDefault();

        yield return PerformAttack(GetTarget(playerParty));
        yield return new WaitForSeconds(0.25f);
    }

    PartyMember GetTarget(List<PartyMember> playerParty)
    {
        PartyMember target; 
        
        if (UnityEngine.Random.value < _chanceToAttackWeakest)
            target = playerParty
                .OrderBy(pm => pm.CharacterStats.CurrentHP)
                .FirstOrDefault();
        else
            target = playerParty[playerParty.Count - 1];

        return target;
    }

    IEnumerator PerformAttack(BattleParticipant attackReceiver)
    {
        // do animations and other stuff
        var randomAttack = attacks[UnityEngine.Random.Range(0, attacks.Length)];
        yield return attackReceiver.ReceiveAttack(randomAttack);
        Debug.Log($"{Name} {randomAttack.Name} does {randomAttack.Damage} damage to {attackReceiver.Name}");
        yield return new WaitForSeconds(0.25f);
    }
    
    public override IEnumerator Die()
    {
        Debug.Log($"{Name} is dying...");
        yield return new WaitForSeconds(0.5f);

        GetComponentInChildren<SpriteRenderer>().enabled = false;
    }

}

