using System.Collections;
using UnityEngine;

public class ShieldmanAI : Enemy 
{
    private ShieldLogic _shieldLogic;
    private Coroutine _attackStanceCoroutine;
    private Coroutine _guardStanceCoroutine;
    
    private new void Start()
    {
        base.Start();
        
        _shieldLogic = GetComponent<ShieldLogic>();
        if (_shieldLogic == null) Debug.LogError("shieldLogic 컴포넌트가 없어요");
    }

    private void Update()
    {
        if (_playerTransform == null || _playerHealth == null) return;
        
        Debug.Log($"{_currentState}");
        
        if ((_hitFeedback != null && _hitFeedback.IsStunned) || (_knockbackController != null && _knockbackController.IsKnockedBack) || 
            (_currentState == EnemyState.GuardBroken)) 
        { 
            if (_shieldLogic != null && _shieldLogic.IsGuarding)
            {
                _shieldLogic.EndGuard();
            }
            return; 
        }
        
        if (_attackStanceCoroutine != null || _guardStanceCoroutine != null) return;
        
        float distanceToPlayer = Vector3.Distance(transform.position, _playerTransform.position);

        if (distanceToPlayer <= mAttackRange)
        {
            if (_currentState != EnemyState.Guard && _currentState != EnemyState.Attack)
            {
                _currentState = EnemyState.Guard; 
            }
        }
        else if (distanceToPlayer <= mDetectionRange)
        {
            if (_currentState != EnemyState.Chase)
            {
                _currentState = EnemyState.Chase;
            }
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
                transform.position += direction * (mMoveSpeed * Time.deltaTime);
                if (_shieldLogic != null && _shieldLogic.IsGuarding) _shieldLogic.EndGuard();
                break;
            case EnemyState.Guard:
                if (!_shieldLogic.IsGuarding)
                {
                    _shieldLogic.StartGuard();
                    _attackCoroutine = StartCoroutine(GuardStanceRoutine());
                }
                transform.LookAt(new Vector3(_playerTransform.position.x, transform.position.y, _playerTransform.position.z));
                break;
            case EnemyState.Attack:
                if (_attackStanceCoroutine == null)
                {
                    Debug.Log("제대로 공격 루틴 시작");
                    _attackStanceCoroutine = StartCoroutine(AttackStanceRoutine());
                }
                break;
            case EnemyState.Idle:
                // Idle 로직
                break;
            case EnemyState.GuardBroken:
                // 무력화 상태
                break;
        }
    }

    private IEnumerator AttackStanceRoutine()
    {
        Debug.Log("실드맨이 플레이어 공격 했어요");
        
        yield return new WaitForSeconds(2f);
        
        _currentState = EnemyState.Idle; 
        _attackStanceCoroutine = null;
    }
    
    private IEnumerator GuardStanceRoutine()
    {
        yield return new WaitForSeconds(3.0f);

        _shieldLogic.EndGuard();
        _currentState = EnemyState.Attack;
        _guardStanceCoroutine = null;
    }
}