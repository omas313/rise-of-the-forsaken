using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DamageDisplayer : MonoBehaviour
{
    [SerializeField] FloatingText _damageTextPrefab;

    Camera _camera;

    FloatingText[] _texts;

    void Awake()
    {
        _texts = GetComponentsInChildren<FloatingText>();
        _camera = Camera.main;
        BattleEvents.DamageReceived += OnDamageReceived;        
    }

    void OnDamageReceived(int damage, Vector3 position)
    {
        var screenPosition = _camera.WorldToScreenPoint(position);
        var text = GetInactiveText();
        text.Play(damage.ToString(), screenPosition);
    }

    FloatingText GetInactiveText()
    {
        var text = _texts.Where(t => t.IsAvailable).FirstOrDefault();
        if (text != null)
            return text;

        Debug.Log("Ran out of floating texts");

        var newText = Instantiate(_damageTextPrefab, Vector3.zero, Quaternion.identity, transform);
        _texts = GetComponentsInChildren<FloatingText>();
        return newText;
    }

}
