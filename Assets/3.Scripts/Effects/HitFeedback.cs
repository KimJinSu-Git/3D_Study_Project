using System.Collections;
using UnityEngine;

public class HitFeedback : MonoBehaviour
    {
        [Header("Visual Feedback")]
        [SerializeField] private Color mHitColor = Color.red;
        [SerializeField] private float mColorDuration = 0.1f;
        
        private Renderer _renderer;
        private Color _originalColor;
        private Coroutine _stunCoroutine;
        
        public bool IsStunned { get; private set; } = false;

        private void Awake()
        {
            _renderer = GetComponentInChildren<SkinnedMeshRenderer>();
            if (_renderer == null)
            {
                _renderer = GetComponent<MeshRenderer>();
                // _renderer = GetComponentInChildren<MeshRenderer>();
            }

            if (_renderer != null)
            {
                _originalColor = _renderer.material.color; 
            }
        }
        
        // 피격 시 호출
        public void ApplyFeedback(DamageInfo info)
        {
            StartCoroutine(FlashColor());
            
            if (_stunCoroutine != null)
            {
                StopCoroutine(_stunCoroutine);
            }
            _stunCoroutine = StartCoroutine(ApplyStun(info.StunDuration));

            // TODO:물리 로직 구현 시 넉백 적용 추가
        }

        // 경직 시간을 적용하는 코루틴
        private IEnumerator ApplyStun(float duration)
        {
            IsStunned = true;
            
            yield return new WaitForSeconds(duration);
            
            IsStunned = false;
        }

        // 색상을 잠시 변경했다가 되돌리는 코루틴
        private IEnumerator FlashColor()
        {
            if (_renderer == null) yield break;

            _renderer.material.color = mHitColor;
            
            yield return new WaitForSeconds(mColorDuration);
            
            _renderer.material.color = _originalColor;
        }
    }