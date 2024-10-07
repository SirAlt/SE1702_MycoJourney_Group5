using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(1000)]
public class SmoothTileMovement : MonoBehaviour, IMovementStrategy
{
    [SerializeField] private SmoothTileMovementConfigs _config;

    private ContactFilter2D _filter;
    private List<RaycastHit2D> _hits;

    private void Awake()
    {
        _filter = new ContactFilter2D();
        _filter.SetLayerMask(_config.TerrainLayer | _config.BoundaryLayer);

        _hits = new();
    }


    private Rigidbody2D _rb;
    public Rigidbody2D Rb
    {
        set
        {
            _rb = value;
            _rb.simulated = true;
            _rb.bodyType = RigidbodyType2D.Kinematic;
        }
    }

    public void Move(Vector2 velocity)
    {
        var cachedQueriesStartInColliders = Physics2D.queriesStartInColliders;
        Physics2D.queriesStartInColliders = false;

        var bounds = GetBounds();

        StepX();
        StepY();

        Physics2D.queriesStartInColliders = cachedQueriesStartInColliders;

        void StepX()
        {
            var xStep = velocity.x * Time.fixedDeltaTime;
            if (xStep == 0) return;

            bool terrainHit = _rb.Cast(Vector2.right * Mathf.Sign(xStep), _filter, _hits, Mathf.Abs(xStep) + _config.CollisionOffset) > 0;

            // TODO: Slopes - TRY: RaycastHit2D.Normal

            if (terrainHit)
            {
                float distance;
                var closestDistance = Mathf.Infinity;
                foreach (var raycastHit in _hits)
                {
                    // TODO: Account for collider shape
                    distance = xStep > 0
                       ? raycastHit.point.x - bounds.max.x
                       : bounds.min.x - raycastHit.point.x;
                    if (distance < closestDistance) { closestDistance = distance; }
                }
                xStep = Mathf.Max(closestDistance - _config.CollisionOffset, 0) * Mathf.Sign(xStep);
            }

            _rb.position = new Vector2(_rb.position.x + xStep, _rb.position.y);
        }

        void StepY()
        {
            var yStep = velocity.y * Time.fixedDeltaTime;
            if (yStep == 0) return;

            bool terrainHit = _rb.Cast(Vector2.up * Mathf.Sign(yStep), _filter, _hits, Mathf.Abs(yStep) + _config.CollisionOffset) > 0;

            if (terrainHit)
            {
                float distance;
                var closestDistance = Mathf.Infinity;
                foreach (var raycastHit in _hits)
                {
                    // TODO: Account for collider shape
                    distance = yStep > 0
                       ? raycastHit.point.y - bounds.max.y
                       : bounds.min.y - raycastHit.point.y;
                    if (distance < closestDistance) { closestDistance = distance; }
                }
                yStep = Mathf.Max(closestDistance - _config.CollisionOffset, 0) * Mathf.Sign(yStep);
            }

            _rb.position = new Vector2(_rb.position.x, _rb.position.y + yStep);
        }
    }

    private Bounds GetBounds()
    {
        var colliders = new List<Collider2D>();
        _ = _rb.GetAttachedColliders(colliders);

        if (colliders.Count > 0)
        {
            var bounds = colliders[0].bounds;
            for (int i = 1; i < colliders.Count; i++)
            {
                bounds.Encapsulate(colliders[i].bounds);
            }
            return bounds;
        }
        return new Bounds(_rb.position, Vector3.zero);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_config == null) Debug.LogWarning($"Please assign a {nameof(SmoothTileMovementConfigs)} asset to the Movement script's Config slot", this);
    }
#endif
}
