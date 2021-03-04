using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MagicAttackDefinition.asset", menuName = "Magic Attack Definition")]
public class MagicAttackDefinition : AttackDefinition
{
    public int MPCost => _mpCost;
    public float CastTime => _castTime;
    public Element[] Elements => _elements;
    public GameObject EffectPrefab => _effectPrefab;
    public MagicAttackTargetType MagicAttackTargetType => _magicAttackTargetType;
    public bool IsMultiElement => _elements.Length > 1;

    [SerializeField] int _mpCost;
    [SerializeField] float _castTime = 2f;
    [SerializeField] Element[] _elements;
    [SerializeField] GameObject _effectPrefab;
    [SerializeField] MagicAttackTargetType _magicAttackTargetType;

    public IEnumerator Perform(PartyMember performer, Enemy receiver, List<Enemy> enemies)
    {
        yield return DoCastVisuals(performer, receiver);
        yield return DoDamage(performer, receiver, enemies); 
    }

    IEnumerator DoCastVisuals(PartyMember performer, Enemy receiver)
    {
        performer.StartCastVisuals();
        if (IsMultiElement && performer.HasLink)
            performer.LinkedPartyMember.StartCastVisuals();

        yield return new WaitForSeconds(_castTime); 

        receiver.TurnOnCollider();
        yield return SpawnParticles(performer, receiver);
        receiver.TurnOffCollider();
        
        performer.StopCastVisuals();
        if (IsMultiElement && performer.HasLink)
            performer.LinkedPartyMember.StopCastVisuals();
    }

    IEnumerator SpawnParticles(PartyMember performer, Enemy receiver)
    {
        // SO to handle this?
        switch (MagicAttackTargetType)
        {
            case MagicAttackTargetType.Single:
                var angle = Vector2.SignedAngle(Vector2.left, (receiver.transform.position - performer.transform.position).normalized);
                var rotation = Quaternion.Euler(0f, 0f, angle);
                var particles = Instantiate(EffectPrefab, performer.SpellCastPoint, rotation);
                var particleSystemMain = particles.GetComponent<ParticleSystem>().main;
                particleSystemMain.startRotation = new ParticleSystem.MinMaxCurve(Mathf.Deg2Rad * -angle);

                var magicAttackHandler = particles.GetComponent<AttackParticleEventHandler>();
                yield return new WaitUntil(() => magicAttackHandler.HasFinished || magicAttackHandler.HasMadeImpact);
                break;

            case MagicAttackTargetType.AOE:
                var handler = Instantiate(EffectPrefab).GetComponent<AttackParticleEventHandler>();
                yield return new WaitUntil(() => handler.HasFinished || handler.HasMadeImpact);
                break;

            default:
                break;
        }
    }

    IEnumerator DoDamage(PartyMember performer, Enemy receiver, List<Enemy> enemies)
    {
        var battleAttack = new BattleAttack(Damage);

        /// let so handle this?
        switch (MagicAttackTargetType)
        {
            case MagicAttackTargetType.Single:
                yield return receiver.ReceiveAttack(battleAttack);
                break;

            case MagicAttackTargetType.AOE:
                foreach (var enemy in enemies)
                    performer.StartCoroutine(enemy.ReceiveAttack(battleAttack));
                yield return new WaitForSeconds(0.5f);
                break;

            default:
                break;
        }
    }

}

// possibly SO to make it spawn itself correctly without doing IFs everywhere
public enum MagicAttackTargetType 
{ 
    Single, 
    AOE 
};
