using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Image 컴포넌트 사용을 위해 추가
using TMPro;

public abstract class Enemy : Entity 
{
    [SerializeField]
    protected float durationFadeOut = 1f;

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

    // 화상 상태 관련 변수
    private int burnStacks = 0;
    private bool isDying = false; // 사망 처리 중복 방지 플래그

    // 화상 스택 UI 관련 변수
    public GameObject burnStatusUIPrefab; // 인스펙터에서 할당할 프리팹
    private GameObject burnStatusUIInstance;
    private TextMeshProUGUI burnStatusText;
    private Coroutine flashRedCoroutine;

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

        InitializeBurnStatusUI(); // 화상 스택 UI 초기화
    }

    private void InitializeBurnStatusUI()
    {
        if (burnStatusUIPrefab != null)
        {
            // 플레이어의 쉴드 UI처럼 적의 자식으로 생성하고 위치를 조정한다고 가정합니다.
            // 필요에 따라 Canvas를 부모로 설정하거나 위치를 정밀하게 조정해야 합니다.
            burnStatusUIInstance = Instantiate(burnStatusUIPrefab, transform); 
            // 예시 위치: 적의 약간 위쪽 (조정 필요)
            burnStatusUIInstance.transform.localPosition = new Vector3(0, GetComponent<RectTransform>().sizeDelta.y * 0.6f, 0); 

            if (burnStatusUIInstance != null)
            {
                burnStatusText = burnStatusUIInstance.GetComponentInChildren<TextMeshProUGUI>();
                if (burnStatusText == null)
                {
                    Debug.LogError($"[Enemy] {gameObject.name}: Burn Status UI Prefab에 TextMeshProUGUI 컴포넌트가 없습니다.");
                }
            }
        }
        else
        {
            Debug.LogWarning($"[Enemy] {gameObject.name}: Burn Status UI Prefab이 할당되지 않았습니다.");
        }
        UpdateBurnStatusUI(); // 초기 상태 업데이트
    }

    private void UpdateBurnStatusUI()
    {
        if (burnStatusUIInstance == null) return;

        if (burnStacks > 0 && HP > 0 && !isDying)
        {
            burnStatusUIInstance.SetActive(true);
            if (burnStatusText != null)
            {
                burnStatusText.text = burnStacks.ToString();
            }
        }
        else
        {
            burnStatusUIInstance.SetActive(false);
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

    // 화상 효과 적용 메서드
    public void ApplyBurn(int stacks)
    {
        if (HP <= 0 || isDying) return; // 이미 죽었거나 죽는 중인 적은 화상 입지 않음

        this.burnStacks += stacks;
        Debug.Log($"[Enemy] {gameObject.name} got {stacks} burn stacks. Total: {this.burnStacks}.");
        UpdateBurnStatusUI(); // UI 업데이트
        // 여기에 화상 이펙트 시각/사운드 효과를 추가할 수 있습니다.
    }

    // 턴 종료 시 화상 데미지 처리 코루틴
    private IEnumerator ProcessBurnDamage()
    {
        if (burnStacks > 0 && HP > 0 && !isDying)
        {
            int damageToTake = burnStacks;
            
            // 빨갛게 번쩍이는 효과 시작
            if (flashRedCoroutine != null) StopCoroutine(flashRedCoroutine);
            flashRedCoroutine = StartCoroutine(FlashRedEffect());
            
            // 데미지 텍스트 표시 (Hit 메서드와 유사하게)
            if (hubDamageText != null)
            {
                GameObject hubText = Instantiate(hubDamageText);
                hubText.transform.position = this.transform.position; // 부모의 위치를 사용할 수도 있음
                // hubText.transform.SetParent(this.transform.parent.parent, false); // Canvas를 부모로 설정하고 싶을 경우
                DamageText dmgTextComponent = hubText.GetComponent<DamageText>();
                if (dmgTextComponent != null)
                {
                    dmgTextComponent.damage = damageToTake;
                    dmgTextComponent.isCritical = false; // 화상 피해는 일반 피해로 표시 (조정 가능)
                }
            }

            this.HP -= damageToTake;
            Debug.Log($"[Enemy] {gameObject.name} takes {damageToTake} burn damage. HP left: {this.HP}.");

            burnStacks--;
            UpdateBurnStatusUI(); // UI 업데이트
            Debug.Log($"[Enemy] {gameObject.name} burn stacks reduced to {this.burnStacks}.");

            if (this.HP <= 0)
            {
                this.HP = 0;
                Die();
            }
            yield return new WaitForSeconds(0.2f); // 데미지 처리 후 짧은 대기
        }
    }

    private IEnumerator FlashRedEffect()
    {
        if (enemyImage != null && HP > 0) // 살아있을 때만 효과 적용
        {
            enemyImage.color = new Color(1f, 0.6f, 0.6f, enemyImage.color.a); // 약간 붉은색 (알파 유지)
            yield return new WaitForSeconds(0.2f); // 번쩍이는 시간
            if (!isFrozen) // 빙결 상태가 아니라면 원래 색으로
            {
                enemyImage.color = originalColor;
            }
            else // 빙결 상태라면 빙결 색(파란색)으로
            {
                 enemyImage.color = new Color(0.5f, 0.8f, 1f, enemyImage.color.a);
            }
        }
        flashRedCoroutine = null;
    }

    public override int Hit(Entity attacker, EntityType attackType, int damageAmount)
    {
        if (isDying || HP <= 0) return 0; // 이미 죽었거나 죽는 중이면 피해 받지 않음

        GameObject hubText = Instantiate(hubDamageText);
        hubText.transform.position = this.transform.position;
        hubText.GetComponent<DamageText>().damage = damageAmount;
        this.HP -= damageAmount;

        if (this.HP <= 0)
        {
            this.HP = 0;
            Die(attacker as Player); // 공격자가 플레이어면 전달
        }
        return damageAmount;
    }

    protected virtual void Die(Player playerAttacker = null) // 플레이어 공격자 정보를 받을 수 있도록 수정
    {
        if (isDying) return;
        isDying = true;

        Debug.Log($"[Enemy] {gameObject.name} died.");

        if (burnStatusUIInstance != null) // 화상 UI 파괴
        {
            Destroy(burnStatusUIInstance);
        }

        if (this.transform.parent.Find("HealthBar") != null)
            Destroy(this.transform.parent.Find("HealthBar").gameObject);
        
        focusManager.ChangeWhenFocusedDie();
        
        if (playerAttacker != null) // Hit을 통해 죽었을 때만 골드 지급 (화상으로 죽으면 null)
        {
            playerAttacker.GainGold(this.baseEntity);
        }
        else if (player != null) // 화상 등 다른 요인으로 죽었을 때, 필드에 있는 플레이어가 골드를 얻도록 할 수도 있음 (선택적)
        {
            playerAttacker.GainGold(this.baseEntity);
            // 예: player.GainGold(this.baseEntity); // 또는 상태이상으로 죽었을땐 골드 X 등의 규칙 적용
        }
        
        StartCoroutine(Fade());
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
        if (HP <= 0 || isDying) 
        {
            Debug.Log($"[Enemy] {gameObject.name} HP is {HP} or isDying ({isDying}). Skipping turn processing.");
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
            yield return new WaitForSeconds(0.5f); 
            // 빙결 상태에서도 화상 데미지는 받을 수 있도록 ProcessBurnDamage 호출 (게임 규칙에 따라 다름)
            // 여기서는 빙결 시 행동만 막고, 상태이상 데미지는 받도록 함
            yield return StartCoroutine(ProcessBurnDamage()); 
            yield break; 
        }

        yield return StartCoroutine(EnemySpecificActions()); // 실제 적 행동
        
        // 행동 후 화상 데미지 처리
        yield return StartCoroutine(ProcessBurnDamage());
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
