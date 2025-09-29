using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] private float mMaxHealth = 100f;
    
    private float _currentHealth;

    // 이전의 C# 이벤트인 public static event Action 방식과 다르게 인스펙터에 노출이 가능하다는 UnityAction 사용해봄
    public UnityAction<float> OnHealthChanged;
    public UnityAction OnDied;

    private void Awake()
    {
        _currentHealth = mMaxHealth;
    }
    
    public void ApplyDamage(DamageInfo info)
    {
        float rawDamage = info.BaseDamage * info.DamageMultiplier;
    
        // TODO: 방어력/취약/저항 계산 로직 추가
    
        // TODO: 치명타 계산 로직 추가
    
        // 최종 데미지
        float finalDamage = rawDamage;
    
        if (finalDamage < 0) return;
        
        HitFeedback feedback = GetComponent<HitFeedback>();
        if (feedback != null)
        {
            feedback.ApplyFeedback(info);
        }
    
        _currentHealth -= finalDamage;
    
        // --- 피격 피드백 및 상태 이상 처리 ---
    
        // TODO: 경직/넉백 적용 로직 추가

        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            Die();
        }
    
        OnHealthChanged?.Invoke(_currentHealth);
    
        Debug.Log($"{gameObject.name}가 {finalDamage} 데미지를 입었습니다. 남은 체력 : {_currentHealth}");
    }
    
    public void Heal(float amount)
    {
        if (amount < 0) return;
            
        _currentHealth += amount;
        
        _currentHealth = Mathf.Min(_currentHealth, mMaxHealth);
            
        OnHealthChanged?.Invoke(_currentHealth);
    }
    
    private void Die()
    {
        OnDied?.Invoke();
        
        gameObject.SetActive(false);
            
        Debug.Log($"{gameObject.name}이 사망했어요.");
    }
}