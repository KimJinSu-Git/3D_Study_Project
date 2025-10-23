using System.Collections.Generic;
using UnityEngine;

public enum ElementType
{
    None = 0,
    Fire,
    Ice,
    Leaf,
    Lightning
}

public enum Grade
{
    Normal,
    Magic,
    Rare,
    Unique,
    Legendary
}

public enum ModifierType 
{ 
    Flat,       // 덧셈
    Percentage  // 곱셈
}

public enum StatType 
{ 
    BaseDamage, 
    MaxHealth, 
    AttackSpeed, 
    CriticalChance, 
    CriticalDamageMultiplier,
    MovementSpeed, 
    CooldownReduction, 
    GuardBreakPower,
    LifeOnHit
}

/// <summary>
/// StatModifier ::: 아이템에 붙어 있는 하나의 옵션을 정의하는 설계도 역할
/// </summary>
[System.Serializable]
public struct StatModifier
{
    public StatType StatType; // 무슨 옵션을
    public ModifierType ModifierType; // 어떻게 
    public float Value; // 얼마나 바꿀거야
    // +10 공격력 => StatType.BaseDamage, ModifierType.Flat, mValue = 10
    // +20% 공격 속도 => StatType.AttackSpeed, ModifierType.Percentage, mValue = 20
}

/// <summary>
/// ItemData ::: 아이템의 모든 정보를 담는 유니티 데이터 파일
/// </summary>
[CreateAssetMenu(fileName = "ItemData", menuName = "Bird/Item Data/Base", order = 1)]
public class ItemData : ScriptableObject
{
    public string ItemName;
    public Grade ItemGrade;
    public ElementType ElementType;
        
    [Header("Base Stat Values")]
    public float BaseDamage;
    public float BaseHealth;
    
    // mModifiers 리스트를 통해 여러 개의 StatModifier를 가질 수 있음, 최종 스탯 계산의 입력 값
    public List<StatModifier> mModifiers = new List<StatModifier>();
}   
