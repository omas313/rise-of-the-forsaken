using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMenu : MonoBehaviour
{
    public bool IsActiveMenu => _isActiveMenu;

    [SerializeField] UIMenuItem _menuItemPrefab;
    [SerializeField] Transform _menuItemsParent;
    [SerializeField] MenuItemDefinition[] _menuItemDefinitions;

    List<UIMenuItem> _menuItems = new List<UIMenuItem>();

    bool _isActiveMenu;
    int _currentItemIndex;


    public void Init()
    {
        CreateMenu();
    }

    void Awake()
    {
        Init();
        FindObjectOfType<BattleController>().PlayerPartyUpdated += OnPlayerPartyUpdated;
    }

    void OnPlayerPartyUpdated(List<PartyMember> partyMembers, PartyMember currentActivePartyMember)
    {
        if (currentActivePartyMember == null)
            return;

        _isActiveMenu = true;
        StartCoroutine(PlaceCursorAtFirstPosition());
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


    void CreateMenu()
    {
        foreach (var menuItemDefinition in _menuItemDefinitions)
        {
            var menuItem = Instantiate(_menuItemPrefab, _menuItemsParent.transform.position, Quaternion.identity, _menuItemsParent)
                .GetComponent<UIMenuItem>();
            menuItem.Init(menuItemDefinition);
            _menuItems.Add(menuItem);
        }
    }

    IEnumerator PlaceCursorAtFirstPosition()
    {
        yield return new WaitForSeconds(0.15f);
        _currentItemIndex = 0;
        MenuCursor.Instance.PlaceAt(_menuItems[_currentItemIndex].CursorPosition);
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
        _menuItems[_currentItemIndex].PerformAction();
        _isActiveMenu = false;
    }
}
