using System;
using System.Collections;
using UnityEngine;

public abstract class Enemy : Entity 
{
    [SerializeField]
    protected float durationFadeOut = 5f;

    [SerializeField]
    protected FocusManager focusManager;

    protected Player player;

    protected void Awake() {
        focusManager = GameObject.Find("FocusManager").GetComponent<FocusManager>();
    }

    protected override void Start() {
        base.Start();

        player = GameObject.FindWithTag("Player").GetComponentInChildren<Player>();
    }

    public override int Hit(Entity attacker, EntityType attackType)
    {
        int damages = DamageManager.ComputeDamages(attacker, this, attackType);
        this.HP -= damages;
        if (this.HP <= 0) {
            this.HP = 0;
            if (this.transform.parent.Find("HealthBar") != null)
                Destroy(this.transform.parent.Find("HealthBar").gameObject);
            StartCoroutine(Fade());
        }
        return damages;
    }

    protected IEnumerator Fade()
    {
        UnityEngine.UI.Image image = this.GetComponent<UnityEngine.UI.Image>();
        Color baseColor = image.color;
        float elapsedTime = 0.0f;

        while (elapsedTime < durationFadeOut) {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(baseColor.a, 0, elapsedTime / durationFadeOut);
            image.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
            yield return null;
        }
        Destroy(this.transform.parent.gameObject);
    }

    public abstract IEnumerator Turn();

    public void NotifyClickToLockManager()
    {
        this.focusManager.SetFocusedEntity(this);
    }

    protected IEnumerator GoForward(float xDistance)
    {
        float elapsedTime = 0.0f;
        var velocity = Vector3.zero;
        var currentPosition = this.transform.parent.position;
        var finalPosition = new Vector3(currentPosition.x - xDistance, currentPosition.y, currentPosition.z);

        while (elapsedTime < 1.5f) {
            elapsedTime += Time.deltaTime;
            currentPosition = Vector3.SmoothDamp(currentPosition, finalPosition, ref velocity, 0.2f);
            this.transform.parent.position = currentPosition;
            yield return null;
        }
        this.transform.parent.position = finalPosition;
    }
}
