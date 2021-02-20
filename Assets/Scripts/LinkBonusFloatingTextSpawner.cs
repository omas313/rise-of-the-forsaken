using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LinkBonusFloatingTextSpawner : MonoBehaviour
{
    [SerializeField] FloatingText _damageTextPrefab;

    Camera _camera;

    FloatingText[] _texts;

    void Awake()
    {
        _texts = GetComponentsInChildren<FloatingText>();
        _camera = Camera.main;

        BattleEvents.PartyMembersLinked += OnPartyMembersLinked;
        BattleEvents.PartyMembersUnlinked += OnPartyMembersUnlinked;
    }

    private void OnPartyMembersUnlinked(PartyMember member1, PartyMember member2)
    {
        var screenPosition1 = _camera.WorldToScreenPoint(member1.transform.position);
        var screenPosition2 = _camera.WorldToScreenPoint(member2.transform.position);

        _texts[0].Play("unlinked", screenPosition1);
        _texts[1].Play("unlinked", screenPosition2);
    }

    void OnPartyMembersLinked(PartyMember member1, PartyMember member2)
    {
        var screenPosition1 = _camera.WorldToScreenPoint(member1.transform.position);
        var screenPosition2 = _camera.WorldToScreenPoint(member2.transform.position);

        _texts[0].Play("Linked\nHP+\nMP+", screenPosition1);
        _texts[1].Play("Linked\nHP+\nMP+", screenPosition2);
    }
}
