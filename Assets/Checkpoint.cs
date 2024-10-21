using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Checkpoint : MonoBehaviour
{
    private const string PlayerTag = "Player";

    private CheckpointSystem _checkpointSys;

    private SpriteRenderer _sprite;
    private Animator _anim;
    private Collider2D _detector;

    private bool _enabled;

    private void Awake()
    {
        _checkpointSys = GetComponentInParent<CheckpointSystem>();

        _sprite = GetComponent<SpriteRenderer>();
        _anim = GetComponent<Animator>();
        _detector = GetComponent<Collider2D>();
    }

    private void Start()
    {
        Deactivate();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(PlayerTag))
        {
            _checkpointSys.SetLastCheckpoint(this);
        }
    }

    public void Activate()
    {
        _enabled = true;
        _sprite.color = Color.white;
        _anim.speed = 1.0f;
        _detector.enabled = false;
    }

    public void Deactivate()
    {
        _enabled = false;
        _sprite.color = new Color(0.4f, 0.4f, 0.4f);
        _detector.enabled = true;
    }

    public void OnAnimationReachedLowestPoint()
    {
        if (!_enabled)
        {
            _anim.speed = 0f;
        }
    }
}
