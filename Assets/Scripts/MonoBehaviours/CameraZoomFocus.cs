using UnityEngine;
using Cinemachine;

public class CameraZoomFocus : MonoBehaviour
{
    Transform _defaultFocusTransform;
    CinemachineVirtualCamera _camera;

    void OnParticipantDying(BattleParticipant participant)
    {
        _camera.Follow = participant.transform;
    }

    void OnParticipantDead(BattleParticipant participant)
    {
        _camera.Follow = _defaultFocusTransform;
    }

    void OnPartyCasting(PartyMember partyMember)
    {
        _camera.Follow = partyMember.transform;
    }

    void OnPartyMemberFinishedCasting(PartyMember partyMember)
    {
        _camera.Follow = _defaultFocusTransform;
    }

    void OnDestroy()
    {
        BattleEvents.PartyMemberIsCasting -= OnPartyCasting;
        BattleEvents.PartyMemberFinishedCasting -= OnPartyMemberFinishedCasting;

        BattleEvents.ParticipantIsDying -= OnParticipantDying;
        BattleEvents.ParticipantIsDead -= OnParticipantDead;
    }

    void Start()
    {
        _defaultFocusTransform = GameManager.Instance.transform;
        _camera = GetComponent<CinemachineVirtualCamera>();

        BattleEvents.PartyMemberIsCasting += OnPartyCasting;
        BattleEvents.PartyMemberFinishedCasting += OnPartyMemberFinishedCasting;

        BattleEvents.ParticipantIsDying += OnParticipantDying;
        BattleEvents.ParticipantIsDead += OnParticipantDead;
    }
}
