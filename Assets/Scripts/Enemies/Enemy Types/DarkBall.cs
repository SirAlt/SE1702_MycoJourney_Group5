using UnityEngine;

public class DarkBall : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private Transform target;
    [SerializeField] private float lifetime;
    [SerializeField] private float turnRate;
    private float lifeTimer;
    private BoxCollider2D boxCollider;
    private Animator animator;
    private Vector2 direction;
    private bool hit;
    public BodyContacts BodyContacts { get; private set; }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        BodyContacts = GetComponent<BodyContacts>();
    }

    void Update()
    {
        if (hit)
        {
            return;
        }

        lifeTimer -= Time.deltaTime;

        if (target != null)
        {
            if (BodyContacts.Ground || BodyContacts.Ceiling || BodyContacts.Wall)
            {
                Explode();
            }
            var direction = (target.position - transform.position).normalized;
            var rotation = Quaternion.FromToRotation(transform.right, direction);
            var targetRotation = rotation * transform.rotation;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnRate);
            float movementSpeed = speed * Time.deltaTime;
            transform.Translate(Vector2.right * movementSpeed);
        }
        if (lifeTimer <= 0)
        {
            Explode();
        }
    }

    private void Explode()
    {
        animator.SetTrigger("explode");
        hit = true;
        boxCollider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Explode();
    }

    public void SetDirection(Vector2 _direction)
    {
        lifeTimer = lifetime;
        var direction = (target.position - transform.position).normalized;
        var rotation = Quaternion.FromToRotation(transform.right, direction);
        var targetRotation = rotation * transform.rotation;
        
        transform.SetPositionAndRotation(
            new Vector3(transform.position.x, transform.position.y, 0),
            Quaternion.RotateTowards(transform.rotation, targetRotation, 360f));
        gameObject.SetActive(true);
        hit = false;
        boxCollider.enabled = true;
        if (direction.x < 0)
        {
            transform.eulerAngles = new Vector3(0f, 180f, 0f);
        }
        else
        {
            transform.eulerAngles = Vector3.zero;
        }
    }
    //animation event
    private void Deactive()
    {
        gameObject.SetActive(false);
    }
}
