using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBarUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Slider mHealthSlider;
    [SerializeField][CanBeNull] private Slider mGuardBreakSlider; // 무력화 게이지는 몬스터 유형에 따라 있을 수도 있고, 없을 수도 있습니다.
    [SerializeField] private Canvas mWorldCanvas;
    
    private Health _health;
    private Transform _cameraTransform;

    private void Start()
    {
        _health = GetComponentInParent<Health>();
        if (Camera.main != null) _cameraTransform = Camera.main.transform;

        if (_health == null)
        {
            Debug.LogError("health 컴포넌트가 필요해");
            return;
        }

        _health.OnHealthChanged += UpdateHealthBar;
        _health.OnGuardBreakChanged += UpdateGuardBreakBar;

        // 초기 값 설정
        UpdateHealthBar(_health.MaxHealth);
        UpdateGuardBreakBar(_health.MaxGuardBreakValue);
    }

    private void LateUpdate()
    {
        if (_cameraTransform != null && mWorldCanvas != null)
        {
            mWorldCanvas.transform.rotation = _cameraTransform.rotation;
        }
    }

    private void UpdateHealthBar(float currentHealth)
    {
        if (mHealthSlider != null)
        {
            mHealthSlider.maxValue = _health.MaxHealth;
            mHealthSlider.value = currentHealth;
        }
    }

    private void UpdateGuardBreakBar(float currentGuardBreak)
    {
        if (mGuardBreakSlider != null)
        {
            // GuardBreak 슬라이더는 역순으로 동작 (Full이 Good, Empty가 Break)
            mGuardBreakSlider.maxValue = _health.MaxGuardBreakValue;
            mGuardBreakSlider.value = currentGuardBreak;
            
            // TODO: 무력화 시 (0일 때) 슬라이더 색상 변경 효과 추가
        }
    }
    
    private void OnDestroy()
    {
        if (_health != null)
        {
            _health.OnHealthChanged -= UpdateHealthBar;
            _health.OnGuardBreakChanged -= UpdateGuardBreakBar;
        }
    }
}