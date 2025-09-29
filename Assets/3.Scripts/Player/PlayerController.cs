using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;


public class PlayerController : MonoBehaviour
    {
        [Header("Player Movement")]
        [SerializeField] private float mMoveSpeed = 7.0f;
        [SerializeField] private LayerMask mGroundLayer;

        [Header("Player Attack")]
        [SerializeField] private PlayerAttackData[] mComboAttackData;
        [SerializeField] private float mComboDelayTime = 0.5f;
        [SerializeField] private LayerMask mEnemyLayer;
        
        private Camera _mainCamera;
        private Vector3 _destination;
        private float _lastAttackTime;
        
        private int _currentComboIndex = 0; 
        private float _lastAttackEndTime; 

        private Health _health; 

        private void Awake()
        {
            _mainCamera = Camera.main;
            _health = GetComponent<Health>();
            
            if (_mainCamera == null || _health == null)
            {
                Debug.LogError("카메라 또는 체력 컴포넌트를 찾지 못했습니다.");
            }
        }
        
        private void Update()
        {
            HandleInput();
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

            if (Mouse.current.leftButton.wasPressedThisFrame && !isStunned)
            {
                Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 100f, mGroundLayer))
                {
                    _destination = hit.point;
                }
            }
            else if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                TryComboAttack();
            }
        }

        private void MovePlayer()
        {
            if (Vector3.Distance(transform.position, _destination) > 0.1f)
            {
                Vector3 direction = (_destination - transform.position).normalized;
                transform.position += direction * (mMoveSpeed * Time.deltaTime);
                
                if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1f, mGroundLayer))
                {
                    transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
                }
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
            if (_destination != transform.position)
            {
                transform.LookAt(_destination);
            }
            
            Vector3 attackPosition = transform.position + transform.forward * attackData.mHitRange / 2;
            Collider[] hitTargets = Physics.OverlapSphere(attackPosition, attackData.mHitRange, mEnemyLayer);

            foreach (Collider target in hitTargets)
            {
                Health targetHealth = target.GetComponent<Health>();
                if (targetHealth != null)
                {
                    targetHealth.TakeDamage(10f * attackData.mDamageMultiplier);
                    Debug.Log($"{10f * attackData.mDamageMultiplier} 데미지가 들어갔습니다");
                    // TODO: P1-4 에서 경직/넉백 로직 추가
                }
            }
        }
    }
