using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMapNodes : MonoBehaviour
{
    [SerializeField] WorldMapNode[] _nodes;
    Dictionary<BattleDataDefinition, WorldMapNode> _battleNodes = new Dictionary<BattleDataDefinition, WorldMapNode>();

    void Awake()
    {
        foreach (var node in _nodes)   
            _battleNodes[node.BattleDefinition] = node;
    }

    public WorldMapNode GetNodeForBattle(BattleDataDefinition definition)
    {
        // Debug.Log($"length of battle nodes when getting: {_battleNodes.Count}, trying to get battle {definition.Order}");

        if (_battleNodes.ContainsKey(definition))
            return _battleNodes[definition];

        Debug.Log("Error: asking world map for battle definition it doesn't have");
        return null;
    }
}
