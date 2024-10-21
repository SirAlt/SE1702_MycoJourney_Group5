using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ProximityTrigger : MonoBehaviour
{
    [SerializeField] private LayerMask triggerLayers;

    private ITriggerable payload;

    private void Awake()
    {
        payload = transform.parent.GetComponentInChildren<ITriggerable>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((triggerLayers & (1 << collision.gameObject.layer)) != 0)
        {
            payload.Trigger();
        }
    }
}
