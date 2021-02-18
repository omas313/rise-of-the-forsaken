using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerPartyTargetPositionManager : MonoBehaviour
{
    [SerializeField] Transform[] _positions;
    [SerializeField] GameObject _marker;

    Dictionary<Transform, PartyMember> _memberPositions;
    List<Transform> _activePositions;

    Camera _camera;
    BattleController _battleController;
    int _currentIndex;
    bool _isControllingCursor;
    bool _isLinking;

    public void InvokeUnlinkEvent()
    {
        BattleEvents.InvokePartyMemberUnlinked(_battleController.CurrentActivePartyMember, _battleController.CurrentActivePartyMember.LinkedPartyMember);
        MenuCursor.Instance.HideCursor();
        _isControllingCursor = false;
    }

    public void StartChoosingLinkTarget()
    {
        _isLinking = true;
        _isControllingCursor = true;
        _currentIndex = 0;
        SetCurrentPosition();
    }

    public void StartChoosingTarget()
    {
        _isLinking = false;
        _isControllingCursor = true;
        _currentIndex = 0;
        SetCurrentPosition();
    }

    void Start()
    {
        _camera = Camera.main;

        _battleController = FindObjectOfType<BattleController>();
        _battleController.BattleStarted += OnBattleStarted;
        _battleController.PlayerPartyUpdated += OnPlayerPartyUpdated;
        _battleController.PartyMemberDied += OnPartyMemberDied;
    }

    void OnBattleStarted(List<PartyMember> playerParty, List<Enemy> enemies)
    {
        _memberPositions = new Dictionary<Transform, PartyMember>();
        _activePositions = new List<Transform>();

        for (var i = 0; i < playerParty.Count; i++)
        {
            var position = _positions[i];
            _memberPositions[position] = playerParty[i];
            _activePositions.Add(position);
            playerParty[i].transform.position = _activePositions[i].position;
        }

        _activePositions = _activePositions.OrderBy(p => p.name).ToList();
    }

    void OnPlayerPartyUpdated(List<PartyMember> playerParty, PartyMember currentActiveMember)
    {
        if (currentActiveMember != null)
            PlaceMarkerAtMember(currentActiveMember);
    }

    void PlaceMarkerAtMember(PartyMember currentActiveMember)
    {
        _marker.transform.position = currentActiveMember.transform.position;
    }

    void OnPartyMemberDied(PartyMember partyMember)
    {
        Transform positionToRemove = null;

        foreach (var pair in _memberPositions)
            if (pair.Value == partyMember)
                positionToRemove = pair.Key;
        
        _activePositions.Remove(positionToRemove);
        _memberPositions.Remove(positionToRemove);
    }

    void Update()
    {
        if (!_isControllingCursor)    
            return;

        if (Input.GetButtonDown("Up"))
            GoToPreviousPosition();
        else if (Input.GetButtonDown("Down"))
            GoToNextPosition();
        else if (Input.GetButtonDown("Confirm"))
            ConfirmCurrentSelection();
    }

    void GoToPreviousPosition()
    {
        _currentIndex = Mathf.Max(_currentIndex - 1, 0);        
        SetCurrentPosition();
    }

    void GoToNextPosition()
    {
        _currentIndex = Mathf.Min(_currentIndex + 1, _activePositions.Count - 1);              
        SetCurrentPosition();
    }

    void ConfirmCurrentSelection()
    {
        var selectedMember = _memberPositions[_activePositions[_currentIndex]];
        if (_isLinking && CantLinkTo(selectedMember))
            return;

        MenuCursor.Instance.HideCursor();
        _isControllingCursor = false;

        if (_isLinking)
        {
            InvokeRemovalOfOtherLinks(selectedMember);
            BattleEvents.InvokePartyMembersLinked(selectedMember, _battleController.CurrentActivePartyMember);
        }
        else
            BattleEvents.InvokePartyMemberSelected(selectedMember);
    }

    private void InvokeRemovalOfOtherLinks(PartyMember selectedMember)
    {
        if (selectedMember.HasLink)
            BattleEvents.InvokePartyMemberUnlinked(selectedMember, selectedMember.LinkedPartyMember);
        if (_battleController.CurrentActivePartyMember.HasLink)
            BattleEvents.InvokePartyMemberUnlinked(_battleController.CurrentActivePartyMember, _battleController.CurrentActivePartyMember.LinkedPartyMember);
    }

    bool CantLinkTo(PartyMember selectedMember) => 
        _battleController.IsCurrentActivePartyMember(selectedMember)
        || (selectedMember.HasLink && _battleController.IsCurrentActivePartyMember(selectedMember.LinkedPartyMember));

    void SetCurrentPosition()
    {
        var position = _activePositions[_currentIndex].position;
        MenuCursor.Instance.PlaceAt(_camera.WorldToScreenPoint(position));
    }
}
