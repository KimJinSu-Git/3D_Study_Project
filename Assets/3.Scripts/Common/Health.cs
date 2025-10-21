using System;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField] private float mMaxHealth = 100f;
    
    [Header("UI Feedback")]
    [SerializeField] private GameObject mDamagePopupPrefab;
    
    [Header("Guard Break")]
    [SerializeField] private float mMaxGuardBreakValue = 100f; // 최대 무력화
    
    [Header("Vulnerability Settings")]
    [SerializeField] private float mVulnerableDamageMultiplier = 2.0f;

    private float _currentGuardBreakValue;
    
    private float _currentHealth;

    // 이전의 C# 이벤트인 public static event Action 방식과 다르게 인스펙터에 노출이 가능하다는 UnityAction 사용해봄
    public UnityAction<float> OnHealthChanged;
    public UnityAction OnDied;
    public UnityAction OnGuardBroken; // 가드 파괴 시 나중에 할 행동
    public UnityAction<float> OnGuardBreakChanged;
    
    public float MaxHealth => mMaxHealth;
    public float MaxGuardBreakValue => mMaxGuardBreakValue;
    
    private void Awake()
    {
        _currentHealth = mMaxHealth;
        _currentGuardBreakValue = mMaxGuardBreakValue;
    }
    
    public void ResetGuardBreak()
    {
        _currentGuardBreakValue = mMaxGuardBreakValue;
        // TODO: UI에 브레이크 게이지 Max로 복구 알림
        OnGuardBreakChanged?.Invoke(_currentGuardBreakValue);
        Debug.Log($"{gameObject.name}의 가드 브레이크 수치 초기화됨.");
    }
    
    public void ApplyGuardBreak(float power)
    {
        if (_currentGuardBreakValue <= 0 || power <= 0) return;
    
        _currentGuardBreakValue -= power;
        _currentGuardBreakValue = Mathf.Max(_currentGuardBreakValue, 0);

        OnGuardBreakChanged?.Invoke(_currentGuardBreakValue);
        
        if (_currentGuardBreakValue <= 0)
        {
            OnGuardBroken?.Invoke(); // 무력화 이벤트 발생
            Debug.Log($"{gameObject.name}의 무력화 발생!");
        }
    }

    public void SetMaxHealth(float newMaxHealth)
    {
        mMaxHealth = newMaxHealth;
        _currentHealth = Mathf.Min(_currentHealth, mMaxHealth);
        OnHealthChanged?.Invoke(_currentHealth / mMaxHealth);
    }
    
    public void ApplyDamage(DamageInfo info)
    {
        float rawDamage = info.BaseDamage * info.DamageMultiplier;
    
        // TODO: 방어력/취약/저항 계산 로직 추가
    
        // TODO: 치명타 계산 로직 추가
    
        float finalDamage = rawDamage;
        float finalBreakPower = info.GuardBreakPower;
        
        ShieldLogic shieldLogic = GetComponent<ShieldLogic>();
        if (shieldLogic != null)
        {
            // ShieldLogic에게 최종 피해 배율을 요청
            float guardMultiplier = shieldLogic.GetDamageMultiplier(info, transform);
        
            finalDamage *= guardMultiplier;
            finalBreakPower *= guardMultiplier;
        
            if (guardMultiplier < 1.0f)
            {
                Debug.Log($"가드 성공! 피해 및 브레이크 수치 {100f - (guardMultiplier * 100f)}% 감소.");
            }
        }
        
        if (finalDamage < 0) return;
        
        Enemy enemy = GetComponent<Enemy>();
        if (enemy != null && enemy.IsVulnerable)
        {
            finalDamage *= mVulnerableDamageMultiplier;
            finalBreakPower *= mVulnerableDamageMultiplier;
            Debug.Log("무력화! 피해 증폭.");
        }
    
        _currentHealth -= finalDamage;
        ApplyGuardBreak(info.GuardBreakPower);
        
        if (finalDamage > 0 && mDamagePopupPrefab != null)
        {
            GameObject popupObject = Instantiate(mDamagePopupPrefab, transform.position + Vector3.up * 1f, Quaternion.identity);
            popupObject.GetComponent<DamagePopup>()?.Setup(finalDamage);
        }
    
        // --- 피격 피드백 및 상태 이상 처리 ---
    
        HitFeedback feedback = GetComponent<HitFeedback>();
        if (feedback != null)
        {
            feedback.ApplyFeedback(info);
        }
        
        if (info.KnockbackForce > 0)
        {
            KnockbackController knockbackController = GetComponent<KnockbackController>();
            if (knockbackController != null)
            {
                knockbackController.ApplyKnockback(info.HitDirection, info.KnockbackForce);
            }
        }

        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            Die();
        }
    
        OnHealthChanged?.Invoke(_currentHealth);
    
        Debug.Log($"{gameObject.name}가 {finalDamage} 데미지를 입었습니다. 남은 체력 : {_currentHealth} , 무력화 수치 : {_currentGuardBreakValue}");
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