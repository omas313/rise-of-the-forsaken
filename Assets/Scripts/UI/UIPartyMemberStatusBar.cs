using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class UIPartyMemberStatusBar : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _nameText;
    [SerializeField] TextMeshProUGUI _hpText;
    [SerializeField] TextMeshProUGUI _mpText;
    [SerializeField] Image _turnImage;
    [SerializeField] Image _koImage;
    
    Color _defaultBGColor;
    Color _currentTurnBGColor;

    public void Init(string name, string hp, string mp)
    {
        _nameText.SetText(name);
        _hpText.SetText(hp);
        _mpText.SetText(mp);
    }

    public void SetCurrentTurnImageActive(bool active)
    {
        _turnImage.color = active ? _currentTurnBGColor : _defaultBGColor;
    }

    public void SetDeadStatusActive(bool active)
    {
        _koImage.gameObject.SetActive(active);
    }

    void Awake()
    {
        _defaultBGColor = new Color(_turnImage.color.r, _turnImage.color.g, _turnImage.color.b, 0f);
        _currentTurnBGColor = new Color(_turnImage.color.r, _turnImage.color.g, _turnImage.color.b, 1f);
    }
}
