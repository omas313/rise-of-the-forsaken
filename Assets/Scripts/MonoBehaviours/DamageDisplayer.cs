using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DamageDisplayer : MonoBehaviour
{
    [SerializeField] FloatingText _damageTextPrefab;

    FloatingText[] _texts;

    void OnDamageReceived(int damage, Vector3 position)
    {
        var text = GetInactiveText();
        text.Play(damage.ToString(), position);
    }

    FloatingText GetInactiveText()
    {
        var text = _texts.Where(t => t != null && t.IsAvailable).FirstOrDefault();
        if (text != null)
            return text;

        Debug.Log("Ran out of floating texts");

        var newText = Instantiate(_damageTextPrefab, Vector3.zero, Quaternion.identity, transform);
        _texts = GetComponentsInChildren<FloatingText>();
        return newText;
    }

    void OnDestroy()
    {
        BattleEvents.DamageReceived -= OnDamageReceived;        
    }

    void Awake()
    {
        _texts = GetComponentsInChildren<FloatingText>();

        BattleEvents.DamageReceived += OnDamageReceived;        
    }
}
