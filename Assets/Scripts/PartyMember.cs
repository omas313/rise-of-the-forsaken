using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PartyMember : BattleParticipant
{
    public override string Name => _name;
    public override CharacterStats CharacterStats => _characterStats;
    public override bool IsDead => _characterStats.CurrentHP <= 0;
    public Element InnateElement => _innateElement;
    public MagicAttackDefinition[] MagicAttacks => _magicAttacks;

    [SerializeField] MagicAttacksStore _magicAttacksStore;
    [SerializeField] Element _innateElement;
    [SerializeField] string _name;
    [SerializeField] int _speed;
    [SerializeField] int _startingHp;
    
    CharacterStats _characterStats;
    Enemy _selectedEnemyToAttack;
    MagicAttackDefinition _selectedMagicAttack;
    MagicAttackDefinition[] _magicAttacks;
    PartyMember _currentlyLinkedPartyMember;

    public override IEnumerator ReceiveAttack(AttackDefinition attack)
    {
        // do animations and other stuff
        yield return new WaitForSeconds(0.25f);
        _characterStats.ReduceCurrentHP(attack.Damage);
    }

    public override IEnumerator Die()
    {
        Debug.Log($"{Name} is dying...");
        yield return new WaitForSeconds(0.5f);
        GetComponentInChildren<SpriteRenderer>().color = Color.black;
    }

    public override IEnumerator PerformAction(List<PartyMember> playerParty, List<Enemy> enemies)
    {
        _selectedEnemyToAttack = null;
        _selectedMagicAttack = null;

        yield return new WaitUntil(() => _selectedEnemyToAttack != null);
        yield return PerformAttack(_selectedEnemyToAttack);
    }

    IEnumerator PerformAttack(BattleParticipant attackReceiver)
    {
        // do animations and other stuff

        var attack = _selectedMagicAttack ??  attacks[UnityEngine.Random.Range(0, attacks.Length)];
        yield return attackReceiver.ReceiveAttack(attack);

        Debug.Log($"{Name} {attack.Name} does {attack.Damage} damage to {attackReceiver.Name}");
        yield return new WaitForSeconds(0.5f);    
    }

    void Awake()
    {
        _characterStats = new CharacterStats 
        { 
            CurrentSpeed = _speed,
            CurrentHP = _startingHp
        };

        BattleEvents.EnemyTargetSelected += OnEnemyTargetSelected;
        BattleEvents.MagicAttackSelected += OnMagicAttackSelected;
        SetMagicAttacks();
    }

    void SetMagicAttacks()
    {
        _magicAttacks = _magicAttacksStore.GetMagicAttackWithElement(_innateElement);

        if (_currentlyLinkedPartyMember == null)
            return;

        var linkedAttacks = _magicAttacksStore.GetMagicAttacksWithElements(
            _innateElement,
            _currentlyLinkedPartyMember.InnateElement);

        _magicAttacks = _magicAttacks.Concat(linkedAttacks).ToArray();
    }

    void OnPartyMemberLinked(PartyMember linkedMember)
    {
        _currentlyLinkedPartyMember = linkedMember;
        SetMagicAttacks();
    }

    void OnPartyMemberUnlinked()
    {
        _currentlyLinkedPartyMember = null;
        SetMagicAttacks();
    }

    private void OnEnemyTargetSelected(Enemy enemy) => _selectedEnemyToAttack = enemy;
    private void OnMagicAttackSelected(MagicAttackDefinition magicAttack) => _selectedMagicAttack = magicAttack;
}
