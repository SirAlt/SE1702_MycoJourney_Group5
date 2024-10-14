using UnityEngine;

public interface IMovementStrategy
{
    Collider2D Collider { set; }

    Vector2 EnvironmentVelocity { set; get; }

    void Move(Vector2 velocity);
}
