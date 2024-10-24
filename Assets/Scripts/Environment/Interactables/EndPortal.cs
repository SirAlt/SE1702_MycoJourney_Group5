using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
public class EndPortal : MonoBehaviour
{
    private const string PlayerTag = "Player";

    private SpriteRenderer _sprite;
    private Collider2D _detector;

    private void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _detector = GetComponent<Collider2D>();
    }

    public void SetActive(bool active)
    {
        _sprite.enabled = active;
        _detector.enabled = active;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(PlayerTag))
        {
            MissionManager.Instance.OnLevelCompleted();
        }
    }
}
