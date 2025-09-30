using UnityEngine;

public interface IWeapon
{
    void Initialize(PlayerController controller, StatSystem statSystem); 
        
    // 공격
    void TryAttack(float chargeDuration = 0f);
        
    // 상태 초기화
    void ResetState();
    
    bool IsBusy { get; }
}