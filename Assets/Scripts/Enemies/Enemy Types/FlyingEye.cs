using Assets.Scripts;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEye : MonoBehaviour
{
    [SerializeField] private float flightSpeed = 2f;
    [SerializeField] private float waypointReachedDistance = 0.2f;
    [SerializeField] private List<Transform> waypoints;
    [SerializeField] private DetectionZone biteDetectionZone;
    [SerializeField] private EnemyFacing facing;

    Animator animator;
    Rigidbody2D rb;

    public bool _hasTarget = false;

    Transform nextWaypoint;
    int waypointNum = 0;

    // Start is called before the first frame update

    public bool HasTarget
    {
        get { return _hasTarget; }

        private set
        {

            _hasTarget = value;
            animator.SetBool(AnimationStrings.hasTarget, value);

        }
    }
    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        facing = GetComponent<EnemyFacing>();
    }

    private void Start()
    {
        nextWaypoint = waypoints[waypointNum];
    }

    // Update is called once per frame
    void Update()
    {
        //HasTarget = biteDetectionZone.detectedColliders.Count > 0;


    }
    //public bool CanMove
    //{
    //    get
    //    {
    //        return animator.GetBool(AnimationStrings.canMove);
    //    }
    //}
    private void FixedUpdate()
    {

        //    if (CanMove)
        //{
        //Flight();
        //}

        Flight();

    }


    private void Flight()
    {
        Vector2 directionToWaypoint = (nextWaypoint.position - transform.position).normalized;

        if (directionToWaypoint.x < 0) facing.FaceLeft();
        else facing.FaceRight();

        float distance = Vector2.Distance(nextWaypoint.position, transform.position);

        rb.velocity = directionToWaypoint * flightSpeed;
        Debug.Log("Current Position: " + transform.position);


        if (distance <= waypointReachedDistance)
        {
            waypointNum++;

            if (waypointNum >= waypoints.Count)
            {
                waypointNum = 0;
            }

            nextWaypoint = waypoints[waypointNum];
            Debug.Log("Updated Next Waypoint: " + nextWaypoint.position);
        }

    }
}
