using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PartyMember : BattleParticipant
{
    const string CAST_ANIMATION_BOOL_KEY = "IsCastingSpell";
    const string HIT_ANIMATION_BOOL_KEY = "IsGettingHit";
    const string DEATH_ANIMATION_BOOL_KEY = "IsDead";
    const string IDLE_ANIMATION_TRIGGER_KEY = "Idle";
    const string ATTACK_ANIMATION_TRIGGER_KEY = "Attack";

    public override string Name => _name;
    public override CharacterStats CharacterStats => _stats;
    public override bool IsDead => _stats.CurrentHP <= 0;

    public PartyMemberStats PartyMemberStats => _stats;
    public Element Element => _innateElement;
    public MagicAttackDefinition[] MagicAttacks => _magicAttacks;
    public PartyMember LinkedPartyMember => _linkedPartyMember;
    public bool HasLink => _linkedPartyMember != null;
    public bool IsLinkBroken => _stats.LinkedHP <= 0;
    public Vector3 SpellCastPoint => _spellCasePoint.position;

    [SerializeField] MagicAttacksStore _magicAttacksStore;
    [SerializeField] Element _innateElement;
    [SerializeField] string _name;
    [SerializeField] PartyMemberStats _stats;
    [SerializeField] Transform _spellCasePoint;
    [SerializeField] ParticleSystem _castParticles;
    [SerializeField] ParticleSystem _deathParticles;
    
    Enemy _selectedEnemyToAttack;
    MagicAttackDefinition _selectedMagicAttack;
    MagicAttackDefinition[] _magicAttacks;
    PartyMember _linkedPartyMember;
    PartyMember _selectedPartyMemberToLink;
    Animator _animator;
    Collider2D _collider;
    bool _requestedUnlink;

    public IEnumerator PreTurnAction(List<PartyMember> playerParty, List<Enemy> enemies)
    {
        IncreaseMP();
        yield return null;
    }

    public override void TurnOnCollider() => _collider.enabled = true;
    public override void TurnOffCollider() => _collider.enabled = false;

    public override IEnumerator PerformAction(List<PartyMember> playerParty, List<Enemy> enemies)
    {
        // todo: must find a cleaner way to do this communication with UI, maybe UIEvent pubsub
        _selectedEnemyToAttack = null;
        _selectedMagicAttack = null;
        _selectedPartyMemberToLink = null;
        _requestedUnlink = false;

        yield return new WaitUntil(() => _selectedEnemyToAttack != null || _selectedPartyMemberToLink != null || _requestedUnlink || Input.GetKeyDown(KeyCode.End));

        if (_selectedEnemyToAttack != null)
            yield return PerformAttack(enemies, _selectedEnemyToAttack);
        else if (_selectedPartyMemberToLink != null)
            yield return PerformLink(_selectedPartyMemberToLink);
        else if (_requestedUnlink)
            yield return PerformUnlink();        
    }

    public override IEnumerator ReceiveAttack(BattleAttack attack)
    {
        _animator.SetBool(HIT_ANIMATION_BOOL_KEY, true);
        if (HasLink)
            _linkedPartyMember._animator.SetBool(HIT_ANIMATION_BOOL_KEY, true);

        ReduceHP(attack);

        BattleEvents.InvokeDamageReceived(attack.Damage, transform.position);
        yield return new WaitForSeconds(0.5f);

        _animator.SetBool(HIT_ANIMATION_BOOL_KEY, false);
        if (HasLink)
            _linkedPartyMember._animator.SetBool(HIT_ANIMATION_BOOL_KEY, false);

        if (HasLink && IsLinkBroken)
            yield return PerformUnlink();
    }

    public override IEnumerator Die()
    {
        if (HasLink)
            Debug.Log($"Error: {Name} dying with link to {_linkedPartyMember.Name}");

        BattleEvents.InvokeParticipantIsDying(this);
        _animator.SetBool(DEATH_ANIMATION_BOOL_KEY, true);
        yield return new WaitForSeconds(0.25f); 
        _deathParticles.Play();
        yield return new WaitUntil(() => _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f);
        BattleEvents.InvokeParticipantIsDead(this);
    }

    public bool HasManaFor(MagicAttackDefinition magicAttack)
    {
        if (HasLink)
            return _stats.LinkedMP >= magicAttack.MPCost;
        
        return _stats.CurrentMP >= magicAttack.MPCost;
    }

    public void StartCastVisuals()
    {
        _castParticles.Play();
        _animator.SetBool(CAST_ANIMATION_BOOL_KEY, true);
        BattleEvents.InvokePartyMemberIsCasting(this);
    }

    public void StopCastVisuals()
    {
        _castParticles.Stop();
        _animator.SetBool(CAST_ANIMATION_BOOL_KEY, false);
        BattleEvents.InvokePartyMemberFinishedCasting(this);
    }

    IEnumerator PerformAttack(List<Enemy> enemies, BattleParticipant attackReceiver)
    {
        if (_selectedMagicAttack)
        {
            yield return _selectedMagicAttack.Perform(this, attackReceiver as Enemy, enemies);
            ConsumeMP(_selectedMagicAttack.MPCost);
        }
        else
        {
            _animator.SetTrigger(ATTACK_ANIMATION_TRIGGER_KEY);
            yield return new WaitForSeconds(0.2f); 
            yield return new WaitUntil(() => _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.8f);
            
            var attack = new BattleAttack(attacks[0].Damage);
            yield return attackReceiver.ReceiveAttack(attack);
            IncreaseMP();
        }
    
        // Debug.Log($"{Name} {attackDefinition.Name} does {attackDefinition.Damage} damage to {attackReceiver.Name}");
        yield return new WaitForSeconds(0.5f);    
    }

    IEnumerator PerformLink(PartyMember partyMember)
    {
        if (partyMember.HasLink)
            yield return partyMember.PerformUnlink();

        if (HasLink)
            yield return PerformUnlink();

        yield return new WaitForSeconds(0.25f);    

        HandleLink(partyMember);
        partyMember.HandleLink(this);

        BattleEvents.InvokePartyMembersLinked(this, partyMember);
    }

    IEnumerator PerformUnlink()
    {
        if (_linkedPartyMember == null)
            Debug.Log("Error: Requested to unlink but no linked member registered");

        BattleEvents.InvokePartyMembersUnlinked(this, _linkedPartyMember);
        yield return new WaitForSeconds(0.15f);    
        yield return _linkedPartyMember.Unlink();
        yield return Unlink();
    }

    IEnumerator Unlink()
    {
        yield return new WaitForSeconds(0.15f);    
        _linkedPartyMember = null;
        SetMagicAttacks();
        UnsetLinkedStats();
    }

    void ReduceHP(BattleAttack attack)
    {
        if (HasLink)
        {
            _stats.ReduceLinkedHP(attack.Damage);
            _linkedPartyMember._stats.ReduceLinkedHP(attack.Damage);
        }
        else
            _stats.ReduceCurrentHP(attack.Damage);
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

    void HandleLink(PartyMember linkedMember)
    {
        _linkedPartyMember = linkedMember;

        SetLinkedStats();
        SetMagicAttacks();
    }

    void SetLinkedStats()
    {
        _stats.SetLinkedHP((int)((_stats.CurrentHP + _linkedPartyMember._stats.CurrentHP) * 1.25f));
        _stats.SetLinkedMP(Mathf.Max(2, _stats.CurrentMP + _linkedPartyMember._stats.CurrentMP));
    }
    
    void UnsetLinkedStats()
    {
        _stats.SetLinkedHP(0);
        _stats.SetLinkedMP(0);
    }

    void SetStartingStats()
    {
        _stats.SetCurrentHP(_stats.BaseHP);
        _stats.SetCurrentMP(1);
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
            _linkedPartyMember.Element);

        _magicAttacks = _magicAttacks.Concat(linkedAttacks).ToArray();
    }

    void OnRequestedPartyMembersLink(PartyMember member1, PartyMember member2)
    {
        _selectedPartyMemberToLink = member1 == this ? member2 : member1;
    }

    void OnRequestedPartyMembersUnlink(PartyMember member1, PartyMember member2)
    {
        if (member1 == this)
            _requestedUnlink = true;
    }

    private void OnEnemyTargetSelected(Enemy enemy) => _selectedEnemyToAttack = enemy;
    private void OnMagicAttackSelected(MagicAttackDefinition magicAttack) => _selectedMagicAttack = magicAttack;

    IEnumerator StartIdleAnimationAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        _animator.SetTrigger(IDLE_ANIMATION_TRIGGER_KEY);
    }
    
    void OnDestroy()
    {
        BattleEvents.EnemyTargetSelected -= OnEnemyTargetSelected;
        BattleEvents.MagicAttackSelected -= OnMagicAttackSelected;
        BattleEvents.RequestedPartyMembersLink -= OnRequestedPartyMembersLink;
        BattleEvents.RequestedPartyMembersUnlink -= OnRequestedPartyMembersUnlink;
    }

    void Awake()
    {
        _collider = GetComponent<Collider2D>();
        _animator = GetComponent<Animator>();
        StartCoroutine(StartIdleAnimationAfterDelay(UnityEngine.Random.Range(0.15f, 1f)));

        BattleEvents.EnemyTargetSelected += OnEnemyTargetSelected;
        BattleEvents.MagicAttackSelected += OnMagicAttackSelected;
        BattleEvents.RequestedPartyMembersLink += OnRequestedPartyMembersLink;
        BattleEvents.RequestedPartyMembersUnlink += OnRequestedPartyMembersUnlink;

        SetMagicAttacks();
        SetStartingStats();
    }
}
