using UnityEngine;

public interface IMovementStrategy
{
    Rigidbody2D Rb { set; }

    void Move(Vector2 velocity);
}
