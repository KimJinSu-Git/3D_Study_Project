using System;
using System.Collections;
using UnityEngine;

public class RunnerAI : Enemy 
{
    [Header("Runner AI")]
    [SerializeField] private float mRushSpeedMultiplier = 2.0f;
    [SerializeField] private float mRushDuration = 1.0f;
    [SerializeField] private float mAttackDelayBeforeRush = 0.5f;

    private Coroutine _currentAttackRoutine;
    private float _nextActionTime;
    private bool _hasHitDuringRush = false;
    
    private void Update()
    {
        Debug.Log($"_playerHealth 상태 : {_playerHealth}");
        if (_playerTransform == null || _playerHealth == null) return;
        
        if ((_hitFeedback != null && _hitFeedback.IsStunned) || (_knockbackController != null && _knockbackController.IsKnockedBack) ||
            (_currentState == EnemyState.GuardBroken) || _currentAttackRoutine != null)
        { 
            return; 
        }
        
        Debug.Log($"{gameObject.name} 상태 : {_currentState}");

        float distanceToPlayer = Vector3.Distance(transform.position, _playerTransform.position);

        if (distanceToPlayer <= mAttackRange)
        {
            _currentState = EnemyState.Attack;
        }
        else if (distanceToPlayer <= mDetectionRange)
        {
            _currentState = EnemyState.Chase;
        }
        else
        {
            _currentState = EnemyState.Idle;
        }

        ExecuteStateAction();
    }

    protected void ExecuteStateAction()
    {
        switch (_currentState)
        {
            case EnemyState.Chase:
                Vector3 direction = (_playerTransform.position - transform.position).normalized;
                transform.position += direction * (mMoveSpeed * mRushSpeedMultiplier * Time.deltaTime);
                break;
                
            case EnemyState.Attack:
                if (Time.time >= _nextActionTime)
                {
                    _currentAttackRoutine = StartCoroutine(AttackSequence());
                }
                break;
            case EnemyState.Idle:
                break;
            case EnemyState.CoolDown:
                break;
            case EnemyState.Guard:
                break;
            case EnemyState.GuardBroken:
                break;
        }
    }
    
    private IEnumerator AttackSequence()
    {
        _hasHitDuringRush = false;
        
        transform.LookAt(new Vector3(_playerTransform.position.x, transform.position.y, _playerTransform.position.z));
        yield return new WaitForSeconds(mAttackDelayBeforeRush); 
        
        float rushEndTime = Time.time + mRushDuration;
        Vector3 rushDirection = transform.forward;
        
        while (Time.time < rushEndTime)
        {
            transform.position += rushDirection * (mMoveSpeed * mRushSpeedMultiplier * Time.deltaTime);
            
            TryAttack();
            
            // TODO :: 돌진 중일 때 플레이어와 충돌했을 때 돌진을 중단 시키고 싶다면 주석 해제. 어떤 방식의 몹일지는 천천히 생각
            // if (_hasHitDuringRush)
            // {
            //     break;
            // }
            
            yield return null;
        }
        
        _nextActionTime = Time.time + mAttackCooldown;
        _currentAttackRoutine = null;
        _currentState = EnemyState.Chase;
    }
    
    private void TryAttack()
    {
        if (_hasHitDuringRush) return;
        
        float closeAttackRadius = 0.5f;
    
        Collider[] hitPlayers = Physics.OverlapSphere(transform.position, closeAttackRadius, 1 << _playerTransform.gameObject.layer);

        Debug.DrawRay(transform.position, Vector3.up * 3f, Color.red, 0.1f);
    
        if (hitPlayers.Length > 0)
        {
            _hasHitDuringRush = true;

            _playerHealth.ApplyDamage(new DamageInfo
            {
                BaseDamage = mAttackDamage,
                DamageMultiplier = 1f,
                StunDuration = 0.1f, 
                KnockbackForce = 5f,
                HitDirection = (transform.position - _playerTransform.position).normalized * -1,
                Instigator = gameObject
            });
            Debug.Log("러너 몬스터가 돌진 중 플레이어에게 1회 히트!");
        }
    }
}
