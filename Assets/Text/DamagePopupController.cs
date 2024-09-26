using TMPro;
using UnityEngine;

public class DamagePopupController : MonoBehaviour
{
    public float timeToLive = 0.5f;
    public float riseSpeed = 50f;

    private Vector3 riseDirection = new(0, 1, 0);
    private float timeElapsed = 0f;

    public string text;
    public Color textColor;
    public Vector3 textScale;

    [SerializeField]
    private RectTransform rectTransform;

    [SerializeField]
    private TextMeshProUGUI textMesh;

    private void Start()
    {
        textMesh.text = text;
        textMesh.color = textColor;
        rectTransform.localScale = textScale;
    }

    private void FixedUpdate()
    {
        timeElapsed += Time.fixedDeltaTime;
        if (timeElapsed > timeToLive)
        {
            Destroy(gameObject);
        }
        else
        {
            rectTransform.position += riseSpeed * Time.fixedDeltaTime * riseDirection;
            textMesh.color -= new Color(0, 0, 0, Time.fixedDeltaTime / timeToLive);
        }
    }
}
