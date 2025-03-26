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
    private bool isInvincible = false;

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
            Die();
        }
    }

    public void Attack()
    {
        // 현재 활성화된 카드의 ID 가져오기
        int currentCardID = 0;
        if (GridChecker.inst != null && GridChecker.inst.GetActiveCards().Count > 0)
        {
            currentCardID = GridChecker.inst.GetActiveCards()[0].carditem.ID;
        }
        Debug.Log($"Using Card ID {currentCardID}");

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
        // 현재 활성화된 카드의 ID 가져오기
        int currentCardID = 0;
        if (GridChecker.inst != null && GridChecker.inst.GetActiveCards().Count > 0)
        {
            currentCardID = GridChecker.inst.GetActiveCards()[0].carditem.ID;
        }
        Debug.Log($"Using Card ID {currentCardID}");

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
        if (Shield > 0)
        {
            if (Shield >= damageAmount)
            {
                Shield -= damageAmount;
                return 0;
            }
            else
            {
                int remainingDamage = damageAmount - Shield;
                Shield = 0;
                this.HP -= remainingDamage;
                return remainingDamage;
            }
        }

        return base.Hit(attacker, attackType, damageAmount);
    }

    public void TakeDamage(int damage, int oraCount)
    {
        if (isInvincible) return;

        if (oraCount > 0)
        {
            damage *= 2;
            Debug.Log($"Player: Critical Hit! Damage: {damage}");
        }

        int currentCardID = 0;
        if (GridChecker.inst != null && GridChecker.inst.GetActiveCards().Count > 0)
        {
            currentCardID = GridChecker.inst.GetActiveCards()[0].carditem.ID;
        }

        if (currentCardID == 9)
        {
            damage = Mathf.Max(1, damage - 2);
            Debug.Log($"Player: Damage reduced by 2 due to Card ID 9. Final damage: {damage}");
        }

        if (currentCardID == 10)
        {
            damage *= 2;
            Debug.Log($"Player: Damage doubled due to Card ID 10. Final damage: {damage}");
        }

        CurrentHP -= damage;
        Debug.Log($"Player: Took {damage} damage. Current health: {CurrentHP}");

        if (CurrentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player died!");
        // 게임 오버 로직 추가
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
    }
}
