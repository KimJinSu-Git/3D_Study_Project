using System.Collections.Generic;
using UnityEngine;

public class SpearWeapon : MonoBehaviour, IWeapon
{
    private static readonly int AttackTrigger = Animator.StringToHash("AttackTrigger");
    private static readonly int ComboIndex = Animator.StringToHash("ComboIndex");

    [Header("Weapon Data")]
    [SerializeField] private float mMaxChargeTime = 2.0f;
    [SerializeField] private float mMinChargeTime = 0.5f;
    [SerializeField] private float mChargeAnimSpeed = 0.15f;
    [SerializeField] private float mBaseDamageMultiplier = 1.0f;
    [SerializeField] private float mMaxChargeMultiplier = 3.0f;
    [SerializeField] private float mAttackCooldown = 1.0f;
    [SerializeField] private LayerMask mEnemyLayer;

    private PlayerController _playerController;
    private StatSystem _statSystem;
    private Transform _playerTransform;
    private Animator _animator;

    private bool _isCharging = false;
    private float _chargeStartTime;
    private float _lastAttackTime;

    public bool IsBusy => Time.time < _lastAttackTime + mAttackCooldown;

    public void Initialize(PlayerController controller, StatSystem statSystem, Animator animator)
    {
        _playerController = controller;
        _statSystem = statSystem;
        _playerTransform = controller.transform;
        _animator = animator;
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
        _lastAttackTime = Time.time;
    
        float chargeFactor = Mathf.Clamp01((chargeDuration - mMinChargeTime) / (mMaxChargeTime - mMinChargeTime));
        float finalMultiplier = mBaseDamageMultiplier + chargeFactor * (mMaxChargeMultiplier - mBaseDamageMultiplier);
    
        float spearLength = 3.0f;
        float finalRange = spearLength * (1.0f + chargeFactor * 0.5f);
    
        Vector3 halfExtents = new Vector3(0.1f, 0.5f, 0.1f);
    
        RaycastHit[] hits = Physics.BoxCastAll(
            _playerTransform.position + _playerTransform.up * 0.5f,
            halfExtents, 
            _playerTransform.forward, 
            _playerTransform.rotation, 
            finalRange, 
            mEnemyLayer
        );

        HashSet<Collider> processedTargets = new HashSet<Collider>();

        foreach (RaycastHit hit in hits)
        {
            Collider targetCollider = hit.collider;
            if (processedTargets.Contains(targetCollider)) continue;

            processedTargets.Add(targetCollider);

            Health targetHealth = targetCollider.GetComponent<Health>();
            if (targetHealth != null)
            {
                DamageInfo damage = new DamageInfo
                {
                    BaseDamage = _statSystem.FinalStats.BaseDamage,
                    DamageMultiplier = finalMultiplier,
                    StunDuration = 0.2f,
                    KnockbackForce = 5f * chargeFactor,
                    HitDirection = _playerTransform.forward,
                    Instigator = _playerTransform.gameObject
                };

                targetHealth.ApplyDamage(damage);
                Debug.Log($"창 관통 히트: {targetCollider.name} | 배율: {finalMultiplier:F2}");
            }
        }
    }
    
    public void StartCharge()
    {
        if (IsBusy) return;
    
        _isCharging = true;
        _chargeStartTime = Time.time;
    
        if (_animator != null)
        {
            _animator.SetInteger(ComboIndex, 1);
            _animator.SetTrigger(AttackTrigger); 
            _animator.speed = mChargeAnimSpeed;
        }
    
        _playerController.StopMovement();
        Debug.Log("창 차징 중... 애니메이션 느리게");
    }
    
    public void ReleaseCharge()
    {
        if (!_isCharging) return;

        _isCharging = false;
        float chargeDuration = Time.time - _chargeStartTime;
    
        if (_animator != null)
        {
            _animator.speed = 1.0f; 
        }

        ExecuteChargeAttack(chargeDuration);
        Debug.Log("창 공격 !");
    }
}