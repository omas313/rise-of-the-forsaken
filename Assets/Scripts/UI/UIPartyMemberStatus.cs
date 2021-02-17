using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPartyMemberStatus : MonoBehaviour
{
    UIPartyMemberStatusBar[] _statusBars;

    void Awake()
    {
        _statusBars = GetComponentsInChildren<UIPartyMemberStatusBar>();
    }

    void Start()
    {
        var battleController = FindObjectOfType<BattleController>();
        battleController.PlayerPartyUpdated += OnPlayerPartyUpdated;
    }

    void OnPlayerPartyUpdated(List<PartyMember> party, PartyMember activeTurnPartyMember)
    {
        for (var i = 0; i < _statusBars.Length; i++)
        {
            var name = party[i].Name;
            var hp = party[i].CharacterStats.CurrentHP.ToString();
            var mp = party[i].CharacterStats.CurrentMP.ToString();
            _statusBars[i].Init(name, hp, mp);

            _statusBars[i].SetCurrentTurnImageActive(party[i] == activeTurnPartyMember);
            _statusBars[i].SetDeadStatusActive(party[i].IsDead);
        }
    }
}
