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
    [SerializeField] private float mBreakRegenRate = 5f; // 무력화 리젠율(필요 시) TODO:: 리젠이 필요하지 않다면 0으로 하자. 이건 고민 좀.
    [SerializeField] private float mBreakRegenDelay = 3f; // TODO :: 무력화 리젠에 필요한 딜레이 시간

    private float _currentGuardBreakValue;
    private float _lastBreakHitTime;
    
    private float _currentHealth;

    // 이전의 C# 이벤트인 public static event Action 방식과 다르게 인스펙터에 노출이 가능하다는 UnityAction 사용해봄
    public UnityAction<float> OnHealthChanged;
    public UnityAction OnDied;
    public UnityAction OnGuardBroken; // 가드 파괴 시 나중에 할 행동
    
    private void Awake()
    {
        _currentHealth = mMaxHealth;
        _currentGuardBreakValue = mMaxGuardBreakValue;
    }

    private void Update()
    {
        RegenerateGuardBreak();
    }
    
    private void RegenerateGuardBreak()
    {
        if (Time.time < _lastBreakHitTime + mBreakRegenDelay)
        {
            return;
        }

        if (_currentGuardBreakValue < mMaxGuardBreakValue)
        {
            _currentGuardBreakValue += mBreakRegenRate * Time.deltaTime;
            _currentGuardBreakValue = Mathf.Min(_currentGuardBreakValue, mMaxGuardBreakValue);
        
            // TODO: UI에 브레이크 게이지 업데이트 알림
        }
    }
    
    public void ApplyGuardBreak(float power)
    {
        if (_currentGuardBreakValue <= 0 || power <= 0) return;
    
        _currentGuardBreakValue -= power;
        _lastBreakHitTime = Time.time; // 재생 타이머 갱신

        if (_currentGuardBreakValue <= 0)
        {
            _currentGuardBreakValue = 0;
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
    
        // 최종 데미지
        float finalDamage = rawDamage;
    
        if (finalDamage < 0) return;
    
        _currentHealth -= finalDamage;
        
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
        
        ApplyGuardBreak(info.GuardBreakPower);

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