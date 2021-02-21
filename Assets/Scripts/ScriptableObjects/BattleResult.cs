using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BattleResult.asset", menuName = "Battle Result")]
public class BattleResult : ScriptableObject
{
    public BattleDataDefinition BattleDataDefinition => _battleDataDefinition;
    public bool Won => _won;
    
    [SerializeField] BattleDataDefinition _battleDataDefinition;
    [SerializeField] bool _won;


    public void SetResult(BattleDataDefinition definition, bool won)
    {
        _battleDataDefinition = definition;
        _won = won;
    }

    public void Lost() => _won = false;

    public void Clear()
    {
        _battleDataDefinition = null;
        _won = false;
    }
}
