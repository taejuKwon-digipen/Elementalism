using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageManager
{
    static readonly float[,] weaknessTab = {
        // Defense : Fire, Water, Wind, Earth, Neutral
        {1f, 0.5f, 2f, 1f, 1f},   // Attack : Fire
        {2f, 1f, 1f, 0.5f, 1f},   // Attack : Water
        {1f, 1f, 1f, 2f, 1f},     // Attack : Wind
        {1f, 2f, 0.5f, 1f, 1f},   // Attack : Earth
        {1f, 1f, 1f, 1f, 1f}      // Attack : Neurtal
    };

    public static int ComputeDamages(Entity attacker, Entity defender, EntityType attackType)
    {
        float multiplier = weaknessTab[(int)attackType, (int)defender.baseEntity.Type];
        float damages = attacker.baseEntity.Attack - defender.baseEntity.Defense;

        if (damages < 0)
            damages = 0;
        
        damages *= multiplier;

        float criticalHitRate = Mathf.Min(attacker.baseEntity.Luck / 100F * 0.5f, 1f);  // Means that 100 Luck stat = 50% Critical Hit Rate

        if (UnityEngine.Random.value < criticalHitRate)
            damages *= 2.0f; // critical hit;
        return (int)Math.Round(damages);
    }
}
