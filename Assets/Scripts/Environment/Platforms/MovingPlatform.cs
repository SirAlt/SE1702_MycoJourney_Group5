using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private LayerMask ferriableLayers;

    private readonly List<IFerriable> _ferriedObjects = new();
    private ICanMove _mover;

    private void Awake()
    {
        // Get component in parent or siblings
        _mover = transform.parent.GetComponentInChildren<ICanMove>();
    }

    private void FixedUpdate()
    {
        foreach (var obj in _ferriedObjects)
        {
            obj.MoveAlong(_mover.MoveVector);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((ferriableLayers & (1 << collision.gameObject.layer)) != 0
            && collision.TryGetComponent<IFerriable>(out var ferriableObj))
        {
            _ferriedObjects.Add(ferriableObj);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if ((ferriableLayers & (1 << collision.gameObject.layer)) != 0
             && collision.TryGetComponent<IFerriable>(out var ferriableObj))
        {
            _ferriedObjects.Remove(ferriableObj);
        }
    }
}
