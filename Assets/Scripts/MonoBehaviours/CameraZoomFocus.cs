using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraZoomFocus : MonoBehaviour
{
    [SerializeField] float _zoomSpeed = 5f;
    [SerializeField] float _defaultOrthographicSize = 8f;
    Transform _defaultFocusTransform;
    CinemachineVirtualCamera _camera;

    void Start()
    {
        _defaultFocusTransform = GameManager.Instance.transform;
        _camera = GetComponent<CinemachineVirtualCamera>();

        BattleEvents.PartyMemberIsCasting += OnPartyCasting;
        BattleEvents.PartyMemberFinishedCasting += OnPartyMemberFinishedCasting;

        BattleEvents.ParticipantIsDying += OnParticipantDying;
        BattleEvents.ParticipantIsDead += OnParticipantDead;
        FindObjectOfType<BattleController>().BattleEnded += OnBattleEnded;
    }

    private void OnBattleEnded()
    {
        BattleEvents.PartyMemberIsCasting -= OnPartyCasting;
        BattleEvents.PartyMemberFinishedCasting -= OnPartyMemberFinishedCasting;

        BattleEvents.ParticipantIsDying -= OnParticipantDying;
        BattleEvents.ParticipantIsDead -= OnParticipantDead;
        FindObjectOfType<BattleController>().BattleEnded -= OnBattleEnded;
    }

    void OnParticipantDying(BattleParticipant participant)
    {
        _camera.Follow = participant.transform;
        // StartCoroutine(Zoom(7.5f));
    }

    void OnParticipantDead(BattleParticipant participant)
    {
        _camera.Follow = _defaultFocusTransform;
        // StartCoroutine(Zoom(_defaultOrthographicSize));
    }

    void OnPartyCasting(PartyMember partyMember)
    {
        _camera.Follow = partyMember.transform;
        // StartCoroutine(Zoom(7.5f));
    }

    void OnPartyMemberFinishedCasting(PartyMember partyMember)
    {
        _camera.Follow = _defaultFocusTransform;
        // StartCoroutine(Zoom(_defaultOrthographicSize));
    }

    IEnumerator Zoom(float zoom)
    {
        while (Mathf.Abs(_camera.m_Lens.OrthographicSize - zoom) > Mathf.Epsilon)
        {
            _camera.m_Lens.OrthographicSize += Time.deltaTime * _zoomSpeed * (zoom < _camera.m_Lens.OrthographicSize ? -1 : 1);
            yield return null;
        }
    }
}
