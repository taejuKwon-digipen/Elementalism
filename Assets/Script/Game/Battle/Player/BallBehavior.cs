using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBehavior : MonoBehaviour
{
    public float speed = 10;
    public GameObject focusedEnemy;
    public Entity player;
    public int damage;
    public GameObject hitEffect;

    public ParticleSystem effects;
    public static int activeAttackAnimations = 0; // 활성화된 공격 애니메이션 수 추적
    // Start is called before the first frame update
    void Start()
    {
        activeAttackAnimations++; // 애니메이션 시작 시 카운트 증가
        var enemyPosition = focusedEnemy.GetComponent<RectTransform>().position;
        transform.position = player.transform.position;
        hitEffect = Instantiate(hitEffect, enemyPosition, Quaternion.identity, this.transform.parent);
        effects = hitEffect.GetComponent<ParticleSystem>();
    }

    private void FixedUpdate() {
        //var currentPosition = transform.position;

        if (focusedEnemy == null)
        {
            activeAttackAnimations--; // 적이 사라지면 애니메이션 카운트 감소
            Destroy(this.gameObject);
            return;
        }

        var enemyPosition = focusedEnemy.GetComponent<RectTransform>().position;
        var entity = focusedEnemy.GetComponentInChildren<Entity>();

        transform.position = Vector3.MoveTowards(transform.position, enemyPosition, speed * Time.fixedDeltaTime);

        // 목표에 도달했는지 확인
        if (Vector3.Distance(transform.position, enemyPosition) <= 0.1f)
        {
            int previousHP = entity.HP;
            entity.Hit(player, player.baseEntity.Type, damage);
            effects.Play();
            // 몬스터가 죽었는지 확인
            if (previousHP > 0 && entity.HP <= 0)
            {
                Player playerComponent = player as Player;
                if (playerComponent != null)
                {
                    playerComponent.OnMonsterDefeated();
                }
            }
            
            activeAttackAnimations--; // 애니메이션 완료 시 카운트 감소
            Destroy(this.gameObject);
        }
    }
}
