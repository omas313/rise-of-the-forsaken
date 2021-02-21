using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BattleDataStore.asset", menuName = "Battle Data Store")]
public class BattleDataStore : ScriptableObject
{
    [SerializeField] BattleDataDefinition[] _battleDataDefinitions;

    public BattleDataDefinition GetBattle(int number) => _battleDataDefinitions[number - 1];
}
