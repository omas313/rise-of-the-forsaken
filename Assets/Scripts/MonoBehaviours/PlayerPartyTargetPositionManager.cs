using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerPartyTargetPositionManager : MonoBehaviour
{
    [SerializeField] Transform[] _positions;
    [SerializeField] GameObject _marker;
    [SerializeField] GameEvent _backEvent;

    Dictionary<Transform, PartyMember> _memberPositions;
    List<Transform> _activePositions;

    Camera _camera;
    PartyMember _currentActivePartyMember;
    int _currentIndex;
    bool _isActive;
    bool _isLinking;

    public void InvokeUnlinkEvent()
    {
        BattleEvents.InvokeRequestedPartyMembersUnlink(_currentActivePartyMember, _currentActivePartyMember.LinkedPartyMember);
        MenuCursor.Instance.HideCursor();
        _isActive = false;
    }

    public void StartChoosingLinkTarget()
    {
        StartCoroutine(StartSelectionInSeconds(0.1f));
    }

    IEnumerator StartSelectionInSeconds(float delay)
    {
        yield return new WaitForSeconds(delay);
        _isLinking = true;
        _isActive = true;
        _currentIndex = 0;
        SetCurrentPosition();
    }

    void HideMarker()
    {
        var sr = _marker.GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
            sr.enabled = false;
    }

    void PlaceMarkerAtMember(PartyMember currentActiveMember)
    {
        var sr = _marker.GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
            sr.enabled = true;
        _marker.transform.position = currentActiveMember.transform.position;
    }

    void GoBack()
    {
        if (_backEvent != null)
            _backEvent.Raise();

        MenuCursor.Instance.HideCursor();
        _isActive = false;
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
        _isActive = false;

        if (_isLinking)
        {
            InvokeRemovalOfOtherLinks(selectedMember);
            BattleEvents.InvokeRequestedPartyMembersLink(selectedMember, _currentActivePartyMember);
        }
        else
            BattleEvents.InvokePartyMemberSelected(selectedMember);
    }

    void InvokeRemovalOfOtherLinks(PartyMember selectedMember)
    {
        if (selectedMember.HasLink)
            BattleEvents.InvokeRequestedPartyMembersUnlink(selectedMember, selectedMember.LinkedPartyMember);
        if (_currentActivePartyMember.HasLink)
            BattleEvents.InvokeRequestedPartyMembersUnlink(_currentActivePartyMember, _currentActivePartyMember.LinkedPartyMember);
    }

    bool CantLinkTo(PartyMember selectedMember) => _currentActivePartyMember == selectedMember || IsAlreadyLinkedTo(selectedMember);
    bool IsAlreadyLinkedTo(PartyMember selectedMember) => selectedMember.HasLink && selectedMember.LinkedPartyMember == _currentActivePartyMember;

    void SetCurrentPosition()
    {
        var position = _activePositions[_currentIndex].position;
        position = _camera.WorldToScreenPoint(position) - new Vector3(60f, 0f, 0f);
        MenuCursor.Instance.PlaceAt(position);
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
        
    void OnPartyMemberCasting(PartyMember partyMember) => HideMarker();

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
        _currentActivePartyMember = currentActiveMember;
        
        if (currentActiveMember == null)
            HideMarker();
        else
            PlaceMarkerAtMember(currentActiveMember);
    }

    void Update()
    {
        if (!_isActive)    
            return;

        if (Input.GetButtonDown("Up"))
            GoToPreviousPosition();
        else if (Input.GetButtonDown("Down"))
            GoToNextPosition();
        else if (Input.GetButtonDown("Confirm"))
            ConfirmCurrentSelection();    
        else if (Input.GetButtonDown("Back"))
            GoBack();
    }

    private void OnDestroy()
    {
        BattleEvents.BattleStarted -= OnBattleStarted;
        BattleEvents.PlayerPartyUpdated -= OnPlayerPartyUpdated;
        BattleEvents.PartyMemberDied -= OnPartyMemberDied;
        BattleEvents.PartyMemberIsCasting -= OnPartyMemberCasting;
    }

    void Start()
    {
        _camera = Camera.main;
        
        BattleEvents.BattleStarted += OnBattleStarted;
        BattleEvents.PlayerPartyUpdated += OnPlayerPartyUpdated;
        BattleEvents.PartyMemberDied += OnPartyMemberDied;
        BattleEvents.PartyMemberIsCasting += OnPartyMemberCasting;
    }
}
