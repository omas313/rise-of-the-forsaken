using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMenu : MonoBehaviour
{
    public bool IsActiveMenu => _isActiveMenu;

    [SerializeField] UIMenuItem _menuItemPrefab;
    [SerializeField] Transform _itemsParent;
    [SerializeField] MenuItemDefinition[] _menuItemDefinitions;
    [SerializeField] MenuItemDefinition _unlinkDefinition;

    List<UIMenuItem> _menuItems = new List<UIMenuItem>();
    CanvasGroup _canvasGroup;

    bool _isActiveMenu;
    int _currentItemIndex;

    public void ActivateMenu()
    {
        Show();
        _isActiveMenu = true;
        StartCoroutine(PlaceCursorAtFirstPosition());
    }

    void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        Hide();
    }

    void Start()
    {
        FindObjectOfType<BattleController>().PlayerPartyUpdated += OnPlayerPartyUpdated;    
        BattleEvents.EnemyTargetSelected += OnEnemyTargetSelected;
    }

    void OnEnemyTargetSelected(Enemy obj) => Hide();

    void Hide() => _canvasGroup.alpha = 0f;
    
    void Show() => _canvasGroup.alpha = 1f;

    void OnPlayerPartyUpdated(List<PartyMember> partyMembers, PartyMember currentActivePartyMember)
    {
        if (currentActivePartyMember == null)
            return;

        CreateMenu(currentActivePartyMember);
        ActivateMenu();
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


    void CreateMenu(PartyMember currentActivePartyMember)
    {
        foreach (var oldItems in _itemsParent.GetComponentsInChildren<UIMenuItem>())
            Destroy(oldItems.gameObject);
        _menuItems.Clear();

        foreach (var menuItemDefinition in _menuItemDefinitions)
        {
            var menuItem = Instantiate(_menuItemPrefab, _itemsParent.transform.position, Quaternion.identity, _itemsParent)
                .GetComponent<UIMenuItem>();
            menuItem.Init(menuItemDefinition);
            _menuItems.Add(menuItem);
        }

        if (currentActivePartyMember.HasLink)
        {
            var linkMenuItem = Instantiate(_menuItemPrefab, _itemsParent.transform.position, Quaternion.identity, _itemsParent)
                    .GetComponent<UIMenuItem>();
            linkMenuItem.Init(_unlinkDefinition);
            _menuItems.Add(linkMenuItem);
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
