using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPartyMemberStatus : MonoBehaviour
{
    [SerializeField] UIPartyMemberStatusBar[] _bars;

    [SerializeField] RectTransform _barsParent;
    Dictionary<PartyMember, UIPartyMemberStatusBar> _playerBars;

    void Start()
    {
        var battleController = FindObjectOfType<BattleController>();
        battleController.PlayerPartyUpdated += OnPlayerPartyUpdated;
        battleController.BattleEnded += OnBattleEnded;

        BattleEvents.RequestedPartyMembersLink += OnPartyMembersLinked;
        BattleEvents.RequestedPartyMembersUnlink += OnPartyMembersUnlinked;
    }

    private void OnBattleEnded()
    {
        var battleController = FindObjectOfType<BattleController>();
        battleController.PlayerPartyUpdated -= OnPlayerPartyUpdated;
        battleController.BattleEnded -= OnBattleEnded;

        BattleEvents.RequestedPartyMembersLink -= OnPartyMembersLinked;
        BattleEvents.RequestedPartyMembersUnlink -= OnPartyMembersUnlinked;
    }

    void OnPartyMembersLinked(PartyMember member1, PartyMember member2)
    {
        _playerBars[member1].SetLinkedStatus(member1.Element.Color, member2.Element.Color);
        _playerBars[member2].SetLinkedStatus(member1.Element.Color, member2.Element.Color);
    }

    void OnPartyMembersUnlinked(PartyMember member1, PartyMember member2)
    {
        _playerBars[member1].HideLinkedStatus();
        _playerBars[member2].HideLinkedStatus();
    }

    void OnPlayerPartyUpdated(List<PartyMember> party, PartyMember activeTurnPartyMember)
    {
        if (_playerBars == null)
            Init(party);

        foreach (var pair in _playerBars)
        {
            var partyMember = pair.Key;
            var bar = pair.Value;

            var name = partyMember.Name;
            var hp = partyMember.HasLink ? partyMember.PartyMemberStats.LinkedHP.ToString() : partyMember.CharacterStats.CurrentHP.ToString();
            var mp = partyMember.HasLink ? partyMember.PartyMemberStats.LinkedMP.ToString() : partyMember.CharacterStats.CurrentMP.ToString();
            
            bar.Init(name, hp, mp);            
            bar.SetCurrentTurnImageActive(partyMember == activeTurnPartyMember);
            bar.SetDeadStatusActive(partyMember.IsDead);
        }

    }

    void Init(List<PartyMember> party)
    {
        _playerBars = new Dictionary<PartyMember, UIPartyMemberStatusBar>();

        for (var i = 0; i < party.Count; i++)
        {
            _playerBars[party[i]] = _bars[i];
            _bars[i].gameObject.SetActive(true);
        }

        

        var height = 50f + party.Count * 45f;
        _barsParent.sizeDelta = new Vector2(_barsParent.sizeDelta.x, height);

        height += 10f;
        GetComponent<RectTransform>().sizeDelta = new Vector2(_barsParent.sizeDelta.x, height);
    }
}
