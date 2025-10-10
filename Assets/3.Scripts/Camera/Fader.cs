using System.Collections;
using UnityEngine;

public class Fader : MonoBehaviour
{
    private const float FADE_DURATION = 0.3f;
        
    private Renderer _renderer;
    private Color _originalColor;
    private Coroutine _fadeCoroutine;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        if (_renderer != null)
        {
            _originalColor = _renderer.material.color;
            Debug.Log("renderer가 있습니다");
        }
    }

    public void FadeOut()
    {
        if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
        _fadeCoroutine = StartCoroutine(FadeSequence(_originalColor.a, 0.2f)); // 20%
    }

    public void FadeIn()
    {
        if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);
        _fadeCoroutine = StartCoroutine(FadeSequence(_originalColor.a, 1.0f)); // 100%
    }

    private IEnumerator FadeSequence(float startAlpha, float targetAlpha)
    {
        if (_renderer == null) yield break;
        
        Debug.Log($"FadeSequence 들어옴 => 투명도 값 :{targetAlpha}");

        float time = 0;
        Color currentColor = _renderer.material.color;
        Color targetColor = new Color(currentColor.r, currentColor.g, currentColor.b, targetAlpha);

        while (time < FADE_DURATION)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, time / FADE_DURATION);
            _renderer.material.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
            yield return null;
        }
            
        _renderer.material.color = targetColor;
    }
}