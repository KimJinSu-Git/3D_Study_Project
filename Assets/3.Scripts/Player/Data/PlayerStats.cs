using System;
using UnityEngine;

/// <summary>
/// PlayerStats ::: 플레이어의 최종 능력치 결과를 저장하는 '그릇' 역할
/// </summary>
[Serializable]
public struct PlayerStats
{
    public float BaseDamage;
    public float MaxHealth;
    public float MaxStamina;
    public float AttackSpeed;
    public float CriticalChance;
    public float CriticalDamageMultiplier;
    public float MovementSpeed;

    public static PlayerStats GetDefaultBase()
    {
        return new PlayerStats
        {
            BaseDamage = 10f,
            MaxHealth = 1000f,
            MaxStamina = 100f,
            AttackSpeed = 1.0f,
            CriticalChance = 0.05f,
            CriticalDamageMultiplier = 1.5f,
            MovementSpeed = 7.0f
        };
    }
}
