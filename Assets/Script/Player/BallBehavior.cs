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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate() {
        var currentPosition = transform.position;

        if (focusedEnemy == null)
            return;

        var enemyPosition = focusedEnemy.GetComponent<RectTransform>().position;
        var entity = focusedEnemy.GetComponentInChildren<Entity>();

        this.transform.position = new Vector3(currentPosition.x + speed, currentPosition.y, currentPosition.z);
        if (enemyPosition.x <= currentPosition.x ) {
            entity.Hit(player, player.baseEntity.Type);
            Destroy(this.gameObject);
        }
    }
}
