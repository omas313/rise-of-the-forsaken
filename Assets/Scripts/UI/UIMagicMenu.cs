using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMagicMenu : MonoBehaviour
{
    [SerializeField] GameEvent _uiMenuMagicSelected;
    [SerializeField] UIMagicAttackItem _magicAttackItemPrefab;
    [SerializeField] Transform _itemsParent;
    [SerializeField] GameEvent _backEvent;

    List<UIMagicAttackItem> _menuItems = new List<UIMagicAttackItem>();
    BattleController _battleController;
    CanvasGroup _canvasGroup;
    bool _isActiveMenu;
    int _currentItemIndex;

    public void StartMagicSelection()
    {
        Show();
        GoToPreviousItem();
        StartCoroutine(ActivateMenuInSeconds(0.1f));
    }

    IEnumerator ActivateMenuInSeconds(float delay)
    {
        yield return new WaitForSeconds(delay);
        _isActiveMenu = true;
    }

    void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        Hide();    
    }

    void Start()
    {
        _battleController = FindObjectOfType<BattleController>();
        _battleController.PlayerPartyUpdated += OnPlayerPartyUpdated;
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
            menuItem.Init(magicAttack, currentActiveMember.HasManaFor(magicAttack));
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
        else if (Input.GetButtonDown("Back"))
            GoBack();
    }

    void GoBack()
    {
        if (_backEvent != null)
            _backEvent.Raise();

        _isActiveMenu = false;
        Hide();
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
        var magicAttack = _menuItems[_currentItemIndex].MagicAttackDefinition;

        if (!_battleController.CurrentActivePartyMember.HasManaFor(magicAttack))
            return;

        if (_uiMenuMagicSelected != null)
            _uiMenuMagicSelected.Raise();
            
        BattleEvents.InvokeMagicAttackSelected(magicAttack);
        _isActiveMenu = false;
        Hide();
    }
}
