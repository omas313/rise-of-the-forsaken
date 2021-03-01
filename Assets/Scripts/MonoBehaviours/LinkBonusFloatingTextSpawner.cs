using UnityEngine;

public class LinkBonusFloatingTextSpawner : MonoBehaviour
{
    [SerializeField] FloatingText _damageTextPrefab;

    FloatingText[] _texts;

    void OnPartyMembersUnlinked(PartyMember member1, PartyMember member2)
    {
        _texts[2].Play("Unlinked", member1.transform.position);
        _texts[3].Play("Unlinked", member2.transform.position);
    }

    void OnPartyMembersLinked(PartyMember member1, PartyMember member2)
    {
        _texts[0].Play("Linked\nHP+\nMP+", member1.transform.position);
        _texts[1].Play("Linked\nHP+\nMP+", member2.transform.position);
    }

    void OnDestroy()
    {
        BattleEvents.PartyMembersLinked -= OnPartyMembersLinked;
        BattleEvents.PartyMembersUnlinked -= OnPartyMembersUnlinked;
    }
    
    void Awake()
    {
        _texts = GetComponentsInChildren<FloatingText>();
        BattleEvents.PartyMembersLinked += OnPartyMembersLinked;
        BattleEvents.PartyMembersUnlinked += OnPartyMembersUnlinked;
    }
}
