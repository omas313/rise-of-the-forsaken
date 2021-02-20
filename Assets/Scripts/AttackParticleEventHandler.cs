using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackParticleEventHandler : MonoBehaviour
{
    // [SerializeField] bool _manualTiming;
    // [SerializeField] float _timeToImpact;
    // [SerializeField] float _timeToImpact;

    public bool HasMadeImpact => _hasMadeImpact;
    public bool HasFinished => _hasFinished;

    bool _hasMadeImpact;
    bool _hasFinished;

    private void OnParticleSystemStopped()
    {
        _hasFinished = true;
        Destroy(gameObject, 3f);
    }

    private void OnParticleCollision(GameObject other)
    {
        _hasMadeImpact = true;
        _hasFinished = true;
    }
}
