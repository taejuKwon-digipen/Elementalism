using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Image 컴포넌트 사용을 위해 추가

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

    // 빙결 상태 관련 변수
    private bool isFrozen = false;
    private int frozenTurns = 0;
    private Image enemyImage;
    private Color originalColor;

    protected virtual void Awake() {
        focusManager = GameObject.Find("FocusManager").GetComponent<FocusManager>();
        animator = this.GetComponent<Animator>();
        enemyImage = GetComponent<Image>(); // 자식의 Image를 가져오려면 GetComponentInChildren<Image>()
        if (enemyImage != null)
        {
            originalColor = enemyImage.color;
        }
        else
        {
            // Player.cs의 경우 GetComponent<Animator>() 후 Sprite를 가져오지만, Enemy는 Image를 직접 사용한다고 가정
            // 만약 SpriteRenderer를 사용한다면 해당 컴포넌트로 변경 필요
            Debug.LogWarning($"[Enemy] {gameObject.name}: Image 컴포넌트를 찾을 수 없습니다. 색상 변경이 작동하지 않을 수 있습니다.");
        }
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

    // 빙결 효과 적용 메서드
    public void Freeze(int turns)
    {
        if (HP <= 0) return; // 이미 죽은 적은 빙결시키지 않음

        isFrozen = true;
        frozenTurns = turns;
        if (enemyImage != null)
        {
            enemyImage.color = new Color(0.5f, 0.8f, 1f, enemyImage.color.a); // 파란색 계열 (알파는 유지)
        }
        Debug.Log($"[Enemy] {gameObject.name} is frozen for {turns} turn(s).");
    }

    // 빙결 효과 해제 메서드
    public void Unfreeze()
    {
        isFrozen = false;
        frozenTurns = 0;
        if (enemyImage != null)
        {
            enemyImage.color = originalColor;
        }
        Debug.Log($"[Enemy] {gameObject.name} is unfrozen.");
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

    // 턴 처리 로직 (공통 빙결 처리 포함)
    public IEnumerator ProcessTurn()
    {
        if (HP <= 0) 
        {
            Debug.Log($"[Enemy] {gameObject.name} HP is {HP}. Skipping turn processing.");
            yield break;
        }

        if (isFrozen)
        {
            Debug.Log($"[Enemy] {gameObject.name} is frozen. Skipping actions. Turns remaining: {frozenTurns}");
            frozenTurns--;
            if (frozenTurns <= 0)
            {
                Unfreeze();
            }
            // 적 턴에도 약간의 딜레이가 필요할 수 있음 (EnemyManager의 EnemyTurn 코루틴의 WaitForSecondsRealtime(1)과 유사하게)
            // 또는 즉시 종료. 여기서는 다음 적의 턴으로 바로 넘어가지 않도록 약간의 시각적 딜레이를 줌.
            yield return new WaitForSeconds(0.5f); // 빙결된 적도 턴을 소모하는 느낌을 주기 위함
            yield break; 
        }
        yield return StartCoroutine(EnemySpecificActions()); // 실제 적 행동
    }

    // 각 Enemy 타입이 구현해야 할 실제 행동 로직
    protected abstract IEnumerator EnemySpecificActions();

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
