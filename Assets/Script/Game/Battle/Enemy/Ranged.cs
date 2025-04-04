using System.Collections;
using UnityEngine;

public class Ranged : Enemy
{
    protected override void Start()
    {
        base.Start();
    }

    private void Update()
    {
        base.Update();  // 부모 클래스의 Update 메서드 호출
    }

    public override IEnumerator Turn()
    {
        GameObject playerPoint = player.gameObject;
        float xDistance = Mathf.Abs(this.transform.position.x - playerPoint.transform.position.x);

        if (xDistance < 2)
        {
            yield return StartCoroutine(GoBackward(1));
            yield break;
        }

        animator.SetTrigger("Attack");
        // baseEntity의 Attack 값을 데미지로 사용
        player.Hit(this, baseEntity.Type, baseEntity.Attack);
        yield return null;
    }
}
