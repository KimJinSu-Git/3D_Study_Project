using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;


public class PlayerController : MonoBehaviour
{
    [Header("Player Movement")]
    [SerializeField] private float mMoveSpeed = 7.0f;
    [SerializeField] private LayerMask mGroundLayer;
    
    [Header("System Settings")]
    [SerializeField] private float mGravity = -20.0f;

    [Header("Player Attack")]
    [SerializeField] private PlayerAttackData[] mComboAttackData;
    [SerializeField] private float mComboDelayTime = 0.5f;
    [SerializeField] private LayerMask mEnemyLayer;
    
    [Header("Evasion Settings")]
    [SerializeField] private float mDashForce = 20f;
    [SerializeField] private float mDashDuration = 0.2f;
    [SerializeField] private float mDashStaminaCost = 25f;
    [SerializeField] private float mDashCooldown = 0.5f;
    
    private CharacterController _characterController;
    private Camera _mainCamera;
    private Vector3 _destination;
    private float _currentVerticalVelocity;
    
    private float _lastAttackTime;
    private int _currentComboIndex = 0; 
    private float _lastAttackEndTime; 

    private Stamina _stamina;
    private Health _health;
    private bool _isDashing = false;
    private bool _isMoving = false;
    private float _lastDashTime;

    private void Awake()
    {
        _mainCamera = Camera.main;
        _health = GetComponent<Health>();
        _stamina = GetComponent<Stamina>();
        _characterController = GetComponent<CharacterController>();
        
        if (_mainCamera == null || _health == null)
        {
            Debug.LogError("카메라 또는 체력 컴포넌트를 찾지 못했습니다.");
        }
    }
    
    private void Update()
    {
        HandleInput();
        ApplyGravity();
        MovePlayer();
        UpdateComboState();
    }

    private void UpdateComboState()
    {
        if (_currentComboIndex > 0 && Time.time > _lastAttackEndTime + mComboDelayTime)
        {
            _currentComboIndex = 0;
        }
    }

    private void HandleInput()
    {
        float currentStunDuration = _currentComboIndex == 0 ? 0 : mComboAttackData[_currentComboIndex - 1].mStunDuration;
        bool isStunned = Time.time < _lastAttackTime + currentStunDuration;

        if (Keyboard.current.spaceKey.wasPressedThisFrame && !isStunned)
        {
            TryDash();
            return;
        }
        
        if (Mouse.current.leftButton.wasPressedThisFrame && !isStunned)
        {
            Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f, mGroundLayer))
            {
                _destination = hit.point;
                _isMoving = true;
            }
        }
        else if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            TryComboAttack();
        }
    }
    
    private void TryDash()
    {
        if (_isDashing || Time.time < _lastDashTime + mDashCooldown) return;
        
        if (!_stamina.TryConsume(mDashStaminaCost))
        {
            Debug.Log("스태미나 부족으로 대시 실패함");
            return;
        }
        
        Vector3 dashDirection = transform.forward; 
        
        StartCoroutine(DashSequence(dashDirection));
    }
    
    private IEnumerator DashSequence(Vector3 direction)
    {
        _isDashing = true;
        _lastDashTime = Time.time;
        
        // TODO :: 대시 무적 시작 추가
            
        float startTime = Time.time;
        
        while (Time.time < startTime + mDashDuration)
        {
            transform.position += direction * (mDashForce * Time.deltaTime);
            yield return null;
        }
        
        // TODO :: 대시 무적 종료 추가

        _isDashing = false;
        _destination = transform.position; 
    }

    private void MovePlayer()
    {
        if (_isDashing || (_currentComboIndex > 0 && Time.time < _lastAttackEndTime)) 
        {
            return; 
        }
        
        if (!_isMoving)
        {
            return;
        }
        
        Vector3 currentPosXZ = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 destPosXZ = new Vector3(_destination.x, 0, _destination.z);
        float distanceXZ = Vector3.Distance(currentPosXZ, destPosXZ);
        
        if (distanceXZ <= 0.1f)
        {
            // 앗.. 롤백되는 현상이 플레이어가 문제가 아니라 시네마신 카메라 문제였다니 ㅏㅏㅏㅏㅏㅏㅏㅏㅏㅏㅏㅏㅏ
            _isMoving = false;
            return;
        }
        
        Vector3 direction = (_destination - transform.position);
        Vector3 moveDirection = direction.normalized;

        Vector3 lookDirection = direction;
        lookDirection.y = 0; 
        if (lookDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f); 
        }
        
        Vector3 horizontalMove = new Vector3(moveDirection.x, 0, moveDirection.z) * mMoveSpeed;
        Vector3 finalMove = horizontalMove + new Vector3(0, _currentVerticalVelocity, 0);

        _characterController.Move(finalMove * Time.deltaTime);
    }
    
    private void ApplyGravity()
    {
        if (_characterController.isGrounded)
        {
            _currentVerticalVelocity = -0.5f;
        }
        else
        {
            _currentVerticalVelocity += mGravity * Time.deltaTime;
        }
    }
    
    private void TryComboAttack()
    {
        if (Time.time < _lastAttackEndTime)
        {
            return;
        }
        
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

    private void ExecuteAttack(PlayerAttackData attackData, int comboStep)
    {
        Vector3 lookAtTarget = _destination;
        lookAtTarget.y = transform.position.y;
    
        if (lookAtTarget != transform.position)
        {
            transform.rotation = Quaternion.LookRotation(lookAtTarget - transform.position);
        }
        
        Vector3 attackPosition = transform.position + transform.forward * attackData.mHitRange / 2;
        Collider[] hitTargets = Physics.OverlapSphere(attackPosition, attackData.mHitRange, mEnemyLayer);

        foreach (Collider target in hitTargets)
        {
            Health targetHealth = target.GetComponent<Health>();
            if (targetHealth != null)
            {
                DamageInfo damage = new DamageInfo
                {
                    BaseDamage = 10f,
                    DamageMultiplier = attackData.mDamageMultiplier,
                    StunDuration = attackData.mStunDuration,
                    KnockbackForce = attackData.mKnockbackForce,
                    HitDirection = (target.transform.position - transform.position).normalized,
                    Instigator = gameObject
                };
                
                targetHealth.ApplyDamage(damage);
                // TODO: 경직/넉백 로직 추가
            }
        }
    }
}
