using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LinkBonusFloatingTextSpawner : MonoBehaviour
{
    [SerializeField] FloatingText _damageTextPrefab;

    FloatingText[] _texts;

    void Awake()
    {
        _texts = GetComponentsInChildren<FloatingText>();
        BattleEvents.PartyMembersLinked += OnPartyMembersLinked;
        BattleEvents.PartyMembersUnlinked += OnPartyMembersUnlinked;
        FindObjectOfType<BattleController>().BattleEnded += OnBattleEnded;
    }

    private void OnBattleEnded()
    {
        BattleEvents.PartyMembersLinked -= OnPartyMembersLinked;
        BattleEvents.PartyMembersUnlinked -= OnPartyMembersUnlinked;
        FindObjectOfType<BattleController>().BattleEnded -= OnBattleEnded;
    }

    private void OnPartyMembersUnlinked(PartyMember member1, PartyMember member2)
    {
        _texts[2].Play("unlinked", member1.transform.position);
        _texts[3].Play("unlinked", member2.transform.position);
    }

    void OnPartyMembersLinked(PartyMember member1, PartyMember member2)
    {
        _texts[0].Play("Linked\nHP+\nMP+", member1.transform.position);
        _texts[1].Play("Linked\nHP+\nMP+", member2.transform.position);
    }
}
