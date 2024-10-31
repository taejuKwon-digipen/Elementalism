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

    // Update is called once per frame
    private void Update()
    {
        ChangeWhenFocusedDie();
    }

    /**
      * <summary>Allows to change the focus when the focused enemy is dead</summary>
      */
    private void ChangeWhenFocusedDie()
    {
        if (FocusedEntity == null) {
            SwapEntityToFocus();
        } else if (FocusedEntity != null && FocusedEntity.HP <= 0) {
            Focus.transform.SetParent(FocusedEntity.transform.parent.parent);
            SwapEntityToFocus();
        }
    }

    private void SwapEntityToFocus()
    {
        List<GameObject> enemies = GameObject.FindGameObjectsWithTag("Enemy").ToList();
        enemies.RemoveAll(enemy => enemy.transform.Find("Entity").GetComponent<Entity>().HP <= 0);
        if (enemies.Count == 0) {
            if (currentCoroutine != null)
                StopCoroutine(currentCoroutine);
            Focus.transform.position = new Vector3(-50, -50, 0);
            return;
        }
        var FocusPoint = enemies[0].transform.Find("FocusPoint").transform.position;
        FocusedEntity = enemies[0].transform.Find("Entity").GetComponent<Entity>();
        Focus.transform.SetParent(FocusedEntity.transform.parent);
        if (Focus.transform.position != hideFocusPoint)
            MoveFocus(FocusPoint);
        else
            Focus.transform.position = FocusPoint;
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
