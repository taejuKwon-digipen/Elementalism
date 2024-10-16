using TMPro;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public Entity entity;

    // HealthBar assets
    private TextMeshProUGUI text;
    private RectTransform health;
    private float maxScale = 1 ;


    private void Start() {
        text = transform.Find("Text").GetComponent<TextMeshProUGUI>();
        health = transform.Find("Front").GetComponent<RectTransform>();
        text.text = entity.ToString() + " / " + entity.MaxHP.ToString();

        maxScale = health.localScale.x;
    }

    void Update()
    {
        text.text = entity.HP.ToString() + " / " + entity.MaxHP.ToString();
        health.localScale = new Vector3(maxScale * entity.HP / entity.MaxHP, 1, 1);
    }
}
