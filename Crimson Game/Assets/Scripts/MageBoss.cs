public class MageBoss : EnemyBossBase
{
    protected override void Start()
    {
        maxHP = 150;
        defense = 5;
        moveSpeed = 2.5f;
        detectionRange = 9f;
        attackRange = 5f;

        attack1Damage = 12; // magic projectile
        attack2Damage = 25; // AoE burst
        attack3Damage = 40; // high-damage cast

        attack2Cooldown = 6f;
        attack3Cooldown = 10f;

        base.Start();
    }
}
