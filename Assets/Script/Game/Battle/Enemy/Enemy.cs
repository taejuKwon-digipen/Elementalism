using System.Collections;
using UnityEngine;

public abstract class Enemy : Entity 
{
    [SerializeField]
    protected float durationFadeOut = 5f;

    [SerializeField]
    protected FocusManager focusManager;

    protected Player player;

    protected EnemyManager em;
    protected Animator animator;

    public GameObject hubDamageText;

    protected void Awake() {
        focusManager = GameObject.Find("FocusManager").GetComponent<FocusManager>();
        animator = this.GetComponent<Animator>();
    }

    protected override void Start() {
        base.Start();
        player = GameObject.FindWithTag("Player").GetComponentInChildren<Player>();
    }

    protected virtual void Update()
    {
        // 0 키를 누르면 Enemy의 HP를 1로 설정 (테스트용)
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            Debug.Log($"[Enemy] 0 키가 눌렸습니다. 현재 오브젝트: {gameObject.name}");
            Debug.Log($"[Enemy] HP 변경 전: {HP}");
            HP = 1;
            Debug.Log($"[Enemy] HP 변경 후: {HP}");
        }
    }

    public void SetDmgTextPrefab(GameObject hubDmgTextfab)
    {
        hubDamageText = hubDmgTextfab;
    }
    public override int Hit(Entity attacker, EntityType attackType, int damageAmount)
    {
        GameObject hubText = Instantiate(hubDamageText);
        hubText.transform.position = this.transform.position;
        hubText.GetComponent<DamageText>().damage = damageAmount;
        this.HP -= damageAmount;
        if (this.HP <= 0)
        {
            this.HP = 0;
            if (this.transform.parent.Find("HealthBar") != null)
                Destroy(this.transform.parent.Find("HealthBar").gameObject);
            
            // 포커스 매니저에게 적이 죽었음을 알림
            focusManager.ChangeWhenFocusedDie();
            
            // 플레이어에게 골드 지급
            if (attacker is Player player)
            {
                player.GainGold(this.baseEntity);
            }
            
            StartCoroutine(Fade());
        }
        return damageAmount;
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
        var velocity = UnityEngine.Vector3.zero;
        var currentPosition = this.transform.parent.position;
        var finalPosition = new UnityEngine.Vector3(currentPosition.x - xDistance, currentPosition.y, currentPosition.z);

        while (elapsedTime < 0.7f) {
            elapsedTime += Time.deltaTime;
            currentPosition = UnityEngine.Vector3.SmoothDamp(currentPosition, finalPosition, ref velocity, 0.15f);
            this.transform.parent.position = currentPosition;
            yield return null;
        }
        this.transform.parent.position = finalPosition;
    }

    protected IEnumerator GoBackward(float xDistance)
    {
        float elapsedTime = 0.0f;
        var velocity = UnityEngine.Vector3.zero;
        var currentPosition = this.transform.parent.position;
        var finalPosition = new UnityEngine.Vector3(currentPosition.x + xDistance, currentPosition.y, currentPosition.z);

        while (elapsedTime < 0.7f) {
            elapsedTime += Time.deltaTime;
            this.transform.parent.position = UnityEngine.Vector3.SmoothDamp(this.transform.parent.position, finalPosition, ref velocity, 0.7f);
            yield return null;
        }

        this.transform.parent.position = finalPosition;
    }

    public void SetEnemyManager(EnemyManager enemyManager)
    {
        em = enemyManager;
    }

    protected void OnDestroy() {
        em.DestroyEnemy(this.transform.parent.gameObject);
    }

    protected virtual bool IsObstacleInFront()
    {
        UnityEngine.Vector3 startingPoint = this.GetComponent<RectTransform>().position;
        startingPoint.y += this.GetComponent<RectTransform>().sizeDelta.y / 2;
        RaycastHit2D[] infos = Physics2D.RaycastAll(startingPoint, UnityEngine.Vector3.left, 100);
        bool flag = false;

        foreach (var info in infos) {
            if (info.collider.gameObject != transform.parent.gameObject) {
                flag = true;
                Debug.Log("Something in front");
            }
        }
        return flag;
    }

}
