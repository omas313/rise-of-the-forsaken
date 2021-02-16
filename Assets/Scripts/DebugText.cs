using UnityEngine;
using TMPro;

public static class DebugText
{
    public static TextMeshProUGUI AttackDebugText = GameObject.FindGameObjectWithTag("AttackDebugText").GetComponent<TextMeshProUGUI>();

    public static void AddBattleAttackLine(string line)
    {
        AttackDebugText.SetText(AttackDebugText.text + $"\n{line}");
    }
}
