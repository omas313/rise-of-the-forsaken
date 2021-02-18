using UnityEngine;

[CreateAssetMenu(fileName = "MagicAttackDefinition.asset", menuName = "Magic Attack Definition")]
public class MagicAttackDefinition : AttackDefinition
{
    public int MPCost => _mpCost;
    public Element[] Elements => _elements;

    [SerializeField] int _mpCost;
    [SerializeField] Element[] _elements;
}
