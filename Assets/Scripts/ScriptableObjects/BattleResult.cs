using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BattleResult.asset", menuName = "Battle Result")]
public class BattleResult : ScriptableObject
{
    public BattleDataDefinition BattleDataDefinition { get; private set; }
    public bool Won { get; private set; }

    public void SetResult(BattleDataDefinition definition, bool won)
    {
        BattleDataDefinition = default;
        Won = won;
    }
}
