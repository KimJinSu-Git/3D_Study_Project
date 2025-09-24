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
    
    public void TakeDamage(float damage)
    {
        if (damage < 0) return;
            
        _currentHealth -= damage;
        
        OnHealthChanged?.Invoke(_currentHealth);
        
        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            Die();
        }
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