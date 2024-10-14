using UnityEngine;

[CreateAssetMenu]
public class SmoothTileMovementConfigs : ScriptableObject
{
    [Tooltip("Layer(s) terrain, solid objects, and movement-limiting entities (such as invisible walls) are on.")]
    public LayerMask ObstacleLayer;

    [Range(0f, 0.5f)]
    [Tooltip("Extra safety distance used when calculating collisions. Helps prevent moving too close and getting stuck inside terrain.")]
    public float CollisionOffset;

    [Range(0f, 90f)]
    [Tooltip("Maximum angle of inclination at which a surface considered to be a slope. Beyond this the surface considered a wall.")]
    public float MaxSlopeAngle;
}
