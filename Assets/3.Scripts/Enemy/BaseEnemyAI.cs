using System.Collections;
using UnityEngine;

public class BaseEnemyAI : Enemy 
{
    private void Update()
    {
        if (_playerTransform == null || _playerHealth == null) return;
        
        if ((_hitFeedback != null && _hitFeedback.IsStunned) || (_knockbackController != null && _knockbackController.IsKnockedBack) || 
            (_currentState == EnemyState.GuardBroken)) { return; }

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
            case EnemyState.Idle:
                // 대기 상태
                break;
            case EnemyState.Chase:
                Vector3 direction = (_playerTransform.position - transform.position).normalized;
                transform.position += direction * (mMoveSpeed * Time.deltaTime);
                break;
            case EnemyState.Attack:
                if (Time.time >= _lastAttackTime + mAttackCooldown && _attackCoroutine == null)
                {
                    _attackCoroutine = StartCoroutine(AttackSequence());
                }
                break;
            case EnemyState.Guard:
                break;
            case EnemyState.GuardBroken:
                break;
        }
    }
    
    protected IEnumerator AttackSequence()
    {
        transform.LookAt(new Vector3(_playerTransform.position.x, transform.position.y, _playerTransform.position.z));
        TryAttack();
        yield return new WaitForSeconds(0.3f);
        TryAttack(); 
        yield return new WaitForSeconds(0.4f);
        _lastAttackTime = Time.time;
        _attackCoroutine = null;
    }
    
    protected void TryAttack()
    {
        if (Vector3.Distance(transform.position, _playerTransform.position) > mAttackRange * 1.1f) { return; }

        _playerHealth.ApplyDamage(new DamageInfo
        {
            BaseDamage = mAttackDamage,
            DamageMultiplier = 1f,
            StunDuration = 0.1f, 
            KnockbackForce = 0f,
            HitDirection = (transform.position - _playerTransform.position).normalized * -1,
            Instigator = gameObject
        });
    }
}
