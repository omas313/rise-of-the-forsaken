using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMapNode : MonoBehaviour
{
    public bool HasBeenVisited { get; private set; }

    [SerializeField] WorldMapNode _previousNode;
    [SerializeField] WorldMapNode _nextNode;
    [SerializeField] LineRenderer _line;
    [SerializeField] BattleDataDefinition _battleDataDefinition;


    void Start()
    {
        DrawLineToNext();
    }

    void DrawLineToNext()
    {
        if (_nextNode == null)
        {
            _line.gameObject.SetActive(false);
            return;
        }

        _line.gameObject.SetActive(true);
        _line.SetPosition(0, Vector2.zero);
        _line.SetPosition(1, _nextNode.transform.position - transform.position);
    }
}
