using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPartyMemberStatus : MonoBehaviour
{
    [SerializeField] UIPartyMemberStatusBar[] _bars;
    [SerializeField] RectTransform _barsParent;

    Dictionary<PartyMember, UIPartyMemberStatusBar> _partyMembersBars;

    void OnPartyMembersLinked(PartyMember member1, PartyMember member2)
    {
        _partyMembersBars[member1].SetLinkedStatus(member1.Element.Color, member2.Element.Color);
        _partyMembersBars[member2].SetLinkedStatus(member1.Element.Color, member2.Element.Color);
    }

    void OnPartyMembersUnlinked(PartyMember member1, PartyMember member2)
    {
        _partyMembersBars[member1].HideLinkedStatus();
        _partyMembersBars[member2].HideLinkedStatus();
    }

    void OnPlayerPartyUpdated(List<PartyMember> party, PartyMember activeTurnPartyMember)
    {
        if (_partyMembersBars == null)
            Init(party);

        foreach (var pair in _partyMembersBars)
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
        _partyMembersBars = new Dictionary<PartyMember, UIPartyMemberStatusBar>();

        for (var i = 0; i < party.Count; i++)
        {
            _partyMembersBars[party[i]] = _bars[i];
            _bars[i].gameObject.SetActive(true);
        }
        

        var height = 50f + party.Count * 45f;
        _barsParent.sizeDelta = new Vector2(_barsParent.sizeDelta.x, height);

        height += 10f;
        GetComponent<RectTransform>().sizeDelta = new Vector2(_barsParent.sizeDelta.x, height);
    }
    void OnDestroy()
    {
        BattleEvents.PlayerPartyUpdated -= OnPlayerPartyUpdated;
        BattleEvents.PartyMembersLinked -= OnPartyMembersLinked;
        BattleEvents.PartyMembersUnlinked -= OnPartyMembersUnlinked;
    }

    void Start()
    {
        BattleEvents.PlayerPartyUpdated += OnPlayerPartyUpdated;
        BattleEvents.PartyMembersLinked += OnPartyMembersLinked;
        BattleEvents.PartyMembersUnlinked += OnPartyMembersUnlinked;
    }
}
