using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
[RequireComponent(typeof(Collider2D))]
public class BodyContacts : MonoBehaviour
{
    [SerializeField] private BodyContactsConfigs _config;

    private Collider2D _collider;
    private ContactFilter2D _filter;
    private List<RaycastHit2D> _hits;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();

        _filter = new ContactFilter2D();
        _filter.SetLayerMask(_config.TerrainLayer);

        _hits = new();
    }

    private void FixedUpdate()
    {
        var _cachedQueryStartInColliders = Physics2D.queriesStartInColliders;
        Physics2D.queriesStartInColliders = false;

        CheckGround();
        CheckCeiling();
        CheckWall();

        Physics2D.queriesStartInColliders = _cachedQueryStartInColliders;
    }

    public bool Ground { get; private set; }
    public bool Ceiling { get; private set; }
    public bool WallLeft { get; private set; }
    public bool WallRight { get; private set; }

    public bool Wall => WallLeft || WallRight;

    private void CheckGround()
    {
        Ground = _collider.Cast(Vector2.down, _filter, _hits, _config.GroundContactDistance) > 0;
    }

    private void CheckCeiling()
    {
        Ceiling = _collider.Cast(Vector2.up, _filter, _hits, _config.CeilingContactDistance) > 0;
    }

    private void CheckWall()
    {
        WallRight = _collider.Cast(Vector2.right, _filter, _hits, _config.WallContactDistance) > 0;
        WallLeft = _collider.Cast(Vector2.left, _filter, _hits, _config.WallContactDistance) > 0;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_config == null) Debug.LogWarning($"Please assign a {nameof(BodyContactsConfigs)} asset to the Body Contacts' Config slot", this);
    }
#endif
}
