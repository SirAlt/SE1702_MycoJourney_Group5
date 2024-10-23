using System.Collections.Generic;
using UnityEngine;

public class DarkBall : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifetime;
    [SerializeField] private float turnRate;
    [SerializeField] private LayerMask terrainLayers;

    private Animator _anim;
    private Collider2D _hitbox;
    private ContactFilter2D _terrainFilter;
    private readonly List<Collider2D> _overlaps = new();

    private Transform _target;
    private float _lifetimeTimer;
    private bool _exploding;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _hitbox = GetComponent<Collider2D>();

        _terrainFilter = new ContactFilter2D();
        _terrainFilter.SetLayerMask(terrainLayers);
    }

    void Update()
    {
        if (_exploding) return;

        if (_hitbox.OverlapCollider(_terrainFilter, _overlaps) > 0)
        {
            Explode();
            return;
        }

        _lifetimeTimer -= Time.deltaTime;
        if (_lifetimeTimer <= 0)
        {
            Explode();
            return;
        }

        if (_target != null)
        {
            var directionToTarget = (_target.position - transform.position).normalized;
            var rotationNeededToFaceTarget = Quaternion.FromToRotation(transform.right, directionToTarget);
            var destRotation = rotationNeededToFaceTarget * transform.rotation;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, destRotation, turnRate);

            float movementSpeed = speed * Time.deltaTime;
            transform.Translate(Vector2.right * movementSpeed);
        }
    }

    private void Explode()
    {
        _exploding = true;
        _anim.SetTrigger("explode");
        _hitbox.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Explode();
    }

    public void Launch(Transform target)
    {
        gameObject.SetActive(true);
        _exploding = false;
        _target = target;
        _lifetimeTimer = lifetime;
        _hitbox.enabled = true;

        var directionToTarget = (target.position - transform.position).normalized;
        var rotationNeededToFaceTarget = Quaternion.FromToRotation(transform.right, directionToTarget);
        transform.rotation *= rotationNeededToFaceTarget;

        if (directionToTarget.x < 0)
            transform.eulerAngles = new Vector3(0f, 180f, 0f);
        else
            transform.eulerAngles = Vector3.zero;
    }

    // AnimEvent: Explode [End]
    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
