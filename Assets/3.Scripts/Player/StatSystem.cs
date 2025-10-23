using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StatSystem : MonoBehaviour
{
    [SerializeField] private PlayerStats mBaseStats = default;
    
    private List<ItemData> _equippedItems = new List<ItemData>();

    public PlayerStats FinalStats { get; private set; } // 최종 스탯을 읽기 전용으로 외부에 노출(PlayerController, Health 등이 이 값을 읽어 스탯을 결정하도록)
    
    public event UnityAction OnStatsRecalculated;

    private void Awake()
    {
        if (mBaseStats.MaxHealth == 0)
        {
            mBaseStats = PlayerStats.GetDefaultBase();
        }
            
        RecalculateStats();
    }
    
    /// <summary>
    /// EquipItem ::: 아이템 장착
    /// 아이템을 _equippedItems 리스트에 추가하고, RecalculateStats()를 호출하여 스탯을 다시 계산
    /// </summary>
    /// <param name="item"></param>
    public void EquipItem(ItemData item)
    {
        if (!_equippedItems.Contains(item))
        {
            _equippedItems.Add(item);
            RecalculateStats();
        }
    }
    public void UnequipItem(ItemData item)
    {
        if (_equippedItems.Contains(item))
        {
            _equippedItems.Remove(item);
            RecalculateStats();
        }
    }

    /// <summary>
    /// RecalculateStats ::: 스탯재계산 총괄 파이프라인 역할
    /// </summary>
    public void RecalculateStats()
    {
        PlayerStats currentStats = mBaseStats; // mBaseStats 복사
        
        Dictionary<StatType, float> flatModifiers = new Dictionary<StatType, float>();
        Dictionary<StatType, float> percentageModifiers = new Dictionary<StatType, float>();
        
        foreach (ItemData item in _equippedItems) // 모든 장착 장비의 옵션을 순회하여
        {
            foreach (StatModifier mod in item.mModifiers)
            {
                Dictionary<StatType, float> targetDict =  // flatModifiers, percentageModifiers 두 개의 Dictionary에 누적.
                    mod.ModifierType == ModifierType.Flat ? flatModifiers : percentageModifiers;
            
                if (!targetDict.ContainsKey(mod.StatType)) // 아직 해당 값이 들어온게 없다면
                {
                    targetDict[mod.StatType] = 0f; // 0으로 설정하고
                }
                targetDict[mod.StatType] += mod.Value; // 값들을 누적시켜서 저장시킨다.
            }
        }
        
        ApplyModifiers(ref currentStats, flatModifiers, percentageModifiers); // 스탯 계산
        
        FinalStats = currentStats; // 최종 스탯에 결과를 저장
        
        // 다른 컴포넌트에 변경을 알림
        GetComponent<Health>()?.SetMaxHealth(FinalStats.MaxHealth);
        GetComponent<Stamina>()?.SetMaxStamina(FinalStats.MaxStamina);
            
        OnStatsRecalculated?.Invoke();
    }
    
    private void ApplyModifiers(ref PlayerStats stats, Dictionary<StatType, float> flat, Dictionary<StatType, float> percentage)
    {
        // Flat과 Percentage을 분리하여 적용합니다. (StatType별로 switch-case 사용)
    
        // Flat 적용
        if (flat.ContainsKey(StatType.BaseDamage))
        {
            stats.BaseDamage += flat[StatType.BaseDamage];
        }
    
        // Percentage 적용
        if (percentage.ContainsKey(StatType.AttackSpeed))
        {
            stats.AttackSpeed *= (1 + percentage[StatType.AttackSpeed] / 100f); 
        }
    
        // TODO: 나머지 StatType에 대한 Flat/Percentage 로직 추가
    }
    
    public void SetBaseStats(PlayerStats newBaseStats)
    {
        mBaseStats = newBaseStats;
        RecalculateStats();
    }
}