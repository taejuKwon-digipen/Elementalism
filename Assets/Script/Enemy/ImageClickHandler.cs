using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ImageClickHandler : MonoBehaviour, IPointerClickHandler
{
    public GameObject Object;
    public GameObject Canva;
    private Entity entity;

    private void Start()
    {
        entity = this.GetComponent<Entity>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        int damage = entity.Hit(entity);

        GameObject obj = Instantiate(
            Object,
            entity.transform.position,
            Quaternion.identity,
            Canva.transform
        );
        Damage d = obj.GetComponent<Damage>();

        d.SetText('-' + damage.ToString());
    }
}
