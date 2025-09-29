using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("AI Settings")]
    [SerializeField] private float mMoveSpeed = 3.0f;
    [SerializeField] private float mDetectionRange = 10.0f;
    [SerializeField] private float mAttackRange = 1.5f;

    private Transform _playerTransform;
    private enum EnemyState { Idle, Chase, Attack }
    private EnemyState _currentState = EnemyState.Idle;
    private HitFeedback _hitFeedback;

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            _playerTransform = playerObject.transform;
        }
        
        _hitFeedback = GetComponent<HitFeedback>();
        if (_hitFeedback == null)
        {
            Debug.LogError("HitFeedback 컴포넌트를 갖고 있지 않아요");
        }
    }

    private void Update()
    {
        if (_playerTransform == null)
        {
            return;
        }

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

    private void ExecuteStateAction()
    {
        if (_hitFeedback != null && _hitFeedback.IsStunned)
        {
            return; 
        }
        
        switch (_currentState)
        {
            case EnemyState.Idle:
                // 대기 상태, 나중에 Idle 애니메이션이 추가되면 해당 애니메이션을 실행하는 걸로 변경
                break;
            case EnemyState.Chase:
                // 추격 상태, 플레이어를 향해서 이동
                Vector3 direction = (_playerTransform.position - transform.position).normalized;
                transform.position += direction * mMoveSpeed * Time.deltaTime;
                break;
            case EnemyState.Attack:
                // 공격 상태, 플레이어 공격
                Debug.Log("몬스터가 플레이어를 공격합니다!");
                // TODO: 공격 로직 (대미지, 쿨타임 등) 추가
                break;
        }
    }
}