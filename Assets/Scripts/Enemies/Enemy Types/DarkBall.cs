using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;

public class DarkBall : MonoBehaviour 
{
    [SerializeField] private float speed =10f;
    private float direction;
    private bool hit;
    private Animator  animator;
    private BoxCollider2D boxCollider;
    private float lifetime;
    [SerializeField] private Transform target;
    public BodyContacts BodyContacts { get; private set; }
    // Start is called before the first frame update
    private void Awake()
    {
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
          BodyContacts = GetComponent<BodyContacts>();

    }


    // Update is called once per frame
    void Update()
    {
        if (hit)
        {
            return;
        }
        //Debug.Log("Speed: " + speed + " Direction: " + direction);
        //float movementSpeed  = speed * Time.deltaTime * direction;

        //transform.Translate(movementSpeed, 0, 0);
        //lifetime += Time.deltaTime;
        if (target != null)
        {
            if (BodyContacts.Ground || BodyContacts.Ceiling || BodyContacts.WallLeft|| BodyContacts.WallRight)
            {
                animator.SetTrigger("explode");

                hit = true;
                boxCollider.enabled = false;
                lifetime = 0; 
            }
            Vector3 direction = (target.position - transform.position).normalized; // Tính h??ng ??n nhân v?t
            float movementSpeed = speed * Time.deltaTime; // Tính t?c ?? di chuy?n

            transform.Translate(direction * movementSpeed); // Di chuy?n viên ??n theo h??ng ??n nhân v?t
            float localScaleX = transform.localScale.x;
            if (direction.x < 0)
            {
                localScaleX = -Mathf.Abs(localScaleX); // L?t viên ??n n?u di chuy?n sang trái
            }
            else
            {
                localScaleX = Mathf.Abs(localScaleX); // ??t l?i kích th??c theo h??ng ph?i
            }
            transform.localScale = new Vector3(localScaleX, transform.localScale.y, transform.localScale.z);
        }
        if (lifetime > 2)
        {

            gameObject.SetActive(false);
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Collision detected with " + collision.name); // Ki
        hit = true;
        boxCollider.enabled = false;
        animator.SetTrigger ("explode");
       
    }
    public void SetDirection(float _direction)
    {
        lifetime = 0;
        direction = _direction;
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);

        gameObject.SetActive(true);
        hit = false;
        boxCollider.enabled = true;

        float localScaleX = transform.localScale.x;
        if(Mathf.Sign(localScaleX) != _direction)
        {
            localScaleX = -localScaleX;
        }
        transform.localScale = new Vector3(localScaleX,transform.localScale.y, transform.localScale.z);
    }
    private void Deactive()
    {
        gameObject.SetActive(false); 
    }
}
