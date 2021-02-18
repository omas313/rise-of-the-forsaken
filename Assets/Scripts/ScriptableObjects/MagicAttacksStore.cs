using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "MagicAttacksStore.asset", menuName = "Magic Attacks Store")]
public class MagicAttacksStore : ScriptableObject
{
    [SerializeField] MagicAttackDefinition[] _magicAttacks;

    public MagicAttackDefinition[] GetMagicAttackWithElement(Element element)
    {
        return _magicAttacks
            .Where(ma => ma.Elements.Length == 1 && ma.Elements[0] == element)
            .ToArray();
    }

    public MagicAttackDefinition[] GetMagicAttacksWithElements(Element element1, Element element2)
    {
        return _magicAttacks
            .Where(ma => ma.Elements.Contains(element1) && ma.Elements.Contains(element2))
            .ToArray();
    }
}
