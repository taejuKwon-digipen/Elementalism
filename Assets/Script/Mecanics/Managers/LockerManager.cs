using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FocusManager : MonoBehaviour
{
    public GraphicRaycaster gRaycaster;
    public EventSystem eventSystem;
    public GameObject Focus;

    private Entity FocusedEntity;
    private Transform canvas;

    private void Awake() {
        canvas = GameObject.Find("Canvas").GetComponent<Transform>();
    }

    // Update is called once per frame
    private void Update()
    {
        GetEnemyToFocus();
        ChangeWhenFocusedDie();
    }

    /**
      * <summary>Allows to focus one of the enemy entity</summary>
      */
    private void GetEnemyToFocus()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PointerEventData pointerData = new PointerEventData(eventSystem)
            {
                position = Input.mousePosition
            };
            List<RaycastResult> raycastResults = new();

            gRaycaster.Raycast(pointerData, raycastResults);
            if (raycastResults.Count == 0) // If there's no UI element clicked
                return;

            foreach (RaycastResult result in raycastResults)
            {
                var gObject = result.gameObject.transform.parent.gameObject;
                if (!gObject.CompareTag("Enemy"))
                    continue;
                FocusedEntity = gObject.transform.Find("Entity").GetComponent<Entity>();
                FocusEntity(gObject);
            }
        }
    }

    private void FocusEntity(GameObject entity)
    {
        Transform FocusPoint = entity.transform.Find("FocusPoint").GetComponent<Transform>();
        Focus.transform.position = FocusPoint.position;
        return;
    }

    /**
      * <summary>Allows to change the focus when the focused enemy is dead</summary>
      */
    private void ChangeWhenFocusedDie()
    {
        if (FocusedEntity != null)
            return;
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length == 0) {
            Focus.transform.position = new Vector3(-50, -50, 0);
            return;
        }
        var FocusPoint = enemies[0].transform.Find("FocusPoint").transform.position;
        Focus.transform.position = FocusPoint;
    }

    /**
      * <summary>Get the the enemy currently focused</summary>
      */
    public Entity GetFocusedEntity()
    {
        return FocusedEntity;
    }
}
