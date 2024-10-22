using Assets.Scripts;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEye : Enemy ,IDamageable
{
    [SerializeField] private float flightSpeed = 2f;
    [SerializeField] private float waypointReachedDistance = 0.2f;
    [SerializeField] private List<Transform> waypoints;
    [SerializeField] private DetectionZone biteDetectionZone;
    [SerializeField] private EnemyFacing facing;
    [SerializeField] private Attack attack; // Thêm thu?c tính Attack
    private static readonly int AttackTrigger = Animator.StringToHash("Attack");
    Animator animator;
    Rigidbody2D rb;


    public bool _hasTarget = false;

    Transform nextWaypoint;
    int waypointNum = 0;

    public BodyContacts BodyContacts { get; private set; }

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
        BodyContacts = GetComponent<BodyContacts>();
        attack = GetComponentInChildren<Attack>(); // L?y compon
    }

    private void Start()
    {
        nextWaypoint = waypoints[waypointNum];
    }
  

    void Update()
    {
        if (biteDetectionZone != null)
        {
            HasTarget = biteDetectionZone.detectedColliders.Count > 0;
            Debug.Log("Detected target: " + HasTarget);

            if (HasTarget)
            {
                if (attack != null)
                {
                    attack.Activate();
                    animator.Play("Attack",-1,0f);


                }
                else
                {
                    Debug.LogError("Attack component is not assigned.");
                }
            }
        }
        else
        {
            Debug.LogWarning("biteDetectionZone is not assigned in the Inspector.");
        }

    }
   
    private void FixedUpdate()
    {

        //    if (CanMove)
        //{
        //Flight();
        //}

        //Flight();

    }


    private void Flight()
    {
        // Tính toán kho?ng cách ??n waypoint hi?n t?i
        float distance = Vector2.Distance(nextWaypoint.position, transform.position);

        // Xác ??nh h??ng di chuy?n t?i waypoint
        Vector2 directionToWaypoint = (nextWaypoint.position - transform.position).normalized;

        // X? lý va ch?m v?i t??ng ho?c m?t ??t: chuy?n sang waypoint ti?p theo n?u ch?m
        if (BodyContacts.WallLeft || BodyContacts.WallRight || BodyContacts.Ground)
        {
            // N?u ch?m t??ng ho?c m?t ??t, quay l?i waypoint ??u tiên
            waypointNum = 0; // Quay l?i waypoint ??u tiên

            // C?p nh?t waypoint ti?p theo
            nextWaypoint = waypoints[waypointNum];

            // Tính toán l?i h??ng di chuy?n ??n waypoint m?i
            directionToWaypoint = (nextWaypoint.position - transform.position).normalized;

            Debug.Log("Hit wall or ground. Moving to initial waypoint.");
        }

        // X? lý va ch?m v?i tr?n: ?i?u ch?nh h??ng bay xu?ng
        if (BodyContacts.Ceiling)
        {
            // N?u ch?m tr?n, bay xu?ng
            directionToWaypoint.y = -Mathf.Abs(directionToWaypoint.y); // Bay xu?ng t? t?
            Debug.Log("Hit the ceiling. Changing direction to down.");
        }

        // Xác ??nh h??ng ??i m?t d?a trên h??ng di chuy?n
        if (directionToWaypoint.x < 0)
            facing.FaceLeft();
        else if (directionToWaypoint.x > 0)
            facing.FaceRight();

        // C?p nh?t v?n t?c c?a Rigidbody d?a trên h??ng di chuy?n
        rb.velocity = directionToWaypoint * flightSpeed;

        // Ki?m tra n?u ?ã ??n g?n waypoint và chuy?n sang waypoint k? ti?p
        if (distance <= waypointReachedDistance)
        {
            waypointNum++;

            if (waypointNum >= waypoints.Count)
            {
                waypointNum = 0;
            }

            nextWaypoint = waypoints[waypointNum];
        }
    }
}
