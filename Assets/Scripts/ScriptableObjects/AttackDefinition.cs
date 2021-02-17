using UnityEngine;

[CreateAssetMenu(fileName = "AttackDefinition.asset", menuName = "Attack Definition")]
public class AttackDefinition : ScriptableObject
{
    public int Damage => _damage;
    public string Name => _name;

    [SerializeField] int _damage;
    [SerializeField] string _name;
}
