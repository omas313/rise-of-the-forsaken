using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BattleDataDefinition.asset", menuName = "Battle Data Definition")]
public class BattleDataDefinition : ScriptableObject
{
    public List<PartyMember> PlayerParty => _playerParty;
    public List<EnemyDefinition> Enemies => _enemiesDefinitions;
    public int Order => _order;
    
    public bool AcidRain => _acidRain;
    public bool Fog => _fog;
    public bool BackFog => _backFog;
    public bool Particles => _particles;


    [SerializeField] List<PartyMember> _playerParty;
    [SerializeField] List<EnemyDefinition> _enemiesDefinitions;
    [SerializeField] int _order;

    [SerializeField] bool _acidRain;
    [SerializeField] bool _fog;
    [SerializeField] bool _backFog;
    [SerializeField] bool _particles;
}
