using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponHandler : MonoBehaviour
{
    private static readonly int IsSwordMode = Animator.StringToHash("IsSwordMode");
    
    [SerializeField] private MonoBehaviour mSwordComponent; 
    [SerializeField] private MonoBehaviour mSpearComponent; 
        
    private IWeapon _currentWeapon;
    private IWeapon _sword;
    private IWeapon _spear;

    private PlayerController _playerController;
    private StatSystem _statSystem;
    private Animator _animator;
    
    private bool _isChargingInputPressed = false;

    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
        _statSystem = GetComponent<StatSystem>();

        _sword = mSwordComponent as IWeapon;
        _spear = mSpearComponent as IWeapon;
        
        _animator = GetComponentInChildren<Animator>();

        if (_animator == null) Debug.LogError("애니메이터가 없어요");

        if (_sword == null || _spear == null)
        {
            Debug.LogError("무기 정보가 비어 있습니다.");
            return;
        }

        _sword.Initialize(_playerController, _statSystem, _animator);
        _spear.Initialize(_playerController, _statSystem, _animator);

        _currentWeapon = _sword;
        
        _animator.SetBool(IsSwordMode, true);
    }

    private void Update()
    {
        if (_currentWeapon != _spear) return; 
        
        if (!_currentWeapon.IsBusy)
        {
            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                _isChargingInputPressed = true;
                
                (_spear as SpearWeapon)?.StartCharge(); 
            }
        }
        
        if (Mouse.current.rightButton.wasReleasedThisFrame && _isChargingInputPressed)
        {
            _isChargingInputPressed = false;
            (_spear as SpearWeapon)?.ReleaseCharge();
        }
    }

    public void SwitchWeapon()
    {
        if (_currentWeapon.IsBusy) return;
            
        _currentWeapon.ResetState();
            
        _currentWeapon = (_currentWeapon == _sword) ? _spear : _sword;
        
        bool isSwordMode = (_currentWeapon == _sword);
        if (_animator != null)
        {
            _animator.SetBool(IsSwordMode, isSwordMode);
        }
        
        Debug.Log($"무기 전환: {_currentWeapon.GetType().Name}");
    }

    public void Attack(float chargeDuration = 0f)
    {
        if (_currentWeapon.IsBusy) return;
            
        if (_currentWeapon == _sword)
        {
            _currentWeapon.TryAttack(chargeDuration);
        }
    }
        
    public bool IsWeaponBusy => _currentWeapon.IsBusy;
}