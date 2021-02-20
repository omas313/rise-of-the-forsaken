using UnityEngine;

[CreateAssetMenu(fileName = "MagicAttackDefinition.asset", menuName = "Magic Attack Definition")]
public class MagicAttackDefinition : AttackDefinition
{
    public int MPCost => _mpCost;
    public Element[] Elements => _elements;
    public GameObject EffectPrefab => _effectPrefab;
    public MagicAttackTargetType MagicAttackTargetType => _magicAttackTargetType;

    [SerializeField] int _mpCost;
    [SerializeField] Element[] _elements;
    [SerializeField] GameObject _effectPrefab;
    [SerializeField] MagicAttackTargetType _magicAttackTargetType;

}

// possibly SO to make it spawn itself correctly without doing IFs everywhere
public enum MagicAttackTargetType 
{ 
    Single, 
    AOE 
};
