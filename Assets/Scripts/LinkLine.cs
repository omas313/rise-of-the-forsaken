using UnityEngine;

public class LinkLine
{
    public PartyMember[] Members { get; set; }
    public LineRenderer Line { get; set; }
    public ParticleSystem Particles { get; set; }
    public LinkLine(PartyMember member1, PartyMember member2, LineRenderer line, ParticleSystem particles)
    {
        Members = new PartyMember[] { member1, member2 };
        Line = line;
        Particles = particles;
    }
}
