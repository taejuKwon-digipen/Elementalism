using UnityEngine;

public class AllEnemyAttackAbility : ICardAbility
{
    public void ExecuteAbility(Player player, int damage, int oraBlockCount)
    {
        if (oraBlockCount > 0)
        {
            var enemies = Object.FindObjectsOfType<Enemy>();
            foreach (var enemy in enemies)
            {
                enemy.Hit(player, player.baseEntity.Type, damage);
            }
        }
        else
        {
            player.AttackWithDamage(damage);
        }
    }
}

public class ShieldAbility : ICardAbility
{
    public void ExecuteAbility(Player player, int damage, int oraBlockCount)
    {
        player.AddShield(damage);
        player.AttackWithDamage(damage);
    }
}

public class MatchDamageAbility : ICardAbility
{
    public void ExecuteAbility(Player player, int damage, int matchCount)
    {
        player.AttackWithDamage(damage + matchCount);
    }
}

public class DefaultAbility : ICardAbility
{
    public void ExecuteAbility(Player player, int damage, int oraBlockCount)
    {
        player.AttackWithDamage(damage);
    }
} 