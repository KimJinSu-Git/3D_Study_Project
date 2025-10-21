using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class Enemy : MonoBehaviour
{
    [Header("AI Settings")]
    [SerializeField] protected float mMoveSpeed = 3.0f;
    [SerializeField] protected float mDetectionRange = 10.0f;
    [SerializeField] protected float mAttackRange = 1.5f;
    [SerializeField] protected float mAttackCooldown;
    [SerializeField] protected float mAttackDamage = 10.0f;
    [SerializeField] protected LayerMask mGroundLayer;
    [SerializeField] protected bool mIsBoss = false;
    
    protected Health _health;
    protected HitFeedback _hitFeedback;
    protected KnockbackController _knockbackController;
    protected GameObject _playerObject;
    
    protected Transform _playerTransform;
    protected Health _playerHealth;
    
    protected float _lastAttackTime;
    protected Coroutine _attackCoroutine;

    public enum EnemyState { Idle, Chase, Attack, Guard, GuardBroken, CoolDown  }
    protected EnemyState _currentState = EnemyState.Idle;
    
    protected bool _isVulnerable = false; 
    public bool IsVulnerable => _isVulnerable;
    public bool IsBoss => mIsBoss;
    
    public EnemyState CurrentState => _currentState;
    public Transform PlayerTransform => _playerTransform;

    protected void Start()
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
        _health.OnGuardBroken += OnGuardBrokenHandler;
    }
    
    protected void OnGuardBrokenHandler()
    {
        if (_currentState == EnemyState.GuardBroken) return;
        
        _currentState = EnemyState.GuardBroken;
        _isVulnerable = true;
        
        if (_knockbackController != null)
        {
            _knockbackController.StopMovement();
        }
        
        // TODO: 몬스터 무력화 애니메이션 재생 추가
    
        // 일정 시간 후 무력화 해제 코루틴 시작
        StartCoroutine(RecoverFromGuardBreak(3.0f)); 
    }
    
    protected IEnumerator RecoverFromGuardBreak(float duration)
    {
        Debug.Log($"{gameObject.name} 무력화 시작.");
        
        yield return new WaitForSeconds(duration);
    
        _currentState = EnemyState.Idle; 
        _isVulnerable = false;
        _health.ResetGuardBreak();
        
        Debug.Log($"{gameObject.name} 무력화 회복. 전투 재개.");
    }
    
    protected void OnDied()
    {
        Debug.Log($"{gameObject.name}가 파괴되었습니다.");
        Destroy(gameObject, 1f);
    }
}