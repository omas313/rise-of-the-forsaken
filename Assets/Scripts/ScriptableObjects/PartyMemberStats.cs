using System;

[System.Serializable]
public class PartyMemberStats : CharacterStats
{
    public int LinkedHP => _linkedHP;
    public int LinkedMP => _linkedMP;

    int _linkedHP;
    int _linkedMP;

    public void IncreaseLinkedHP(int amount) => _linkedHP = Math.Min(100, _linkedHP + amount);
    public void ReduceLinkedHP(int amount) => _linkedHP = Math.Max(0, LinkedHP - amount);
    public void SetLinkedHP(int amount) => _linkedHP = amount;

    public void IncreaseLinkedMP(int amount) => _linkedMP = Math.Min(100, _linkedMP + amount);
    public void ReduceLinkedMP(int amount) => _linkedMP = Math.Max(0, LinkedMP - amount);
    public void SetLinkedMP(int amount) => _linkedMP = amount;
}
