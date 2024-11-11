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

        // �̵� ���� �߰�
        //Vector3 direction = (enemyPosition - transform.position).normalized; // ���� ���
        //transform.position += direction * speed * Time.fixedDeltaTime; // �̵�

        transform.position = Vector3.MoveTowards(transform.position, enemyPosition, speed * Time.fixedDeltaTime);

        Debug.Log(enemyPosition);
        // ��ǥ�� �����ߴ��� Ȯ��
        if (Vector3.Distance(transform.position, enemyPosition) <= 0.1f)
        {
            entity.Hit(player, player.baseEntity.Type);
            Destroy(this.gameObject);
        }
    }
}
