using UnityEngine;

public class SwordWeapon : MonoBehaviour, IWeapon
{
    [Header("Weapon Data")]
    [SerializeField] private PlayerAttackData[] mComboAttackData;
    [SerializeField] private float mComboDelayTime = 0.5f;
    [SerializeField] private LayerMask mEnemyLayer;

    private PlayerController _playerController;
    private StatSystem _statSystem;
    private Transform _playerTransform;

    private int _currentComboIndex = 0;
    private float _lastAttackEndTime;
    private float _lastAttackTime;

    public bool IsBusy => _currentComboIndex > 0 && Time.time < _lastAttackEndTime;

    public void Initialize(PlayerController controller, StatSystem statSystem)
    {
        _playerController = controller;
        _statSystem = statSystem;
        _playerTransform = controller.transform;
        Debug.Log("SwordWeapon 초기화 완료");
    }

    public void TryAttack(float chargeDuration = 0f)
    {
        if (Time.time < _lastAttackEndTime) return;

        Debug.Log("공격은 하고 있는건가요 ?");
        
        if (_currentComboIndex > 0 && Time.time > _lastAttackEndTime + mComboDelayTime)
        {
            _currentComboIndex = 0;
        }

        int nextComboIndex = _currentComboIndex;
        if (nextComboIndex >= mComboAttackData.Length)
        {
            nextComboIndex = 0;
        }

        PlayerAttackData currentAttackData = mComboAttackData[nextComboIndex];
        ExecuteAttack(currentAttackData, nextComboIndex);

        _currentComboIndex = nextComboIndex + 1;
        _lastAttackTime = Time.time;
        _lastAttackEndTime = Time.time + currentAttackData.mStunDuration;
    }

    public void ResetState()
    {
        _currentComboIndex = 0;
        _lastAttackEndTime = 0;
    }

    private void ExecuteAttack(PlayerAttackData attackData, int comboStep)
    {
        Vector3 attackPosition = _playerTransform.position + _playerTransform.forward * attackData.mHitRange / 2;
        Collider[] hitTargets = Physics.OverlapSphere(attackPosition, attackData.mHitRange, mEnemyLayer);

        foreach (Collider target in hitTargets)
        {
            Health targetHealth = target.GetComponent<Health>();
            if (targetHealth != null)
            {
                DamageInfo damage = new DamageInfo
                {
                    BaseDamage = _statSystem.FinalStats.BaseDamage,
                    DamageMultiplier = attackData.mDamageMultiplier,
                    StunDuration = attackData.mStunDuration,
                    KnockbackForce = attackData.mKnockbackForce,
                    HitDirection = (target.transform.position - _playerTransform.position).normalized,
                    Instigator = _playerTransform.gameObject
                };

                targetHealth.ApplyDamage(damage);
            }
        }
    }
}