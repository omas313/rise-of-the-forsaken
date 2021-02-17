using UnityEngine;

[CreateAssetMenu(fileName = "MenuItem", menuName = "Menu Item")]
public class MenuItemDefinition : ScriptableObject
{
    public string Label => _label;
    public int MPCost => _mpCost;
    public bool HasMPCost => MPCost > 0;

    [SerializeField] string _label;
    [SerializeField] int _mpCost;
    [SerializeField] GameEvent _eventToRaise;

    public void InvokeEvent()
    {
        if (_eventToRaise == null)
        {
            Debug.Log("Trying to invoke an event attached to this MenuItemDefinition but it is null.");
            return;
        }

        _eventToRaise.Raise();            
    }
}
