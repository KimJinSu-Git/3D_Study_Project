using UnityEngine;
using UnityEngine.Events;

public class StatSystem : MonoBehaviour
{
    [SerializeField] private PlayerStats mBaseStats = default;

    public PlayerStats FinalStats { get; private set; }
    
    public event UnityAction OnStatsRecalculated;

    private void Awake()
    {
        if (mBaseStats.MaxHealth == 0)
        {
            mBaseStats = PlayerStats.GetDefaultBase();
        }
            
        RecalculateStats();
    }

    public void RecalculateStats()
    {
        FinalStats = mBaseStats;
            
        // TODO: 1. 장비 옵션 합산 추가
        // TODO: 2. 버프/디버프 합산 추가
        
        GetComponent<Health>()?.SetMaxHealth(FinalStats.MaxHealth);
        GetComponent<Stamina>()?.SetMaxStamina(FinalStats.MaxStamina);
            
        OnStatsRecalculated?.Invoke();
    }
    
    public void SetBaseStats(PlayerStats newBaseStats)
    {
        mBaseStats = newBaseStats;
        RecalculateStats();
    }
}