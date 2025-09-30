using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    [SerializeField] private MonoBehaviour mSwordComponent; 
    [SerializeField] private MonoBehaviour mSpearComponent; 
        
    private IWeapon _currentWeapon;
    private IWeapon _sword;
    private IWeapon _spear;

    private PlayerController _playerController;
    private StatSystem _statSystem;

    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
        _statSystem = GetComponent<StatSystem>();

        _sword = mSwordComponent as IWeapon;
        _spear = mSpearComponent as IWeapon;

        if (_sword == null || _spear == null)
        {
            Debug.LogError("무기 정보가 비어 있습니다.");
            return;
        }

        _sword.Initialize(_playerController, _statSystem);
        _spear.Initialize(_playerController, _statSystem);

        _currentWeapon = _sword;
    }

    public void SwitchWeapon()
    {
        if (_currentWeapon.IsBusy) return;
            
        _currentWeapon.ResetState();
            
        _currentWeapon = (_currentWeapon == _sword) ? _spear : _sword;
        Debug.Log($"무기 전환: {_currentWeapon.GetType().Name}");
    }

    public void Attack(float chargeDuration = 0f)
    {
        if (_currentWeapon.IsBusy) return;
            
        _currentWeapon.TryAttack(chargeDuration);
    }
        
    public bool IsWeaponBusy => _currentWeapon.IsBusy;
}