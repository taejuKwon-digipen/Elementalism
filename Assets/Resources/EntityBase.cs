using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
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
    public AnimatorController Animator;

    public EntityType Type;

    // Statistics
    public int MaxHP;
    public int MaxMP;

    public int Attack;

    public int Defense;

    public int Agility;

    public int Luck;
}
