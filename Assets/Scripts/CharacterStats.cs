using System;

public class CharacterStats
{
    // maybe SO later
    public int BaseSpeed { get; set; }
    public int CurrentSpeed { get; set; }

    public int BaseHP { get; set; }
    public int CurrentHP { get; set; }

    public int BaseMP { get; set; }
    public int CurrentMP { get; set; }

    public void ReduceCurrentHP(int amount) => CurrentHP = Math.Max(0, CurrentHP - amount);
}

