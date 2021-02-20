using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PartyMember : BattleParticipant
{
    const string CAST_ANIMATION_BOOL_KEY = "IsCastingSpell";
    const string HIT_ANIMATION_BOOL_KEY = "IsGettingHit";
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

    [SerializeField] MagicAttacksStore _magicAttacksStore;
    [SerializeField] Element _innateElement;
    [SerializeField] string _name;
    [SerializeField] int _speed;
    [SerializeField] int _startingHp;
    [SerializeField] PartyMemberStats _stats;
    [SerializeField] Transform _spellCasePoint;
    [SerializeField] ParticleSystem _castParticles;
    
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
        _selectedEnemyToAttack = null;
        _selectedMagicAttack = null;
        _selectedPartyMemberToLink = null;
        _requestedUnlink = false;

        yield return new WaitUntil(() => _selectedEnemyToAttack != null || _selectedPartyMemberToLink != null || _requestedUnlink);

        if (_selectedEnemyToAttack != null)
            yield return PerformAttack(enemies, _selectedEnemyToAttack);
        else if (_selectedPartyMemberToLink != null)
            yield return Link(_selectedPartyMemberToLink);
        else if (_requestedUnlink)
            yield return TryUnlink();
    }

    public override IEnumerator ReceiveAttack(BattleAttack attack)
    {
        // do animations and other stuff

        _animator.SetBool(HIT_ANIMATION_BOOL_KEY, true);

        if (HasLink)
        {
            _stats.ReduceLinkedHP(attack.Damage);
            _linkedPartyMember._stats.ReduceLinkedHP(attack.Damage);

            if (IsLinkBroken)
                yield return TryUnlink();
        }
        else
            _stats.ReduceCurrentHP(attack.Damage);

        BattleEvents.InvokeDamageReceived(attack.Damage, transform.position);

        yield return new WaitForSeconds(0.5f);

        _animator.SetBool(HIT_ANIMATION_BOOL_KEY, false);
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

    IEnumerator PerformAttack(List<Enemy> enemies, BattleParticipant attackReceiver)
    {
        // do animations and other stuff

        var attackDefinition = _selectedMagicAttack ??  attacks[UnityEngine.Random.Range(0, attacks.Length)];
        var attack = new BattleAttack(attackDefinition.Damage);

        if (_selectedMagicAttack)
        {
            attackReceiver.TurnOnCollider();
            _castParticles.Play();
            _animator.SetBool(CAST_ANIMATION_BOOL_KEY, true);

            // cast time can be added to magic attack def
            yield return new WaitForSeconds(2f); 
            // let magic SO do the casting and spawning
            yield return SpawnParticles(attackReceiver);
            
            attackReceiver.TurnOffCollider();
            _castParticles.Stop();
            _animator.SetBool(CAST_ANIMATION_BOOL_KEY, false);
        }
        else
        {
            _animator.SetTrigger(ATTACK_ANIMATION_TRIGGER_KEY);
            yield return new WaitForSeconds(0.2f); 
            yield return new WaitUntil(() => _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);
        }
    
        // can let magic SO do the damage dealing
        yield return attackReceiver.ReceiveAttack(attack);

        if (_selectedMagicAttack && _selectedMagicAttack.MagicAttackTargetType == MagicAttackTargetType.AOE)
            foreach (var enemy in enemies)
                if (enemy != attackReceiver)
                    StartCoroutine(enemy.ReceiveAttack(attack));

        if (_selectedMagicAttack)
            ConsumeMP(_selectedMagicAttack.MPCost);

        if (!_selectedMagicAttack)
            IncreaseMP();

        // Debug.Log($"{Name} {attackDefinition.Name} does {attackDefinition.Damage} damage to {attackReceiver.Name}");
        yield return new WaitForSeconds(0.5f);    
    }

    IEnumerator SpawnParticles(BattleParticipant attackReceiver)
    {
        // needs to go in SO
        if (_selectedMagicAttack.MagicAttackTargetType == MagicAttackTargetType.Single)
        {
            var angle = Vector2.SignedAngle(Vector2.left, (attackReceiver.transform.position - transform.position).normalized);
            var rotation = Quaternion.Euler(0f, 0f, angle);
            var particles = Instantiate(_selectedMagicAttack.EffectPrefab, _spellCasePoint.position, rotation);
            var particleSystemMain = particles.GetComponent<ParticleSystem>().main;
            particleSystemMain.startRotation = new ParticleSystem.MinMaxCurve(Mathf.Deg2Rad * -angle);

            var magicAttackHandler = particles.GetComponent<AttackParticleEventHandler>();
            yield return new WaitUntil(() => magicAttackHandler.HasFinished || magicAttackHandler.HasMadeImpact);
        }
        else if (_selectedMagicAttack.MagicAttackTargetType == MagicAttackTargetType.AOE)
        {
            var magicAttackHandler = Instantiate(_selectedMagicAttack.EffectPrefab).GetComponent<AttackParticleEventHandler>();
            yield return new WaitUntil(() => magicAttackHandler.HasFinished || magicAttackHandler.HasMadeImpact);
        }
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
            _linkedPartyMember.Element);

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
        _animator = GetComponent<Animator>();
        _collider = GetComponent<Collider2D>();

        BattleEvents.EnemyTargetSelected += OnEnemyTargetSelected;
        BattleEvents.MagicAttackSelected += OnMagicAttackSelected;
        BattleEvents.PartyMembersLinked += OnPartyMembersLinked;
        BattleEvents.PartyMembersUnlinked += OnPartyMembersUnlinked;
        SetMagicAttacks();
    }
}
