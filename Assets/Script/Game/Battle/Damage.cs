using System;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class Damage : MonoBehaviour
{
    public float Timer;

    private TextMeshProUGUI Text;
    private float BeginningTimer;

    private void Awake()
    {
        Rigidbody2D rb = this.GetComponent<Rigidbody2D>();
        Text = this.GetComponent<TextMeshProUGUI>();
        float xForce = UnityEngine.Random.Range(-0.8F, 0.8F);
        BeginningTimer = Timer;

        rb.AddForce(new Vector2(xForce * 50, 50), ForceMode2D.Impulse);
    }

    private void FixedUpdate()
    {
        Timer -= Time.deltaTime;

        if (Text.alpha <= 0)
        {
            Destroy(this.gameObject);
        }
        float alpha = Timer / BeginningTimer;
        Text.alpha = alpha < 0 ? 0 : alpha;
    }

    public void SetText(string text)
    {
        Text.text = text;
    }
}
