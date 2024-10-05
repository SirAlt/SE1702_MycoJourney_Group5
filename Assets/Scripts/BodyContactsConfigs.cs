using UnityEngine;

[CreateAssetMenu]
public class BodyContactsConfigs : ScriptableObject
{
    [Tooltip("Layer(s) solid objects such as terrain and obstacles are on.")]
    public LayerMask TerrainLayer;

    [Range(0f, 0.5f)]
    [Tooltip("How far above solid objects the character is considered to be grounded.")]
    public float GroundContactDistance;

    [Range(0f, 0.5f)]
    [Tooltip("How far below solid objects the character is considered to be in contact with it.")]
    public float CeilingContactDistance;

    [Range(0f, 0.5f)]
    [Tooltip("How far away from solid object the character is considered to be in contact with it. Useful for abilities like wallslide and walljump.")]
    public float WallContactDistance;
}
