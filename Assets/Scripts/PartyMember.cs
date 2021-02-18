using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PartyMember : BattleParticipant
{
    public override string Name => _name;
    public override CharacterStats CharacterStats => _stats;
    public override bool IsDead => _stats.CurrentHP <= 0;

    public PartyMemberStats PartyMemberStats => _stats;
    public Element InnateElement => _innateElement;
    public MagicAttackDefinition[] MagicAttacks => _magicAttacks;
    public PartyMember LinkedPartyMember => _linkedPartyMember;
    public bool HasLink => _linkedPartyMember != null;
    public bool IsLinkBroken => _stats.LinkedHP <= 0;

    [SerializeField] MagicAttacksStore _magicAttacksStore;
    [SerializeField] Element _innateElement;
    [SerializeField] string _name;
    [SerializeField] int _speed;
    [SerializeField] int _startingHp;
    [SerializeField] PartyMemberStats _stats;
    
    Enemy _selectedEnemyToAttack;
    MagicAttackDefinition _selectedMagicAttack;
    MagicAttackDefinition[] _magicAttacks;
    PartyMember _linkedPartyMember;
    PartyMember _selectedPartyMemberToLink;
    bool _requestedUnlink;

    public IEnumerator PreTurnAction(List<PartyMember> playerParty, List<Enemy> enemies)
    {
        IncreaseMP();
        yield return null;
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

    public override IEnumerator ReceiveAttack(AttackDefinition attack)
    {
        // do animations and other stuff

        if (HasLink)
        {
            _stats.ReduceLinkedHP(attack.Damage);
            _linkedPartyMember._stats.ReduceLinkedHP(attack.Damage);

            if (IsLinkBroken)
                yield return TryUnlink();
        }
        else
            _stats.ReduceCurrentHP(attack.Damage);

        yield return new WaitForSeconds(0.25f);
    }

    public override IEnumerator Die()
    {
        Debug.Log($"{Name} is dying...");
        yield return new WaitForSeconds(0.5f);
        GetComponentInChildren<SpriteRenderer>().color = Color.black;
    }

    public bool HasManaFor(MagicAttackDefinition magicAttack)
    {
        if (HasLink)
            return _stats.LinkedMP >= magicAttack.MPCost;
        
        return _stats.CurrentMP >= magicAttack.MPCost;
    }

    IEnumerator PerformAttack(BattleParticipant attackReceiver)
    {
        // do animations and other stuff

        var attack = _selectedMagicAttack ??  attacks[UnityEngine.Random.Range(0, attacks.Length)];
        yield return attackReceiver.ReceiveAttack(attack);

        if (_selectedMagicAttack)
            ConsumeMP(_selectedMagicAttack.MPCost);

        if (!_selectedMagicAttack)
            IncreaseMP();

        Debug.Log($"{Name} {attack.Name} does {attack.Damage} damage to {attackReceiver.Name}");
        yield return new WaitForSeconds(0.5f);    
    }

    void ConsumeMP(int amount)
    {
        if (HasLink)
        {
            _stats.ReduceLinkedMP(amount);
            _linkedPartyMember._stats.ReduceLinkedMP(amount);
        }
        else
            _stats.ReduceCurrentMP(amount);
    }

    void IncreaseMP()
    {
        if (HasLink)
        {
            _stats.IncreaseLinkedMP(1);
            _linkedPartyMember._stats.IncreaseLinkedMP(1);
        }
        else
            _stats.IncreaseCurrentMP(1);
    }

    IEnumerator Link(PartyMember partyMember)
    {
        // do animations and other stuff

        if (partyMember.HasLink)
            yield return partyMember.TryUnlink();
        if (HasLink)
            yield return TryUnlink();

        HandleLink(partyMember);
        partyMember.HandleLink(this);

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
        _linkedPartyMember = null;
        SetMagicAttacks();
        UnsetLinkedStats();
        yield return new WaitForSeconds(0.25f);
    }

    void HandleLink(PartyMember linkedMember)
    {
        _linkedPartyMember = linkedMember;

        SetLinkedStats();
        SetMagicAttacks();
    }

    void SetLinkedStats()
    {
        _stats.SetLinkedHP((int)((_stats.CurrentHP + _linkedPartyMember._stats.CurrentHP) * 1.25f));
        _stats.SetLinkedMP(Mathf.Max(2, (int)((_stats.CurrentMP + _linkedPartyMember._stats.CurrentMP) * 1.25f)));
    }
    
    void UnsetLinkedStats()
    {
        _stats.SetLinkedHP(0);
        _stats.SetLinkedMP(0);
    }

    void SetMagicAttacks()
    {
        _magicAttacks = _magicAttacksStore.GetMagicAttackWithElement(_innateElement);

        if (_linkedPartyMember != null)
            AddLinkedMagicAttacks();
    }

    void AddLinkedMagicAttacks()
    {
        var linkedAttacks = _magicAttacksStore.GetMagicAttacksWithElements(
            _innateElement,
            _linkedPartyMember.InnateElement);

        _magicAttacks = _magicAttacks.Concat(linkedAttacks).ToArray();
    }

    void SetLinkedStats(PartyMember linkedMember)
    {
        
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
        BattleEvents.EnemyTargetSelected += OnEnemyTargetSelected;
        BattleEvents.MagicAttackSelected += OnMagicAttackSelected;
        BattleEvents.PartyMembersLinked += OnPartyMembersLinked;
        BattleEvents.PartyMembersUnlinked += OnPartyMembersUnlinked;
        SetMagicAttacks();
    }
}
