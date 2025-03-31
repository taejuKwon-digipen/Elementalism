using System.Collections.Generic;

public class CardAbilityManager
{
    private static readonly Dictionary<int, ICardAbility> cardAbilities = new()
    {
        { 6, new ShieldAbility() },
        { 8, new AllEnemyAttackAbility() },
        { 9, new MatchDamageAbility() }
    };

    public static ICardAbility GetAbility(int cardId)
    {
        if (cardAbilities.TryGetValue(cardId, out var ability))
        {
            return ability;
        }
        return new DefaultAbility();
    }
} 