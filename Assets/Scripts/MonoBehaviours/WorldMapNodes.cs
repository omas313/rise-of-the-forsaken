using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMapNodes : MonoBehaviour
{
    [SerializeField] WorldMapNode[] _nodes;

    Dictionary<BattleDataDefinition, WorldMapNode> _battleNodes = new Dictionary<BattleDataDefinition, WorldMapNode>();

    void Start()
    {
        foreach (var node in _nodes)   
            _battleNodes[node.BattleDefinition] = node;
    }

    public WorldMapNode GetNodeForBattle(BattleDataDefinition definition)
    {
        if (_battleNodes.ContainsKey(definition))
            return _battleNodes[definition];

        Debug.Log("Error: asking world map for battle definition it doesn't have");
        return null;
    }
}
