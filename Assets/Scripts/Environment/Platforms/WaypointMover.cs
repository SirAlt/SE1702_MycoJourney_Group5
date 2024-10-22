using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-5)]
[RequireComponent(typeof(Rigidbody2D))]
public class WaypointMover : MonoBehaviour, ICanMove
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float waypointReachedDistance = 0.1f;
    [SerializeField] private List<Transform> waypoints = new();

    private int _direction = -1;
    private int _currentWaypointIndex;
    private Transform _currentWaypoint;

    [HideInInspector] public Vector2 MoveVector { get; private set; }

    private Rigidbody2D _rb;

    private void Start()
    {
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
        // => We set both the positions of the Rigidbody and the Transform. The call to Rigidbody must be made first, so that it'll be overriden by
        //    the call to its own Transform and not the one to parent's Rigidbody.

        var oldPos = (Vector2)transform.position;
        var newPos = Vector2.MoveTowards(transform.position, _currentWaypoint.position, moveSpeed * Time.fixedDeltaTime);
        _rb.position = newPos;          // This is to move colliders immediately.
        transform.position = newPos;    // And this is to allow nested moving platforms.

        // Update movement vector for interface.
        MoveVector = newPos - oldPos;

        if (Vector2.Distance(transform.position, _currentWaypoint.position) <= waypointReachedDistance)
        {
            GetNextWaypoint();
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
}
