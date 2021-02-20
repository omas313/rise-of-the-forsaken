using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStore.asset", menuName = "Enemy Store")]
public class EnemyStore : ScriptableObject
{
    public EnemyDefinition[] _slimes;
    public EnemyDefinition[] _corruptedSouls;

    public EnemyDefinition GetRandomSlimeDefinition() => _slimes[UnityEngine.Random.Range(0, _slimes.Length)];
    public EnemyDefinition GetRandomCorruptedDefinition() => _corruptedSouls[UnityEngine.Random.Range(0, _corruptedSouls.Length)];
    public EnemyDefinition GetRandomEnemy() => UnityEngine.Random.value < 0.5f ? GetRandomSlimeDefinition() : GetRandomCorruptedDefinition();
}