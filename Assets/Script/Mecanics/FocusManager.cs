using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FocusManager : MonoBehaviour
{
    public GameObject Focus;

    private Entity FocusedEntity;
    private Coroutine currentCoroutine;
    private float sּmoothMovementDuration = 1.0f;
    private readonly Vector3 hideFocusPoint = new (-50, -50, 0);

    void Awake()
    {
        // Focus 오브젝트가 없으면 찾기
        if (Focus == null)
        {
            Focus = GameObject.Find("Focus");
        }
    }

    void Start()
    {
        Debug.Log("FocusManager Start called");
        // 약간의 딜레이 후 포커스 설정 (다른 오브젝트들이 모두 초기화될 때까지 대기)
        StartCoroutine(InitialFocusDelay());
    }

    private IEnumerator InitialFocusDelay()
    {
        yield return new WaitForSeconds(0.1f);
        SwapEntityToFocus();
    }

    /**
      * <summary>Allows to change the focus when the focused enemy is dead</summary>
      */
    public void ChangeWhenFocusedDie()
    {
        if (FocusedEntity == null || FocusedEntity.HP <= 0)
        {
            Debug.Log("Focused entity died or null, swapping focus");
            if (FocusedEntity != null)
                Focus.transform.SetParent(FocusedEntity.transform.parent.parent);
            SwapEntityToFocus();
        }
    }

    private void SwapEntityToFocus()
    {
        Debug.Log("SwapEntityToFocus called");
        List<GameObject> enemies = GameObject.FindGameObjectsWithTag("Enemy").ToList();
        Debug.Log($"Found {enemies.Count} enemies");

        enemies.RemoveAll(enemy => 
        {
            var entityComponent = enemy.transform.Find("Entity");
            if (entityComponent == null) return true;
            var entity = entityComponent.GetComponent<Entity>();
            return entity == null || entity.HP <= 0;
        });

        if (enemies.Count == 0)
        {
            Debug.Log("No valid enemies found");
            if (currentCoroutine != null)
                StopCoroutine(currentCoroutine);
            Focus.transform.position = hideFocusPoint;
            FocusedEntity = null;
            return;
        }

        // 가장 가까운 적 찾기
        GameObject nearestEnemy = enemies[0];
        float nearestDistance = float.MaxValue;
        Vector3 playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;

        foreach (var enemy in enemies)
        {
            float distance = Vector3.Distance(playerPos, enemy.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestEnemy = enemy;
            }
        }

        var focusPointTransform = nearestEnemy.transform.Find("FocusPoint");
        if (focusPointTransform != null)
        {
            var FocusPoint = focusPointTransform.position;
            var entityTransform = nearestEnemy.transform.Find("Entity");
            if (entityTransform != null)
            {
                FocusedEntity = entityTransform.GetComponent<Entity>();
                Focus.transform.SetParent(FocusedEntity.transform.parent);

                if (Focus.transform.position != hideFocusPoint)
                    MoveFocus(FocusPoint);
                else
                    Focus.transform.position = FocusPoint;

                Debug.Log($"Focus set to enemy at position {FocusPoint}");
            }
        }
    }

    private void MoveFocus(Vector3 position)
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(SmoothMove(position));
    }

    private IEnumerator SmoothMove(Vector3 position)
    {
        float elapsedTime = 0.0f;
        Vector3 velocity = Vector3.zero;

        while (elapsedTime < sּmoothMovementDuration) {
            elapsedTime += Time.deltaTime;
            var currentPosition = Focus.transform.position;
            var newPosition = Vector3.SmoothDamp(currentPosition, position, ref velocity, 0.2f);
            Focus.transform.position = newPosition;
            yield return null;
        }
        Focus.transform.position = position;
    }

    /**
      * <summary>Get the the enemy currently focused</summary>
      */
    public Entity GetFocusedEntity()
    {
        return FocusedEntity;
    }

    public void SetFocusedEntity(Entity entity)
    {
        if (FocusedEntity != null && entity.transform.parent.gameObject == FocusedEntity.transform.parent.gameObject)
            return;
        FocusedEntity = entity;
        Focus.transform.SetParent(FocusedEntity.transform.parent);
        MoveFocus(entity.transform.parent.transform.Find("FocusPoint").transform.position);
    }

}
