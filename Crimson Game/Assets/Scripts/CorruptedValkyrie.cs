public class CorruptedValkyrie : EnemyBossBase
{
    protected override void Start()
    {
        maxHP = 200;
        defense = 10;
        moveSpeed = 3f;
        detectionRange = 7f;
        attackRange = 2f;

        attack1Damage = 15;
        attack2Damage = 20;
        attack3Damage = 30;

        attack2Cooldown = 4f;
        attack3Cooldown = 6f;

        base.Start();
    }

    protected override void Die()
    {
        // Optional: trigger curse effects on death
        base.Die();
    }
}
