using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIMenuItem : MonoBehaviour
{
    public MenuItemDefinition MenuItemDefinition { get; private set; }
    public Vector3 CursorPosition => _cursorPosition.position;

    [SerializeField] TextMeshProUGUI _labelText;
    [SerializeField] GameObject _mpCostGroup;
    [SerializeField] TextMeshProUGUI _mpValue;
    [SerializeField] Transform _cursorPosition;

    public void Init(MenuItemDefinition menuItemDefinition) 
    {
        MenuItemDefinition = menuItemDefinition;
        _labelText.SetText(menuItemDefinition.Label);

        if (_mpCostGroup != null)
        {
            _mpCostGroup.SetActive(menuItemDefinition.HasMPCost);    
            if (menuItemDefinition.HasMPCost)
                _mpValue.SetText(menuItemDefinition.MPCost.ToString());
        }
    }

    public void PerformAction()
    {
        MenuItemDefinition.InvokeEvent();
    }
}
