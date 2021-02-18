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
    public PartyMember LinkedPartyMember => _linkedPartyMember;
    public bool HasLink => _linkedPartyMember != null;

    [SerializeField] MagicAttacksStore _magicAttacksStore;
    [SerializeField] Element _innateElement;
    [SerializeField] string _name;
    [SerializeField] int _speed;
    [SerializeField] int _startingHp;
    
    CharacterStats _characterStats;
    Enemy _selectedEnemyToAttack;
    MagicAttackDefinition _selectedMagicAttack;
    MagicAttackDefinition[] _magicAttacks;
    PartyMember _linkedPartyMember;
    PartyMember _selectedPartyMemberToLink;
    private bool _requestedUnlink;

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
        _selectedPartyMemberToLink = null;
        _requestedUnlink = false;

        yield return new WaitUntil(() => _selectedEnemyToAttack != null || _selectedPartyMemberToLink != null || _requestedUnlink);

        if (_selectedEnemyToAttack != null)
            yield return PerformAttack(_selectedEnemyToAttack);
        else if (_selectedPartyMemberToLink != null)
            yield return Link(_selectedPartyMemberToLink);
        else if (_requestedUnlink)
            yield return TryUnlink();
    }

    IEnumerator PerformAttack(BattleParticipant attackReceiver)
    {
        // do animations and other stuff

        var attack = _selectedMagicAttack ??  attacks[UnityEngine.Random.Range(0, attacks.Length)];
        yield return attackReceiver.ReceiveAttack(attack);

        Debug.Log($"{Name} {attack.Name} does {attack.Damage} damage to {attackReceiver.Name}");
        yield return new WaitForSeconds(0.5f);    
    }

    IEnumerator Link(PartyMember partyMember)
    {
        // do animations and other stuff

        if (partyMember.HasLink)
            yield return partyMember.TryUnlink();
        if (HasLink)
            yield return TryUnlink();

        SetMagicAttacks(partyMember);
        _linkedPartyMember.SetMagicAttacks(this);

        Debug.Log($"{Name} linked to {partyMember.Name}");
        yield return new WaitForSeconds(0.5f);    
    }

    IEnumerator TryUnlink()
    {
        // do animations and other stuff
        if (_linkedPartyMember == null)
            Debug.Log("Error: Requested to unlink but no linked member registered");

        yield return _linkedPartyMember.Unlink();
        yield return Unlink();

        yield return new WaitForSeconds(0.5f);    
    }

    IEnumerator Unlink()
    {
        Debug.Log($"{Name} unlinked from {_linkedPartyMember.Name}");

        // do animations and stuff
        SetMagicAttacks();
        // set stats
        _linkedPartyMember = null;
        yield return new WaitForSeconds(0.25f);
    }

    void SetMagicAttacks(PartyMember partyMember = null)
    {
        _magicAttacks = _magicAttacksStore.GetMagicAttackWithElement(_innateElement);

        _linkedPartyMember = partyMember;
        if (_linkedPartyMember != null)
            AddLinkedMagicAttacks();
    }

    void AddLinkedMagicAttacks()
    {
        var linkedAttacks = _magicAttacksStore.GetMagicAttacksWithElements(
            _innateElement,
            _linkedPartyMember.InnateElement);
        // Debug.Log(linkedAttacks.Length);
        _magicAttacks = _magicAttacks.Concat(linkedAttacks).ToArray();
    }

    void OnPartyMembersLinked(PartyMember member1, PartyMember member2)
    {
        _selectedPartyMemberToLink = member1 == this ? member2 : member1;
    }

    void OnPartyMembersUnlinked(PartyMember member1, PartyMember member2)
    {
        if (member1 == this)
            _requestedUnlink = true;
    }

    private void OnEnemyTargetSelected(Enemy enemy) => _selectedEnemyToAttack = enemy;
    private void OnMagicAttackSelected(MagicAttackDefinition magicAttack) => _selectedMagicAttack = magicAttack;

    void Awake()
    {
        _characterStats = new CharacterStats 
        { 
            CurrentSpeed = _speed,
            CurrentHP = _startingHp
        };

        BattleEvents.EnemyTargetSelected += OnEnemyTargetSelected;
        BattleEvents.MagicAttackSelected += OnMagicAttackSelected;
        BattleEvents.PartyMembersLinked += OnPartyMembersLinked;
        BattleEvents.PartyMembersUnlinked += OnPartyMembersUnlinked;
        SetMagicAttacks();
    }
}
