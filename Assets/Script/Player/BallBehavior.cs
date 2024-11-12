using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBehavior : MonoBehaviour
{
    public float speed = 10;
    public GameObject focusedEnemy;
    public Entity player;
    public int damage;
    // Start is called before the first frame update
    void Start()
    {
        transform.position = player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate() {
        //var currentPosition = transform.position;

        if (focusedEnemy == null)
            return;

        var enemyPosition = focusedEnemy.GetComponent<RectTransform>().position;
        var entity = focusedEnemy.GetComponentInChildren<Entity>();

        transform.position = Vector3.MoveTowards(transform.position, enemyPosition, speed * Time.fixedDeltaTime);

        // 목표에 도착했는지 확인
        if (Vector3.Distance(transform.position, enemyPosition) <= 0.1f)
        {
            entity.HP -= damage;
            //entity.Hit(player, player.baseEntity.Type);
            Destroy(this.gameObject);
        }
    }
}
