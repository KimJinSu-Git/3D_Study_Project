using UnityEngine;

public class SpearWeapon : MonoBehaviour, IWeapon
{
    [Header("Weapon Data")]
    [SerializeField] private float mMaxChargeTime = 2.0f;
    [SerializeField] private float mMinChargeTime = 0.5f;
    [SerializeField] private float mBaseDamageMultiplier = 1.0f;
    [SerializeField] private float mMaxChargeMultiplier = 3.0f;
    [SerializeField] private float mAttackCooldown = 1.0f;
    [SerializeField] private LayerMask mEnemyLayer;

    private PlayerController _playerController;
    private StatSystem _statSystem;
    private Transform _playerTransform;

    private bool _isCharging = false;
    private float _chargeStartTime;
    private float _lastAttackTime;

    public bool IsBusy => _isCharging || Time.time < _lastAttackTime + mAttackCooldown;

    public void Initialize(PlayerController controller, StatSystem statSystem)
    {
        _playerController = controller;
        _statSystem = statSystem;
        _playerTransform = controller.transform;
        Debug.Log("SpearWeapon 초기화 완료");
    }

    public void TryAttack(float chargeDuration)
    {
        if (Time.time < _lastAttackTime + mAttackCooldown) return;
        if (_isCharging) return;

        ExecuteChargeAttack(chargeDuration);
        _lastAttackTime = Time.time;
    }

    public void ResetState()
    {
        _isCharging = false;
    }

    private void ExecuteChargeAttack(float chargeDuration)
    {
        float chargeFactor = Mathf.Clamp01((chargeDuration - mMinChargeTime) / (mMaxChargeTime - mMinChargeTime));
        float finalMultiplier = mBaseDamageMultiplier + chargeFactor * (mMaxChargeMultiplier - mBaseDamageMultiplier);

        Debug.Log($"창 차지 공격 실행");

        // TODO: 선형 관통 판정 로직 추가
    }
}