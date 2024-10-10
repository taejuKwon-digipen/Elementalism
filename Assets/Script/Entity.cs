using System;
using UnityEngine;

public enum EntityType
{
    Fire,
    Water,
    Wind,
    Earth,
    Neutral,
}

public class Entity : MonoBehaviour
{
    // Statistics
    [field: SerializeField]
    public int MaxHP { get; set; }

    [field: SerializeField]
    public int HP { get; set; }

    [field: SerializeField]
    public int MaxMP { get; set; }

    [field: SerializeField]
    public int MP { get; set; }

    [field: SerializeField]
    public int Attack { get; set; }

    [field: SerializeField]
    public int Defense { get; set; }

    [field: SerializeField]
    public int Agility { get; set; }
    
    [field: SerializeField]
    public int Luck { get; set; }

    public EntityType Type { get; set; }

    /** <summary>
      * Allows to deal damages to the entity with the statistic calculation
      * </summary>
      * <param name="attack">Attack statistic of the other entity </param>
      */
    public int Hit(Entity attacker)
    {
        int attack = attacker.Attack;
        attack += (int)Math.Round(attacker.Attack * (UnityEngine.Random.Range(-2.0f, 2.0f) / 10));
        attack -= this.Defense;
        this.HP -= attack;
        if (attack <= 0)
            return attack;

        if (this.HP <= 0)
            Destroy(this.transform.parent.gameObject);
        return attack;
    }
}
