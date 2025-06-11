using UnityEngine;
using TMPro;

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
    private int CardID;
    public DefaultAbility(int i) { CardID = i; }
    public void ExecuteAbility(Player player, int damage, int oraBlockCount)
    {
        if (CardID == 1) { player.AttackWithDamage(damage, player.FireBall); }
        else if (CardID == 2) { player.AttackWithDamage(damage, player.WaterBall); }
        else if (CardID == 3) { player.AttackWithDamage(damage, player.FireBall); }
        else if (CardID == 4) { player.AttackWithDamage(damage, player.AirBall); }
        else { player.AttackWithDamage(damage); }
    }
}

// 새로운 카드 능력: 적을 빙결시키고 공격
public class FreezeAndAttackAbility : ICardAbility
{
    public void ExecuteAbility(Player player, int damage, int oraBlockCount)
    {
        // 1. 적 빙결 시도 (1턴 동안)
        player.ApplyFreezeToFocusedEnemy(1);

        // 2. 기본 공격 수행
        // oraBlockCount는 이 능력에서 직접 사용되지는 않지만, Player의 AttackWithDamage가 이를 처리할 수 있음
        // 또는 oraBlockCount > 0일 때 추가 효과를 줄 수도 있음 (예: 빙결 턴 증가)
        player.AttackWithDamage(damage); 
    }
}

// 새로운 카드 능력: 적에게 화상을 입히고 공격
public class BurnAndAttackAbility : ICardAbility
{
    public void ExecuteAbility(Player player, int damage, int oraBlockCount)
    {
        // 1. 적에게 공격력(damage)만큼 화상 스택 적용
        player.ApplyBurnToFocusedEnemy(damage);

        // 2. 기본 공격 수행
        player.AttackWithDamage(damage);
    }
} 