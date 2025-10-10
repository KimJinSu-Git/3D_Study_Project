using UnityEngine;

public class ShieldLogic : MonoBehaviour
{
    [Header("Shield Logic")]
    [SerializeField] private float mGuardDamageMultiplier = 0.1f;
    
    public bool IsGuarding { get; private set; } = false; 
    
    public void StartGuard()
    {
        IsGuarding = true;
        // TODO: 가드 애니메이션/이펙트 활성화
    }

    public void EndGuard()
    {
        IsGuarding = false;
        // TODO: 가드 애니메이션/이펙트 비활성화
    }
    
    public float GetDamageMultiplier(DamageInfo info, Transform monsterTransform)
    {
        Debug.Log("Shield Logic이 달린 오브젝트에요");
        if (!IsGuarding) return 1.0f;

        // 정면 방어 체크: 공격 방향과 몬스터의 각도 비교 (90도 이내)
        Vector3 toAttacker = (info.Instigator.transform.position - monsterTransform.position).normalized;
        float angle = Vector3.Angle(toAttacker, monsterTransform.forward);

        if (angle < 90f) 
        {
            return mGuardDamageMultiplier;
        }
            
        return 1.0f;
    }
}
