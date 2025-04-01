using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EntityType
{
    Fire,
    Water,
    Wind,
    Earth,
    Neutral,
}

[CreateAssetMenu(menuName = "Elementalism/Create New Entity")]
public class EntityBase : ScriptableObject
{
    public string Name;

    [TextArea]
    public string Description;

    public Sprite Sprite;
    public UnityEngine.RuntimeAnimatorController Animator;

    public EntityType Type;

    // Statistics
    public int MaxHP;
    public int MaxMP;

    public int Attack;

    public int Defense;

    public int Agility;

    public int Luck;

    // 골드 획득 범위
    [Header("Gold Reward")]
    public int minGold = 10;
    public int maxGold = 50;

    // 골드 획득 메서드
    public int GetGoldReward()
    {
        return Random.Range(minGold, maxGold + 1);
    }
}
