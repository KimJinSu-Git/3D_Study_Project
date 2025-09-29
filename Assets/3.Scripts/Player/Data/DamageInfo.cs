using UnityEngine;

/// <summary>
/// Struct를 선택하게 된 이유
/// DamageInfo는 공격 시 생성된 뒤 바로 사라지는 임시적인 데이터인데,
/// Class(참조 타입) 대신 Struct(값 타입)을 사용하면 힙 메모리 할당을 피하고,
/// 스택 메모리에서 빠르게 처리되므로, GC 부하를 줄여 성능을 최적화할 수 있다고 들었습니다.
/// </summary>
public struct DamageInfo
{
    public float BaseDamage; // 기본 데미지
    public float DamageMultiplier; // 데미지 증가배수
    
    public float StunDuration;     // 경직 시간
    public float KnockbackForce;   // 넉백 힘
    public Vector3 HitDirection;   // 피격 방향 (넉백 계산에 사용)
    
    public bool IsCritical;
    public GameObject Instigator;  // 공격 주체 (장판기 같은 경우 본인이 맞으면 안 되니 추가 요소)
}