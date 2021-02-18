using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMagicMenu : MonoBehaviour
{
    [SerializeField] GameEvent _uiMenuMagicSelected;
    [SerializeField] UIMagicAttackItem _magicAttackItemPrefab;
    [SerializeField] Transform _itemsParent;

    List<UIMagicAttackItem> _menuItems = new List<UIMagicAttackItem>();
    CanvasGroup _canvasGroup;
    bool _isActiveMenu;
    int _currentItemIndex;

    public void StartMagicSelection()
    {
        Show();
        _isActiveMenu = true;
        GoToPreviousItem();
    }

    void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        Hide();    
    }

    void Start()
    {
        var battleController = FindObjectOfType<BattleController>();
        battleController.PlayerPartyUpdated += OnPlayerPartyUpdated;
    }

    void Hide() => _canvasGroup.alpha = 0f;
    void Show() => _canvasGroup.alpha = 1f;

    private void OnPlayerPartyUpdated(List<PartyMember> partyMembers, PartyMember currentActiveMember)
    {
        if (currentActiveMember == null)
            return;

        foreach (var oldItems in _itemsParent.GetComponentsInChildren<UIMagicAttackItem>())
            Destroy(oldItems.gameObject);
        _menuItems.Clear();

        foreach (var magicAttack in currentActiveMember.MagicAttacks)
        {
            var menuItem = Instantiate(_magicAttackItemPrefab, _itemsParent.transform.position, Quaternion.identity, _itemsParent);
            menuItem.Init(magicAttack);
            _menuItems.Add(menuItem);
        }
    }

    void Update()
    {
        if (!_isActiveMenu)
            return;

        if (Input.GetButtonDown("Up"))
            GoToPreviousItem();
        else if (Input.GetButtonDown("Down"))
            GoToNextItem();
        else if (Input.GetButtonDown("Confirm"))
            ConfirmCurrentSelection();    
    }

    void GoToNextItem()
    {
        _currentItemIndex = Mathf.Min(_currentItemIndex + 1, _menuItems.Count - 1);
        MenuCursor.Instance.PlaceAt(_menuItems[_currentItemIndex].CursorPosition);
    }

    void GoToPreviousItem()
    {
        _currentItemIndex = Mathf.Max(_currentItemIndex - 1, 0);
        MenuCursor.Instance.PlaceAt(_menuItems[_currentItemIndex].CursorPosition);
    }

    void ConfirmCurrentSelection()
    {
        if (_uiMenuMagicSelected != null)
            _uiMenuMagicSelected.Raise();
            
        BattleEvents.InvokeMagicAttackSelected(_menuItems[_currentItemIndex].MagicAttackDefinition);
        _isActiveMenu = false;
        Hide();
    }
}
