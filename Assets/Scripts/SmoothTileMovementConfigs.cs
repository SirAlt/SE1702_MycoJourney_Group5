using UnityEngine;

[CreateAssetMenu]
public class SmoothTileMovementConfigs : ScriptableObject
{
    [Tooltip("Layer(s) terrain and obstacles are on.")]
    public LayerMask TerrainLayer;

    [Tooltip("Layer(s) other types of movement-limiting objects (such as invisible walls) are on.")]
    public LayerMask BoundaryLayer;

    [Range(0f, 0.5f)]
    [Tooltip("Extra safety distance used when calculating collisions. Helps prevent moving too close and getting stuck inside terrain.")]
    public float CollisionOffset;
}
