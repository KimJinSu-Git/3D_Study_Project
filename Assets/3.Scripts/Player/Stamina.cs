using UnityEngine;
using UnityEngine.Events;

public class Stamina : MonoBehaviour
{
    [Header("Stamina Settings")]
    [SerializeField] private float mMaxStamina = 100f;
    [SerializeField] private float mRegenRate = 10f;
    [SerializeField] private float mRegenDelay = 1.0f;

    private float _currentStamina;
    private float _lastActionTime;
        
    // 스태미나 변화 이벤트
    public UnityAction<float> OnStaminaChanged;

    public float CurrentStamina => _currentStamina;

    private void Awake()
    {
        _currentStamina = mMaxStamina;
        OnStaminaChanged?.Invoke(_currentStamina / mMaxStamina);
    }

    private void Update()
    {
        Regenerate();
    }

    private void Regenerate()
    {
        if (Time.time < _lastActionTime + mRegenDelay) { return; }

        if (_currentStamina < mMaxStamina)
        {
            _currentStamina += mRegenRate * Time.deltaTime;
            _currentStamina = Mathf.Min(_currentStamina, mMaxStamina);
            
            OnStaminaChanged?.Invoke(_currentStamina / mMaxStamina);
        }
    }

    public bool TryConsume(float amount)
    {
        if (_currentStamina >= amount)
        {
            _currentStamina -= amount;
            _lastActionTime = Time.time;
            OnStaminaChanged?.Invoke(_currentStamina / mMaxStamina);
            return true;
        }
        return false;
    }
}
