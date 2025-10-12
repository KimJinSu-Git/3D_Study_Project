using System.Collections;
using UnityEngine;

public class ShieldmanAI : Enemy 
{
    [Header("Shieldman AI Parameters")]
    [SerializeField] private float mGuardDuration = 3.0f;
    
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
        
        Debug.Log($"{gameObject.name} 상태 : {_currentState}");
        
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
            case EnemyState.CoolDown:
                // 아무것도 하지 않고 대기합니다.
                break; 
            case EnemyState.GuardBroken:
                // 무력화 상태
                break;
        }
    }

    private IEnumerator AttackStanceRoutine()
    {
        Debug.Log($"{gameObject.name} 반격 시작.");
        
        transform.LookAt(new Vector3(_playerTransform.position.x, transform.position.y, _playerTransform.position.z));
        
        // TODO: 애니메이션 컴포넌트를 사용하여 반격 애니메이션 트리거
        
        yield return new WaitForSeconds(2f);
        
        _currentState = EnemyState.CoolDown;
        Debug.Log($"{gameObject.name} 공격 완료. 쿨타임 시작.");
        
        yield return new WaitForSeconds(mAttackCooldown); 
        
        _currentState = EnemyState.Idle; 
        _attackStanceCoroutine = null;
    }
    
    private IEnumerator GuardStanceRoutine()
    {
        Debug.Log($"{gameObject.name} 가드 자세 시작. {mGuardDuration}초 후 반격 준비.");
        yield return new WaitForSeconds(mGuardDuration);

        if (_shieldLogic != null)
        {
            _shieldLogic.EndGuard();
        }
        _currentState = EnemyState.Attack;
        _guardStanceCoroutine = null;
    }
}