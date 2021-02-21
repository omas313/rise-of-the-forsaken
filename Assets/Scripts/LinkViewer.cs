using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LinkViewer : MonoBehaviour
{
    [SerializeField] LineRenderer _linePrefab;
    [SerializeField] ParticleSystem _particlesPrefab;
    PartyMember[] _party;
    List<LinkLine> _linkLines = new List<LinkLine>();

    void Start()
    {
        _party = FindObjectsOfType<PartyMember>();        
        
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
    void OnPartyMembersLinked(PartyMember member1, PartyMember member2)
    {
        CreateLinkLine(member1, member2);
    }

    void CreateLinkLine(PartyMember member1, PartyMember member2)
    {
        var midPoint = member1.transform.position + (member2.transform.position - member1.transform.position) * 0.5f;
        var line = Instantiate(_linePrefab, Vector3.zero, Quaternion.identity, transform);
        var particles = Instantiate(_particlesPrefab, midPoint, Quaternion.identity, line.transform);
        
        var directionVector = (member2.transform.position - member1.transform.position).normalized;

        line.SetPosition(0, member1.transform.position + directionVector * 0.3f);
        line.SetPosition(1, member2.transform.position - directionVector * 0.3f);

        var gradient = CreateGradient(member1.Element.Color, member2.Element.Color);

        var main = particles.main;
        var minMaxGradient = new ParticleSystem.MinMaxGradient(gradient);
        minMaxGradient.mode = ParticleSystemGradientMode.RandomColor;
        main.startColor = minMaxGradient;

        line.colorGradient = gradient;

        _linkLines.Add(new LinkLine(member1, member2, line, particles));
    }

    Gradient CreateGradient(Color color1, Color color2)
    {
        var gradient = new Gradient();

        var colorkey = new GradientColorKey[2];
        colorkey[0].color = color1;
        colorkey[0].time = 0.45f;
        colorkey[1].color = color2;
        colorkey[1].time = 0.55f;

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
        var linkLine = _linkLines.Where(ll => ll.Members.Contains(member1) && ll.Members.Contains(member2)).FirstOrDefault();
        if (linkLine != null)
        {
            Destroy(linkLine.Line);
            Destroy(linkLine.Particles);
            _linkLines.Remove(linkLine);
        }
    }

}
