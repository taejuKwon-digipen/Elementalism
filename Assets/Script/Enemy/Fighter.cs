using System.Collections;
using UnityEngine;

public class Fighter : Enemy
{
    protected override void Start()
    {
        base.Start();
    }

    public override IEnumerator Turn()
    {
        GameObject playerPoint = player.gameObject;
        float xDistance = Mathf.Abs(this.transform.position.x - playerPoint.transform.position.x);

        if (xDistance > 200) {
            yield return StartCoroutine(GoForward(100));
            yield break;
        }
        player.Hit(this, baseEntity.Type);
        yield return null;
    }
}