using UnityEngine;

[CreateAssetMenu(fileName = "AttackData", menuName = "Bird/Attack Data", order = 1)]
public class PlayerAttackData : ScriptableObject
{
    public float mHitAngle = 60f;
    public float mHitRange = 1.5f;
    public float mDamageMultiplier = 1.0f;
    public float mStunDuration = 0.5f;
    public float mKnockbackForce = 0f; // 넉백 힘
}
