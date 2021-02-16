public class AttackBattleAction
{
    private AttackDefinition _attack;

    public AttackBattleAction(AttackDefinition attack)
    {
        _attack = attack;
    }

    public void Attack(BattleParticipant participant)
    {
        participant.ReceiveAttack(_attack);
    }
}


