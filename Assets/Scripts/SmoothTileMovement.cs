using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(1000)]
public class SmoothTileMovement : MonoBehaviour, IMovementStrategy
{
    [SerializeField] private SmoothTileMovementConfigs _config;

    private ContactFilter2D _filter;
    private List<RaycastHit2D> _hits = new();
    private float minDot;   // Dot product of 2 normalized vectors where the angle between them is MAX_SLOPE_ANGLE.

    private Rigidbody2D _rb;

    private Collider2D _collider;
    public Collider2D Collider
    {
        set
        {
            _collider = value;
            _rb = _collider.attachedRigidbody;
        }
    }

    public Vector2 EnvironmentVelocity { get; set; }

    private void Awake()
    {
        _filter = new ContactFilter2D();
        _filter.SetLayerMask(_config.ObstacleLayer);
        _filter.useTriggers = false;

        minDot = Mathf.Cos(_config.MaxSlopeAngle * Mathf.Deg2Rad);
    }

    public void Move(Vector2 velocity)
    {
        var cachedQueriesStartInColliders = Physics2D.queriesStartInColliders;
        Physics2D.queriesStartInColliders = true;

        var totalVelocity = velocity + EnvironmentVelocity;

        // TODO: Use Collider shape instead
        var bounds = GetBounds();

        StepX();
        StepY();

        Physics2D.queriesStartInColliders = cachedQueriesStartInColliders;

        void StepX()
        {
            var xStep = totalVelocity.x * Time.fixedDeltaTime;
            if (xStep == 0) return;

            bool terrainHit = _collider.Cast(Vector2.right * Mathf.Sign(xStep), _filter, _hits, Mathf.Abs(xStep) + _config.CollisionOffset) > 0;

            if (terrainHit)
            {
                float distance;
                var closestDistance = Mathf.Infinity;
                RaycastHit2D closestHit = _hits[0];
                foreach (var hit in _hits)
                {
                    distance = xStep > 0
                       ? hit.point.x - bounds.max.x
                       : bounds.min.x - hit.point.x;
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestHit = hit;
                    }
                }

                // BUG: When going up a ledge, especially when approaching the ledge from level ground, the character goes airborne briefly.
                // PARTIAL CAUSE: In the 1st frame of slope movement, StepY detects 2 grounds, and moves
                // so little - even with high grounding totalVelocity - that the character remains airborne.
                var dot = Vector2.Dot(closestHit.normal, Vector2.up);
                if (dot >= minDot && dot < 1)
                {
                    //var oldPos = _rb.position;

                    var bodyXStep = velocity.x * Time.fixedDeltaTime;
                    var envXStep = EnvironmentVelocity.x * Time.fixedDeltaTime;

                    var step = -1.0f * bodyXStep * Vector2.Perpendicular(closestHit.normal) + new Vector2(envXStep, 0);
                    _rb.position += step;

                    //Debug.Log($"Slope movement:  oldPos={oldPos}  step={step}  newPos={_rb.position}  minDot={minDot}");
                    return;
                }
                else
                {
                    xStep = Mathf.Sign(xStep) * (closestDistance - _config.CollisionOffset);
                }
            }
            _rb.position = new Vector2(_rb.position.x + xStep, _rb.position.y);
        }

        void StepY()
        {
            var yStep = totalVelocity.y * Time.fixedDeltaTime;
            if (yStep == 0) return;

            bool terrainHit = _collider.Cast(Vector2.up * Mathf.Sign(yStep), _filter, _hits, Mathf.Abs(yStep) + _config.CollisionOffset) > 0;

            if (terrainHit)
            {
                float distance;
                var closestDistance = Mathf.Infinity;
                foreach (var raycastHit in _hits)
                {
                    distance = yStep > 0
                       ? raycastHit.point.y - bounds.max.y
                       : bounds.min.y - raycastHit.point.y;
                    if (distance < closestDistance) { closestDistance = distance; }
                }
                yStep = Mathf.Sign(yStep) * (closestDistance - _config.CollisionOffset);
            }

            _rb.position = new Vector2(_rb.position.x, _rb.position.y + yStep);
        }
    }

    private Bounds GetBounds()
    {
        return _collider.bounds;

        //var colliders = new List<Collider2D>();
        //_ = _rb.GetAttachedColliders(colliders);

        //if (colliders.Count > 0)
        //{
        //    var bounds = colliders[0].bounds;
        //    for (int i = 1; i < colliders.Count; i++)
        //    {
        //        bounds.Encapsulate(colliders[i].bounds);
        //    }
        //    return bounds;
        //}
        //return new Bounds(_rb.position, Vector3.zero);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_config == null) Debug.LogWarning($"Please assign a {nameof(SmoothTileMovementConfigs)} asset to the Movement script's Config slot", this);
    }
#endif
}
