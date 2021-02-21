using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RandomTextLineStore.asset", menuName = "Random Text Line Store")]
public class RandomTextLineStore : ScriptableObject
{
    public string GetRandomLine() => _textLines[UnityEngine.Random.Range(0, _textLines.Count)];

    [SerializeField] [TextArea(3, 10)] List<string> _textLines;
}
