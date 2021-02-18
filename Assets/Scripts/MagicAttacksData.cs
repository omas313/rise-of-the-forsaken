using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicAttacksData : MonoBehaviour
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
