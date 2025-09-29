using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    [SerializeField] private float mMoveSpeed = 2f;
    [SerializeField] private float mDisappearTime = 0.6f;
    [SerializeField] private float mUpwardFloat = 1.0f;

    private TextMeshPro _textMesh;
    private float _timeElapsed;
    private Color _textColor;

    private void Awake()
    {
        _textMesh = GetComponent<TextMeshPro>();
        _textColor = _textMesh.color;
    }

    public void Setup(float damageAmount)
    {
        _textMesh.SetText(damageAmount.ToString("F0"));
        _timeElapsed = 0f;
        
        Vector3 randomOffset = new Vector3(Random.Range(-0.5f, 0.5f), 0, Random.Range(-0.5f, 0.5f));
        transform.position += randomOffset;
    }

    private void Update()
    {
        transform.position += new Vector3(0, mMoveSpeed * Time.deltaTime * mUpwardFloat, 0);
            
        _timeElapsed += Time.deltaTime;

        float alpha = 1f - (_timeElapsed / mDisappearTime);
        _textMesh.color = new Color(_textColor.r, _textColor.g, _textColor.b, alpha);

        // TODO :: 나중에 오브젝트 풀링으로 변경 예정
        if (_timeElapsed > mDisappearTime)
        {
            Destroy(gameObject);
        }
    }
}