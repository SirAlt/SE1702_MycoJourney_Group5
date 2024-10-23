using UnityEngine;

public class FlyingEye : Enemy
{
    [SerializeField] private DetectionZone detector;
    [SerializeField] private Transform darkPoint;
    [SerializeField] private GameObject[] darkballs;
    [SerializeField] private float attackCooldown;
    [SerializeField] private int barrageSize;
    [SerializeField] private float barrageDelay;

    private Animator _anim;
    private EnemyFacing _facing;
    private WaypointMover _mover;

    private float _attackTimer;
    private Transform _target;

    private bool _hasTarget;
    public bool HasTarget
    {
        get => _hasTarget;
        private set
        {
            _hasTarget = value;
            _anim.SetBool(Constants.hasTarget, value);
        }
    }

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _facing = GetComponent<EnemyFacing>();
        _mover = GetComponent<WaypointMover>();
    }

    private void Update()
    {
        _attackTimer -= Time.deltaTime;
        HasTarget = detector.detectedColliders.Count > 0;
        if (HasTarget)
        {
            _mover.enabled = false;
            _target = detector.detectedColliders[0].transform;

            var directionToTarget = (_target.position - transform.position).normalized;
            if (directionToTarget.x < 0)
                _facing.FaceLeft();
            else
                _facing.FaceRight();

            if (_attackTimer <= 0)
            {
                _attackTimer = attackCooldown;
                for (int i = 0; i < barrageSize; i++)
                {
                    Invoke(nameof(Fire), barrageDelay * i);
                }
            }
        }
        else
        {
            _mover.enabled = true;
        }
    }

    private void Fire()
    {
        var darkball = FindDarkball();
        darkballs[darkball].transform.position = darkPoint.position;
        darkballs[darkball].GetComponent<DarkBall>().Launch(_target);
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
        UpdateFacing();
    }

    private void UpdateFacing()
    {
        if (HasTarget) return;
        if (_mover.MoveVector.x < 0)
            _facing.FaceLeft();
        else
            _facing.FaceRight();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (detector == null) Debug.LogWarning($"No {nameof(DetectionZone)} assigned for [ {gameObject.name} ].");
    }
#endif
}
