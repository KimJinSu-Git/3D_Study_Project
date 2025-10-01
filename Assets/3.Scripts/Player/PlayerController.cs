using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;


public class PlayerController : MonoBehaviour
{
    private static readonly int MoveSpeed = Animator.StringToHash("MoveSpeed");

    [Header("Player Movement")]
    [SerializeField] private LayerMask mGroundLayer;
    
    [Header("System Settings")]
    [SerializeField] private float mGravity = -20.0f;
    
    [Header("Evasion Settings")]
    [SerializeField] private float mDashForce = 20f;
    [SerializeField] private float mDashDuration = 0.2f;
    [SerializeField] private float mDashStaminaCost = 25f;
    [SerializeField] private float mDashCooldown = 0.5f;
    
    [Header("Weapon Mode")]
    [SerializeField] private float mWeaponSwitchCooldown = 1.0f;
    
    public Vector3 CurrentDestination => _destination; 
    
    private CharacterController _characterController;
    private Camera _mainCamera;
    private StatSystem _statSystem;
    private Stamina _stamina;
    private Health _health;
    private WeaponHandler _weaponHandler;
    private Animator _animator;
    
    private Vector3 _destination;
    private float _currentVerticalVelocity;
    
    private float _lastAttackTime;
    private int _currentComboIndex = 0; 
    private float _lastAttackEndTime; 
    
    private bool _isDashing = false;
    private bool _isMoving = false;
    private float _lastDashTime;

    private void Awake()
    {
        _mainCamera = Camera.main;
        _health = GetComponent<Health>();
        _stamina = GetComponent<Stamina>();
        _characterController = GetComponent<CharacterController>();
        _statSystem = GetComponent<StatSystem>();
        _weaponHandler = GetComponent<WeaponHandler>();
        _animator = GetComponent<Animator>();
        
        if (_mainCamera == null || _health == null || _stamina == null || _characterController == null || _statSystem == null)
        {
            Debug.LogError("필수 컴포넌트를 찾지 못했습니다.");
        }
    }
    
    private void Update()
    {
        HandleInput();
        ApplyGravity();
        MovePlayer();
    }

    private void HandleInput()
    {
        if (_weaponHandler.IsWeaponBusy) return;
        
        // 임시적으로 Q 키로 무기 변경 추가
        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            _weaponHandler.SwitchWeapon();
            return;
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            TryDash();
            return;
        }
        
        if (Mouse.current.leftButton.wasPressedThisFrame)
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
            _weaponHandler.Attack();
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
        if (_isDashing || _weaponHandler.IsWeaponBusy) 
        {
            return; 
        }
        
        if (!_isMoving)
        {
            if (_animator != null)
            {
                _animator.SetFloat(MoveSpeed, 0f); 
            }
            return;
        }
        
        Vector3 currentPosXZ = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 destPosXZ = new Vector3(_destination.x, 0, _destination.z);
        float distanceXZ = Vector3.Distance(currentPosXZ, destPosXZ);
        
        if (distanceXZ <= 0.1f)
        {
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
        
        float currentMoveSpeed = _statSystem.FinalStats.MovementSpeed;
        
        Vector3 horizontalMove = new Vector3(moveDirection.x, 0, moveDirection.z) * currentMoveSpeed;
        Vector3 finalMove = horizontalMove + new Vector3(0, _currentVerticalVelocity, 0);

        _characterController.Move(finalMove * Time.deltaTime);
        
        if (_animator != null)
        {
            _animator.SetFloat(MoveSpeed, currentMoveSpeed);
        }
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
    
    public void StopMovement()
    {
        _isMoving = false;
        _isDashing = false;
    }
}
