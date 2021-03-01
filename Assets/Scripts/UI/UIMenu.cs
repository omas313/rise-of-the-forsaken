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
    [SerializeField] MenuItemDefinition _linkDefinition;
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

    IEnumerator PlaceCursorAtFirstPosition()
    {
        yield return new WaitForSeconds(0.15f);
        _currentItemIndex = 0;
        MenuCursor.Instance.PlaceAt(_menuItems[_currentItemIndex].CursorPosition);
    }
    
    void Hide() => _canvasGroup.alpha = 0f;

    void Show() => _canvasGroup.alpha = 1f;

    void CreateMenu(List<PartyMember> partyMembers, PartyMember currentActivePartyMember)
    {
        ClearOldItems();

        foreach (var menuItemDefinition in _menuItemDefinitions)
        {
            var menuItem = Instantiate(_menuItemPrefab, _itemsParent.transform.position, Quaternion.identity, _itemsParent)
                .GetComponent<UIMenuItem>();
            menuItem.Init(menuItemDefinition);
            _menuItems.Add(menuItem);
        }

        if (partyMembers.Count > 1)
            CreateLinkMenuItem();

        if (currentActivePartyMember.HasLink)
            CreateUnlinkMenuItem();

        SetMenuSize();
    }

    void ClearOldItems()
    {
        foreach (var oldItems in _itemsParent.GetComponentsInChildren<UIMenuItem>())
            Destroy(oldItems.gameObject);
        _menuItems.Clear();
    }

    void CreateLinkMenuItem()
    {
        var linkItem = Instantiate(_menuItemPrefab, _itemsParent.transform.position, Quaternion.identity, _itemsParent)
                .GetComponent<UIMenuItem>();
        linkItem.Init(_linkDefinition);
        _menuItems.Add(linkItem);
    }

    void CreateUnlinkMenuItem()
    {
        var linkMenuItem = Instantiate(_menuItemPrefab, _itemsParent.transform.position, Quaternion.identity, _itemsParent)
                    .GetComponent<UIMenuItem>();
        linkMenuItem.Init(_unlinkDefinition);
        _menuItems.Add(linkMenuItem);
    }

    void SetMenuSize()
    {
        var height = 50f + _menuItems.Count * 45f;
        var itemsRect = _itemsParent.GetComponent<RectTransform>();
        itemsRect.sizeDelta = new Vector2(itemsRect.sizeDelta.x, height);

        height += 10f;
        GetComponent<RectTransform>().sizeDelta = new Vector2(itemsRect.sizeDelta.x, height);
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

    void OnEnemyTargetSelected(Enemy obj) => Hide();

    void OnPlayerPartyUpdated(List<PartyMember> partyMembers, PartyMember currentActivePartyMember)
    {
        if (currentActivePartyMember == null)
            return;

        CreateMenu(partyMembers, currentActivePartyMember);
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

    void OnDestroy()
    {
        BattleEvents.EnemyTargetSelected -= OnEnemyTargetSelected;
        BattleEvents.PlayerPartyUpdated -= OnPlayerPartyUpdated;    
    }
    
    void Start()
    {
        BattleEvents.PlayerPartyUpdated += OnPlayerPartyUpdated;    
        BattleEvents.EnemyTargetSelected += OnEnemyTargetSelected;
    }

    void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        Hide();
    }
}
