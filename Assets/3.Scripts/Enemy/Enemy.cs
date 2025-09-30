using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("AI Settings")]
    [SerializeField] private float mMoveSpeed = 3.0f;
    [SerializeField] private float mDetectionRange = 10.0f;
    [SerializeField] private float mAttackRange = 1.5f;
    [SerializeField] private LayerMask mGroundLayer;
    
    [Header("Attack Settings")]
    [SerializeField] private float mAttackCooldown = 1.0f;
    [SerializeField] private float mAttackDamage = 10.0f;

    private Transform _playerTransform;
    private Health _playerHealth;
    private Health _health;
    private HitFeedback _hitFeedback;
    private KnockbackController _knockbackController;
    private GameObject _playerObject;
    
    private float _lastAttackTime;
    private Coroutine _attackCoroutine;
    
    private enum EnemyState { Idle, Chase, Attack }
    private EnemyState _currentState = EnemyState.Idle;

    private void Start()
    {
        _health = GetComponent<Health>();
        _hitFeedback = GetComponent<HitFeedback>();
        _knockbackController = GetComponent<KnockbackController>();
        
        _playerObject = GameObject.FindGameObjectWithTag("Player");
        
        if (_playerObject != null)
        {
            _playerTransform = _playerObject.transform;
            _playerHealth = _playerObject.GetComponent<Health>();
        }
        if (_hitFeedback == null) { Debug.LogWarning("HitFeedback 컴포넌트를 갖고 있지 않아요"); }
        if (_knockbackController == null) { Debug.LogWarning("KnockbackController 컴포넌트를 갖고 있지 않아요"); }
        if (_health == null) { Debug.LogWarning("Health 컴포넌트를 갖고 있지 않아요"); }
        if (_playerHealth == null) { Debug.LogWarning("Player가 Health 컴포넌트를 갖고 있지 않아요"); }
        
        _health.OnDied += OnDied;
    }

    private void Update()
    {
        if (_playerTransform == null || _playerHealth == null) return;
        
        if ((_hitFeedback != null && _hitFeedback.IsStunned) || (_knockbackController != null && _knockbackController.IsKnockedBack)) { return; }

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
        
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1f, mGroundLayer))
        {
            transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
        }
    }

    private void ExecuteStateAction()
    {
        switch (_currentState)
        {
            case EnemyState.Idle:
                // 대기 상태, 나중에 Idle 애니메이션이 추가되면 해당 애니메이션을 실행하는 걸로 변경
                break;
            case EnemyState.Chase:
                // 추격 상태, 플레이어를 향해서 이동
                Vector3 direction = (_playerTransform.position - transform.position).normalized;
                transform.position += direction * (mMoveSpeed * Time.deltaTime);
                break;
            case EnemyState.Attack:
                // 공격 상태, 플레이어 공격
                Debug.Log("몬스터가 플레이어를 공격합니다!");
                // TODO: 공격 로직 (대미지, 쿨타임 등) 추가
                if (Time.time >= _lastAttackTime + mAttackCooldown && _attackCoroutine == null)
                {
                    _attackCoroutine = StartCoroutine(AttackSequence());
                }
                break;
        }
    }
    
    private IEnumerator AttackSequence()
    {
        transform.LookAt(new Vector3(_playerTransform.position.x, transform.position.y, _playerTransform.position.z));

        TryAttack();
        yield return new WaitForSeconds(0.6f);

        TryAttack(); 
        yield return new WaitForSeconds(0.8f);
            
        _lastAttackTime = Time.time;
        _attackCoroutine = null;
    }
    
    private void TryAttack()
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
    
    private void OnDied()
    {
        Debug.Log($"{gameObject.name}가 파괴되었습니다.");
        Destroy(gameObject, 1f);
    }
}