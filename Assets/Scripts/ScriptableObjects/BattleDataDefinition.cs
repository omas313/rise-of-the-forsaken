using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BattleDataDefinition.asset", menuName = "Battle Data Definition")]
public class BattleDataDefinition : ScriptableObject
{
    public List<PartyMember> PlayerParty => _playerParty;
    public List<EnemyDefinition> Enemies => _enemiesDefinitions;
    
    public bool AcidRain => _acidRain;
    public bool Fog => _fog;
    public bool Particles => _particles;
    
    
    [SerializeField] List<PartyMember> _playerParty;
    [SerializeField] List<EnemyDefinition> _enemiesDefinitions;

    [SerializeField] bool _acidRain;
    [SerializeField] bool _fog;
    [SerializeField] bool _particles;
}
