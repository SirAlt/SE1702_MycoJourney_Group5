using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-5)]
public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float waypointReachedDistance = 0.1f;
    [SerializeField] private List<Transform> waypoints = new();
    [SerializeField] private LayerMask moveableLayers;

    //private PlayerController _player;
    private Rigidbody2D _rb;

    private int _direction = -1;
    private int _currentWaypointIndex;
    private Transform _currentWaypoint;

    private readonly List<IMoveable> _ferriedObjects = new();

    private void Start()
    {
        //_player = FindAnyObjectByType<PlayerController>();
        _rb = GetComponent<Rigidbody2D>();
        _currentWaypoint = waypoints[_currentWaypointIndex];
    }

    private void FixedUpdate()
    {
        // HACK:
        // 1. Setting Rigidbody position fails for nested moving platforms (e.g., moving platforms inside moving rooms).
        //    This is because a Rigidbody controls the world-space position, and never the local-space position, of its GameObject's Transform.
        //    This means the Rigidbody of the parent moving platform overrides that of its children's.
        // 2. Setting the Transform's position (either world- or local-space) causes our physics scripts to fail.
        //    Our scripts rely on Rigidbodies and Colliders, but setting a GameObject's Transform does not update its components' positions until
        //    Unity's internal physics update phase, which is after all user scripts' FixedUpdate calls.
        // 
        // => We set both the positions of the Rigidbody and the Transform. The  call to Rigidbody must be made first, so that it'll be overriden by
        //    the call to its own Transform and not the one to parent's Rigidbody.
        var newPos = Vector2.MoveTowards(transform.position, _currentWaypoint.position, moveSpeed * Time.fixedDeltaTime);
        _rb.position = newPos;          // This is for our custom physics.
        transform.position = newPos;    // And this is to allow nested moving platforms.

        if (Vector2.Distance(transform.position, _currentWaypoint.position) <= waypointReachedDistance)
        {
            GetNextWaypoint();
        }

        var frameVelocity = moveSpeed * (_currentWaypoint.position - transform.position).normalized;
        foreach (var obj in _ferriedObjects)
        {
            obj.Move(frameVelocity);
        }
    }

    private void GetNextWaypoint()
    {
        if (_currentWaypointIndex <= 0 || _currentWaypointIndex >= waypoints.Count - 1)
        {
            _direction *= -1;
        }
        _currentWaypointIndex += _direction;
        _currentWaypoint = waypoints[_currentWaypointIndex];
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if (collision.gameObject == _player.gameObject)
        //{
        //    _player.Platforms.Add(this);
        //}

        if ((moveableLayers & (1 << collision.gameObject.layer)) != 0
            && collision.TryGetComponent<IMoveable>(out var moveableObj))
        {
            _ferriedObjects.Add(moveableObj);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //if (collision.gameObject == _player.gameObject)
        //{
        //    _player.Platforms.Remove(this);
        //}

        if ((moveableLayers & (1 << collision.gameObject.layer)) != 0
             && collision.TryGetComponent<IMoveable>(out var moveableObj))
        {
            _ferriedObjects.Remove(moveableObj);
        }
    }
}
