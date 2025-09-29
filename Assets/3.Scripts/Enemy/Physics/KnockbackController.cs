using System.Collections;
using UnityEngine;

public class KnockbackController : MonoBehaviour
{
    [SerializeField] private float mRecoveryDuration = 0.5f;

    private Rigidbody _rigidbody;
    private Coroutine _knockbackCoroutine;
    
    public bool IsKnockedBack { get; private set; } = false;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        if (_rigidbody == null)
        {
            Debug.LogError($"rigidbody 컴포넌트를 갖고 있지 않아요");
        }
    }

    public void ApplyKnockback(Vector3 direction, float force)
    {
        if (_rigidbody == null || force <= 0) return;

        if (_knockbackCoroutine != null)
        {
            StopCoroutine(_knockbackCoroutine);
        }

        _knockbackCoroutine = StartCoroutine(KnockbackSequence(direction, force));
    }

    private IEnumerator KnockbackSequence(Vector3 direction, float force)
    {
        IsKnockedBack = true;
        _rigidbody.isKinematic = false;
        
        Vector3 knockbackVector = new Vector3(direction.x, 0, direction.z).normalized * force;
        _rigidbody.linearVelocity = Vector3.zero;
        _rigidbody.AddForce(knockbackVector, ForceMode.VelocityChange);
        
        yield return new WaitForSeconds(mRecoveryDuration);
        
        _rigidbody.linearVelocity = Vector3.zero;
        _rigidbody.isKinematic = true;
        IsKnockedBack = false;
    }
}