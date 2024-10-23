using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    private const string PlayerTag = "Player";

    [SerializeField] private int HealthRestore = 30;

    private void Update()
    {
        transform.eulerAngles += new Vector3(0f, 180f * Time.deltaTime, 0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(PlayerTag)
            && collision.TryGetComponent<IHealable>(out var target))
        {
            target.Heal(HealthRestore);
            Destroy(gameObject);
        }
    }
}
