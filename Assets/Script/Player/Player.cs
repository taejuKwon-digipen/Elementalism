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

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        focusManager = FindAnyObjectByType<FocusManager>();
        CurrentHP = MaxHP;
        animator = GetComponent<Animator>();
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
}
