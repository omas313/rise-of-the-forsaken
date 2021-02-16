using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIPartyMemberStatusBar : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _nameText;
    [SerializeField] TextMeshProUGUI _hpText;
    [SerializeField] TextMeshProUGUI _mpText;


    public void Init(string name, string hp, string mp)
    {
        _nameText.SetText(name);
        _hpText.SetText(hp);
        _mpText.SetText(mp);
    }
}
