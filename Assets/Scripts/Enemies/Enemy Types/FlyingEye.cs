using Assets.Scripts;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FlyingEye : Enemy
{
    [SerializeField] private DetectionZone biteDetectionZone;
    [SerializeField] private EnemyFacing facing;
    [SerializeField] private Transform darkPoint;
    [SerializeField] private GameObject[] darkballs;
    [SerializeField] private float attackCooldown;
    [SerializeField] private int barrageSize;
    [SerializeField] private float barrageDelay;

    private WaypointMover waypointMover;
    private float timer;
    private Animator animator;
    private Rigidbody2D rb;
    public bool _hasTarget = false;
    Transform nextWaypoint;
    int waypointNum = 0;

    public BodyContacts BodyContacts { get; private set; }

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
        waypointMover = GetComponent<WaypointMover>();
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (biteDetectionZone != null)
        {
            HasTarget = biteDetectionZone.detectedColliders.Count > 0;
            if (timer <= 0 && HasTarget)
            {

                timer = attackCooldown;

                for (int i = 0; i < barrageSize; i++)
                {
                    Fire();
                    Invoke(nameof(Fire), barrageDelay);
                    Invoke(nameof(Fire), barrageDelay * 2);
                }

                rb.velocity = Vector2.zero;

                if (biteDetectionZone.detectedColliders.Count > 0)
                {
                    Collider2D targetCollider = biteDetectionZone.detectedColliders[0];
                    Vector2 directionToTarget = (targetCollider.transform.position - transform.position).normalized;
                    facing.FaceRight();
                    if (directionToTarget.x < 0)
                    {
                        facing.FaceLeft();
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("biteDetectionZone is not assigned in the Inspector.");
        }


    }
    void Fire()
    {
        darkballs[FindDarkball()].transform.position = darkPoint.position;
        darkballs[FindDarkball()].GetComponent<DarkBall>().SetDirection(new Vector2(Mathf.Sign(transform.localScale.x), 0));
    }
    private int FindDarkball()
    {
        for (int i = 0; i < darkballs.Length; i++)
        {
            if (!darkballs[i].activeInHierarchy)
            {
                return i;
            }
        }
        return 0;
    }

    private void FixedUpdate()
    {
        if (!HasTarget)
        {
            if (waypointMover.MoveVector.x < 0)
            {
                facing.FaceLeft();
            }
            else
            {
                facing.FaceRight();
            }
        }
    }


}
