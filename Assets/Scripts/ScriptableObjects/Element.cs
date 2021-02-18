using UnityEngine;

[CreateAssetMenu(fileName = "Element.asset", menuName = "Element")]
public class Element : ScriptableObject
{
    public string Name => _name;
    public Color Color => _color;

    [SerializeField] string _name;
    [SerializeField] Color _color;
}
