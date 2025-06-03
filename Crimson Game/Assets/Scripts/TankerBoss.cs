public class TankerBoss : EnemyBossBase
{
    protected override void Start()
    {
        maxHP = 300;
        defense = 20;
        moveSpeed = 1.5f;
        detectionRange = 6f;
        attackRange = 1.8f;

        attack1Damage = 10;
        attack2Damage = 15;
        attack3Damage = 25;

        attack2Cooldown = 5f;
        attack3Cooldown = 8f;

        base.Start();
    }
}
