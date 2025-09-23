using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    [Header("Player Movement")]
    [SerializeField] private float mMoveSpeed = 7.0f;
    [SerializeField] private LayerMask mGroundLayer;

    [Header("Player Attack")]
    [SerializeField] private float mAttackCooldown = 0.5f;
    [SerializeField] private float mAttackRadius = 1.5f;
    [SerializeField] private LayerMask mEnemyLayer;

    private Camera _mainCamera;
    private Vector3 _destination;
    private float _lastAttackTime;

    private void Awake()
    {
        _mainCamera = Camera.main;
        if (_mainCamera == null)
        {
            Debug.LogError("메인 카메라가 존재하지 않아요.");
        }
    }
    
    private void Update()
    {
        HandleInput();
        MovePlayer();
    }

    private void HandleInput()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f, mGroundLayer))
            {
                _destination = hit.point;
                _destination.y = transform.position.y;
            }
        }
        else if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            TryAttack();
        }
    }

    private void MovePlayer()
    {
        if (Vector3.Distance(transform.position, _destination) > 0.1f)
        {
            Vector3 direction = (_destination - transform.position).normalized;
            transform.position += direction * (mMoveSpeed * Time.deltaTime);
        }
    }
    
    private void TryAttack()
    {
        if (Time.time < _lastAttackTime + mAttackCooldown)
        {
            return;
        }

        _lastAttackTime = Time.time;
        
        Vector3 attackPosition = transform.position + transform.forward * mAttackRadius;
        
        Collider[] hitEnemies = Physics.OverlapSphere(attackPosition, mAttackRadius, mEnemyLayer);
        
        foreach (Collider enemyCollider in hitEnemies)
        {
            // 추후, 몬스터 데미지 입력 구간
            Debug.Log($"Hit enemy: {enemyCollider.name}");
        }
    }
}
