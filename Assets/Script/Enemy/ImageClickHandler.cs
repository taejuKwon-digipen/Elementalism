using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ImageClickHandler : MonoBehaviour, IPointerClickHandler
{
    public GameObject Object;
    public GameObject Canva;
    private Enemy enemy;
    private Player player;

    private void Start()
    {
        enemy = this.GetComponent<Enemy>();
        player = GameObject.FindWithTag("Player").GetComponentInChildren<Player>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (enemy.HP <= 0)
            return;
        int damage = enemy.Hit(player, EntityType.Water);

        GameObject obj = Instantiate(
            Object,
            enemy.transform.position,
            Quaternion.identity,
            Canva.transform
        );
        Damage d = obj.GetComponent<Damage>();

        d.SetText('-' + damage.ToString());
        enemy.NotifyClickToLockManager();
    }
}
