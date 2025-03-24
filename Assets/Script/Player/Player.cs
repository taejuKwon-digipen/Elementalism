using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    public GameObject ball;
    private FocusManager focusManager;
    public ShopManager shopManager;
    public int totalMonsters = 1;    
    private int defeatedMonsters = 0;
    public int Gold { get; set; } = 100;
    public int MaxHP = 100;
    public int CurrentHP { get; private set; }
    private Animator animator;
    public GameObject fireballPrefab;
    public Transform fireballSpawnPoint;
    public float fireballSpeed = 5f;
    private int shield = 0;

    public static Player inst;

    private void Awake()
    {
        inst = this;
    }
    public int Shield
    {
        get => shield;
        private set => shield = Mathf.Max(0, value);
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        focusManager = FindAnyObjectByType<FocusManager>();
        CurrentHP = MaxHP;
        animator = GetComponent<Animator>();
        if (GameManager.Instance.Player_HP != 0)
        {
            HP = GameManager.Instance.Player_HP;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (this.HP <= 0) {
            this.HP = 0;
            // GameOver
        }
    }

    public void Attack()
    {
        Debug.Log($"Using Card ID {GridChecker.inst.UsingCard_ID}");
        var entity = focusManager.GetFocusedEntity();
        if (entity == null || entity.HP <= 0)
            return;

        Vector3 newPosition = this.transform.position;
        newPosition.y += this.GetComponent<RectTransform>().sizeDelta.y / 2;
        GameObject enemyObject = focusManager.GetFocusedEntity().transform.parent.gameObject;
        GameObject newBall = Instantiate(ball, newPosition, Quaternion.identity, this.transform.parent.parent);
        BallBehavior behavior = newBall.GetComponent<BallBehavior>();

        behavior.focusedEnemy = enemyObject;
        behavior.player = this;
    }

    public void AttackWithDamage(int damage)
    {
        Debug.Log($"Using Card ID {GridChecker.inst.UsingCard_ID}");
        var entity = focusManager.GetFocusedEntity();
        if (entity == null || entity.HP <= 0)
            return;

        // 볼 생성 및 발사
        Vector3 newPosition = this.transform.position;
        newPosition.y += this.GetComponent<RectTransform>().sizeDelta.y / 2;

        GameObject enemyObject = focusManager.GetFocusedEntity().transform.parent.gameObject;
        GameObject newBall = Instantiate(ball, newPosition, Quaternion.identity, this.transform.parent.parent);
        BallBehavior behavior = newBall.GetComponent<BallBehavior>();

        behavior.focusedEnemy = enemyObject;
        behavior.player = this;
        behavior.damage = damage;


    }

    public void HealHP(int amount)
    {
        CurrentHP = Mathf.Min(CurrentHP + amount, MaxHP);
    }

    public void OnMonsterDefeated()
    {
        defeatedMonsters++;
        Gold += 50;
    }

    public void AddShield(int amount)
    {
        Shield += amount;
        Debug.Log($"Shield added: {amount}, Total shield: {Shield}");
    }

    public override int Hit(Entity attacker, EntityType attackType, int damageAmount)
    {
        // 쉴드가 있다면 먼저 쉴드로 데미지를 막음
        if (Shield > 0)
        {
            if (Shield >= damageAmount)
            {
                Shield -= damageAmount;
                return 0;  // 모든 데미지를 쉴드가 막음
            }
            else
            {
                int remainingDamage = damageAmount - Shield;
                Shield = 0;
                this.HP -= remainingDamage;
                return remainingDamage;
            }
        }

        // 쉴드가 없으면 일반적인 데미지 처리
        return base.Hit(attacker, attackType, damageAmount);
    }
}
