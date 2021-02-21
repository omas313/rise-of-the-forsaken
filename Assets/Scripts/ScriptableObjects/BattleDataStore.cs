using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BattleDataStore.asset", menuName = "Battle Data Store")]
public class BattleDataStore : ScriptableObject
{
    public BattleDataDefinition FinalBattleDefinition => _battleDataDefinitions[_battleDataDefinitions.Length - 1];
    public BattleDataDefinition GetBattle(int number) => _battleDataDefinitions[number - 1];

    [SerializeField] BattleDataDefinition[] _battleDataDefinitions;
}
