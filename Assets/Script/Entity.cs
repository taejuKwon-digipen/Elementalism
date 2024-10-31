using System;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Entity : MonoBehaviour
{
    public EntityBase baseEntity;
    public int HP;
    public int MP;

    protected virtual void Start() {
        var animator = this.GetComponent<Animator>();
        var image = this.GetComponent<Image>();

        HP = baseEntity.MaxHP;
        MP = baseEntity.MaxMP;
        animator.runtimeAnimatorController = baseEntity.Animator ? baseEntity.Animator : null;
        image.sprite = baseEntity.Sprite ? baseEntity.Sprite : null;
    }

    /** <summary>
      * Allows to deal damages to the entity with the statistic calculation
      * </summary>
      * <param name="attack">Attack statistic of the other entity </param>
      */
    public virtual int Hit(Entity attacker, EntityType attackType)
    {
        int damages = DamageManager.ComputeDamages(attacker, this, attackType);
        this.HP -=  damages;
        return damages;
    }
}
