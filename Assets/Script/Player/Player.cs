using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    public GameObject ball;
    private FocusManager focusManager;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        focusManager = FindAnyObjectByType<FocusManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (this.HP <= 0) {
            this.HP = 0;
            // GameOver
        }
    }

    public void Attack()
    {
        var entity = focusManager.GetFocusedEntity();
        if (entity == null || entity.HP <= 0)
            return;

        Vector3 newPosition = this.transform.position;
        newPosition.y += this.GetComponent<RectTransform>().sizeDelta.y / 2;
        GameObject enemyObject = focusManager.GetFocusedEntity().transform.parent.gameObject;
        GameObject newBall = Instantiate(ball, newPosition, Quaternion.identity, this.transform.parent.parent);
        BallBehavior behavior = newBall.GetComponent<BallBehavior>();

        behavior.focusedEnemy = enemyObject;
        behavior.player = this;
    }
}
