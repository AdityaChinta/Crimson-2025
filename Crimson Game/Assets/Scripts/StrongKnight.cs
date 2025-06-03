public class StrongKnight : EnemyBossBase
{
    protected override void Start()
    {
        maxHP = 250;
        defense = 15;
        moveSpeed = 2.2f;
        detectionRange = 7f;
        attackRange = 1.6f;

        attack1Damage = 18;
        attack2Damage = 28;
        attack3Damage = 35;

        attack2Cooldown = 5f;
        attack3Cooldown = 7f;

        base.Start();
    }
}
