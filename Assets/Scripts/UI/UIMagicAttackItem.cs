using TMPro;
using UnityEngine;

public class UIMagicAttackItem : MonoBehaviour
{
    public MagicAttackDefinition MagicAttackDefinition { get; private set; }
    public Vector3 CursorPosition => _cursorPosition.position;

    [SerializeField] TextMeshProUGUI _labelText;
    [SerializeField] GameObject _mpCostGroup;
    [SerializeField] TextMeshProUGUI _mpValue;
    [SerializeField] Transform _cursorPosition;

    public void Init(MagicAttackDefinition magicAttackDefinition) 
    {
        MagicAttackDefinition = magicAttackDefinition;
        _labelText.SetText(magicAttackDefinition.Name);
        _mpValue.SetText(magicAttackDefinition.MPCost.ToString());
    }
}