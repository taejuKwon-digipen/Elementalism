using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBehavior : MonoBehaviour
{
    public float speed = 10;
    public GameObject focusedEnemy;
    public Entity player;
    // Start is called before the first frame update
    void Start()
    {
        //_startPosition = transform.position;
        transform.position = player.transform.position;
        //Debug.Log(_startPosition);
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

        //this.transform.position = Vector3.zero + _startPosition;

        //if (enemyPosition.x <= _startPosition.x ) {
        //    entity.Hit(player, player.baseEntity.Type);
        //    Destroy(this.gameObject);
        //}

        // 이동 로직 추가
        //Vector3 direction = (enemyPosition - transform.position).normalized; // 방향 계산
        //transform.position += direction * speed * Time.fixedDeltaTime; // 이동

        transform.position = Vector3.MoveTowards(transform.position, enemyPosition, speed * Time.fixedDeltaTime);

        Debug.Log(enemyPosition);
        // 목표에 도착했는지 확인
        if (Vector3.Distance(transform.position, enemyPosition) <= 0.1f)
        {
            entity.Hit(player, player.baseEntity.Type);
            Destroy(this.gameObject);
        }
    }
}
