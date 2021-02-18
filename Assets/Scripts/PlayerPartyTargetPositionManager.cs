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

    int _currentIndex;
    Camera _camera;
    bool _isControllingCursor;

    public void StartChoosingTarget()
    {
        _isControllingCursor = true;
        _currentIndex = 0;
        SetCurrentPosition();
    }

    void Start()
    {
        _camera = Camera.main;

        var battleController = FindObjectOfType<BattleController>();
        battleController.BattleStarted += OnBattleStarted;
        battleController.PlayerPartyUpdated += OnPlayerPartyUpdated;
        battleController.PartyMemberDied += OnPartyMemberDied;
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

        // _activePositions = _activePositions.OrderBy(p => p.name).ToList();
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
        MenuCursor.Instance.HideCursor();
        _isControllingCursor = false;
        var selectedMember = _memberPositions[_activePositions[_currentIndex]];
        BattleEvents.InvokePartyMemberSelected(selectedMember);
    }

    void SetCurrentPosition()
    {
        var position = _activePositions[_currentIndex].position;
        MenuCursor.Instance.PlaceAt(_camera.WorldToScreenPoint(position));
    }
}
