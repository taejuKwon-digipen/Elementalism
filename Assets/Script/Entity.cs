using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class Entity : MonoBehaviour
{
    public EntityBase baseEntity;
    public int HP;
    public int MP;

    protected virtual void Start() {
        var animator = this.GetComponent <RuntimeAnimatorController>();
        var image = this.GetComponent<Image>();

        HP = baseEntity.MaxHP;
        MP = baseEntity.MaxMP;
        animator = baseEntity.Animator ? baseEntity.Animator : null;
        image.sprite = baseEntity.Sprite ? baseEntity.Sprite : null;
    }

    /** <summary>
      * Allows to deal damages to the entity with the statistic calculation
      * </summary>
      * <param name="attack">Attack statistic of the other entity </param>
      */
    public virtual int Hit(Entity attacker, EntityType attackType, int damageAmount)
    {
        this.HP -= damageAmount;
        if (this.HP <= 0)
        {
            this.HP = 0;
        }
        return damageAmount;
    }
}
