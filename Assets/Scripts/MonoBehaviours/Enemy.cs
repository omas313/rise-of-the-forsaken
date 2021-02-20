using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemy : BattleParticipant
{
    const string HIT_ANIMATION_BOOL_KEY = "IsGettingHit";
    const string DEATH_ANIMATION_BOOL_KEY = "IsDead";
    const string ATTACK_ANIMATION_TRIGGER_KEY = "Attack";
    const string IDLE_ANIMATION_TRIGGER_KEY = "Idle";

    public override string Name => _name;
    public override CharacterStats CharacterStats => _stats;
    public override bool IsDead => _stats.CurrentHP <= 0;

    [SerializeField] EnemyDefinition _definition;
    [SerializeField] float _chanceToAttackWeakest = 0.8f;
    [SerializeField] string _name;
    [SerializeField] ParticleSystem _deathParticles;

    CharacterStats _stats;
    Animator _animator;
    Collider2D _collider;
    private bool _isInitialized;

    public void Initialize(EnemyDefinition definition)
    {
        _definition = definition;

        CopyStats();
        GetComponentInChildren<SpriteRenderer>().sprite = _definition.Sprite;
        _name = _definition.name;
        attacks = _definition.Attacks;

        _isInitialized = true;
    }

    public override void TurnOnCollider() => _collider.enabled = true;
    public override void TurnOffCollider() => _collider.enabled = false;

    public override IEnumerator ReceiveAttack(BattleAttack attack)
    {
        _animator.SetBool(HIT_ANIMATION_BOOL_KEY, true);

        yield return new WaitForSeconds(0.5f);

        BattleEvents.InvokeDamageReceived(attack.Damage, transform.position);
        _stats.ReduceCurrentHP(attack.Damage);

        _animator.SetBool(HIT_ANIMATION_BOOL_KEY, false);
    }

    public override IEnumerator PerformAction(List<PartyMember> playerParty, List<Enemy> enemies)
    {
        var target = GetTarget(playerParty); target = playerParty
            .OrderBy(pm => pm.CharacterStats.CurrentHP)
            .FirstOrDefault();

        yield return PerformAttack(GetTarget(playerParty));
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
        
        _animator.SetTrigger(ATTACK_ANIMATION_TRIGGER_KEY);
        yield return new WaitForSeconds(0.2f); 
        yield return new WaitUntil(() => _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.8f);
    
        yield return attackReceiver.ReceiveAttack(new BattleAttack(randomAttack.Damage));
        // Debug.Log($"{Name} {randomAttack.Name} does {randomAttack.Damage} damage to {attackReceiver.Name}");

    }
    
    public override IEnumerator Die()
    {
        BattleEvents.InvokeParticipantIsDying(this);
        _animator.SetBool(DEATH_ANIMATION_BOOL_KEY, true);
        yield return new WaitForSeconds(0.25f); 
        _deathParticles.Play();
        yield return new WaitUntil(() => _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f);
        GetComponentInChildren<SpriteRenderer>().enabled = false;
        BattleEvents.InvokeParticipantIsDead(this);
    }

    void CopyStats()
    {
        _stats = new CharacterStats();
        _stats.SetCurrentHP(_definition.Stats.BaseHP);
        _stats.SetCurrentSpeed(_definition.Stats.BaseSpeed);
        _stats.SetCurrentMP(_definition.Stats.BaseMP);
    }

    IEnumerator StartAnimation(float delay)
    {
        yield return new WaitForSeconds(delay);
        _animator.SetTrigger(IDLE_ANIMATION_TRIGGER_KEY);
    }

    void Update()
    {
        if (!_isInitialized && _definition != null)
            Initialize(_definition);
    }

    void Awake()
    {
        _animator = GetComponent<Animator>();
        StartCoroutine(StartAnimation(UnityEngine.Random.Range(0.15f, 1f)));
        
        _collider = GetComponent<Collider2D>();
    }
}

