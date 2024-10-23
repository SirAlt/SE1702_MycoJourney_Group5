using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-5)]
[RequireComponent(typeof(Rigidbody2D))]
public class WaypointMover : MonoBehaviour, ICanMove
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float waypointReachedDistance = 0.1f;
    [SerializeField] private List<Transform> waypoints = new();
    [SerializeField] private List<float> restTimes = new();
    [SerializeField] private PathEndBehavior pathEndBehavior;

    private int _currentWaypointIndex;
    private Transform _currentWaypoint;
    private Rigidbody2D _rb;

    private bool _isResting;
    private int _direction = 1;

    public Vector2 MoveVector { get; private set; }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        ValidateSettings();
        _currentWaypoint = waypoints[_currentWaypointIndex];
    }

    private void ValidateSettings()
    {
        foreach (var waypoint in waypoints)
        {
            if (waypoint.IsChildOf(transform))
            {
                Debug.LogWarning($"One of the {nameof(WaypointMover)} waypoints set for [ {gameObject.name} ] is itself or a child object.");
                break;
            }
        }

        if (restTimes.Count != waypoints.Count)
        {
            Debug.LogWarning($"Different numbers of waypoints and rest times set for {nameof(WaypointMover)} of [ {gameObject.name} ].");
            if (restTimes.Count < waypoints.Count)
            {
                restTimes.AddRange(new float[waypoints.Count - restTimes.Count]);
            }
        }

        // Not a typo. Only validate the rest times that have matching waypoints.
        for (var i = 0; i < waypoints.Count; i++)
        {
            if (restTimes[i] < 0f)
            {
                restTimes[i] = 0f;
                Debug.LogWarning($"Negative rest time at index {i} set for {nameof(WaypointMover)} of [ {gameObject.name} ].");
            }
        }

        // Do this check last since disabling movement changes the waypoint list, which could cause other checks to fail.
        switch (pathEndBehavior)
        {
            case PathEndBehavior.Reverse:
                if (waypoints.Count < 2)
                    DisableMovement();
                break;
            default:
                if (waypoints.Count < 1)
                    DisableMovement();
                break;
        }

        void DisableMovement()
        {
            moveSpeed = 0f;
            waypointReachedDistance = -1.0f;
            waypoints = new() { transform };
            Debug.LogWarning($"{nameof(WaypointMover)} of [ {gameObject.name} ] does not meet the required minimum number of waypoints for the selected path end behavior.");
        }
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

        if (!_isResting && Vector2.Distance(transform.position, _currentWaypoint.position) <= waypointReachedDistance)
        {
            _isResting = true;
            Invoke(nameof(GetNextWaypoint), restTimes[_currentWaypointIndex]);
        }
    }

    private void GetNextWaypoint()
    {
        switch (pathEndBehavior)
        {
            case PathEndBehavior.Loop:
                _currentWaypointIndex = (_currentWaypointIndex + 1) % waypoints.Count;
                break;
            case PathEndBehavior.Reverse:
                if (_currentWaypointIndex <= 0)
                    _direction = 1;
                else if (_currentWaypointIndex >= waypoints.Count - 1)
                    _direction = -1;
                _currentWaypointIndex += _direction;
                break;
            case PathEndBehavior.Stop:
                if (_currentWaypointIndex < waypoints.Count - 1)
                    ++_currentWaypointIndex;
                break;
        }

        _currentWaypoint = waypoints[_currentWaypointIndex];
        _isResting = false;
    }

    private enum PathEndBehavior
    {
        Loop,
        Reverse,
        Stop,
    }
}
