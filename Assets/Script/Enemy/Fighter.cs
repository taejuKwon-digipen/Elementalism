using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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

        if (IsObstacleInFront())
            yield break;

        if (xDistance > 2) {
            yield return StartCoroutine(GoForward(1));
            yield break;
        }
        player.Hit(this, baseEntity.Type);
        yield return null;
    }

    override protected bool IsObstacleInFront()
    {
        Vector3 startingPoint = this.GetComponent<RectTransform>().position;
        startingPoint.y += 0.5f;
        RaycastHit2D[] infos = Physics2D.RaycastAll(startingPoint, UnityEngine.Vector3.left, 1.5f);
        bool flag = false;

        foreach (var info in infos) {
            if (info.collider.gameObject != transform.parent.gameObject)
            {
                flag = true;
                Debug.Log("Something in front");
            }
        }
        return flag;
    }

    private void Update()
    {
        Vector3 startingPoint = this.GetComponent<RectTransform>().position;
        startingPoint.y += 0.5f;
        Debug.DrawRay(startingPoint, UnityEngine.Vector3.left * 1.5f, Color.blue);
    } 
}
