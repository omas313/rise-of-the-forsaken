using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LinkViewer : MonoBehaviour
{
    [SerializeField] LineRenderer _linePrefab;
    PartyMember[] _party;
    List<LinkLine> _linkLines = new List<LinkLine>();

    void Start()
    {
        _party = FindObjectsOfType<PartyMember>();        
        
        BattleEvents.PartyMembersLinked += OnPartyMembersLinked;
        BattleEvents.PartyMembersUnlinked += OnPartyMembersUnlinked;
    }

    void OnPartyMembersLinked(PartyMember member1, PartyMember member2)
    {
        CreateLinkLine(member1, member2);
    }

    void CreateLinkLine(PartyMember member1, PartyMember member2)
    {
        var line = Instantiate(_linePrefab, Vector3.zero, Quaternion.identity, transform);
        
        line.SetPosition(0, member1.transform.position);
        line.SetPosition(1, member2.transform.position);

        line.colorGradient = CreateGradient(member2.InnateElement.Color, member1.InnateElement.Color);
        _linkLines.Add(new LinkLine(member1, member2, line));
    }

    Gradient CreateGradient(Color color1, Color color2)
    {
        var gradient = new Gradient();

        var colorkey = new GradientColorKey[2];
        colorkey[0].color = color1;
        colorkey[0].time = 0f;
        colorkey[1].color = color2;
        colorkey[0].time = 1f;

        var alphaKey = new GradientAlphaKey[2];
        alphaKey[0].alpha = 1f;
        alphaKey[0].time = 0f;
        alphaKey[1].alpha = 1f;
        alphaKey[1].time = 1f;

        gradient.SetKeys(colorkey, alphaKey);

        return gradient;
    }

    void OnPartyMembersUnlinked(PartyMember member1, PartyMember member2)
    {
        // foreach (var line in _linkLines)
        //     Debug.Log($"line: {line.Members[0]} + {line.Members[1]}");

        var linkLine = _linkLines.Where(ll => ll.Members.Contains(member1) && ll.Members.Contains(member2)).FirstOrDefault();
        if (linkLine != null)
        {
            Destroy(linkLine.Line);
            _linkLines.Remove(linkLine);
        }
    }

}
