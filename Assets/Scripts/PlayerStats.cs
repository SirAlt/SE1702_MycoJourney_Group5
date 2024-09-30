using UnityEngine;

[CreateAssetMenu]
public class PlayerStats : ScriptableObject
{
    [Header("Collision")]
    [Tooltip("Layer(s) terrain and obstacles are on.")]
    public LayerMask TerrainLayers;

    [Range(0f, 0.5f)]
    [Tooltip("Offset for calculating collisions when applying movement.")]
    public float CollisionOffset;

    [Range(0f, 0.5f)]
    [Tooltip("How far above the ground the character stands.")]
    public float FloorOffset;

    [Range(0f, 0.5f)]
    [Tooltip("How far below an obstacle the character stops and loses all upward velocity.")]
    public float CeilingOffset;

    [Range(0f, 0.5f)]
    [Tooltip("The distance at which the character is considered to be touching a wall. Useful for abilities like wallgrab, wallslide, and walljump.")]
    public float WallOffset;


    [Header("Movement")]
    [Tooltip("Maximum horizontal movement speed.")]
    public float MaxHorizontalSpeed;

    [Tooltip("How fast the character gains horizontal movement speed.")]
    public float HorizontalAcceleration;

    [Tooltip("How fast the character's horizontal movement stops while on the ground.")]
    public float GroundDeceleration;

    [Tooltip("How fast the character's horizontal movement stops while in the air.")]
    public float AirDeceleration;


    [Header("Gravity")]
    [Tooltip("Downward acceleration when in the air.")]
    public float GravitationalAcceleration;

    [Tooltip("Terminal velocity.")]
    public float MaxFallSpeed;


    [Header("Jump")]
    [Tooltip("Velocity gained instantly at the start of a jump.")]
    public float JumpPower;

    [Tooltip("Continual upward acceleration applied during the beginning portion of a jump.")]
    public float JumpAcceleration;

    [Range(0f, 2f)]
    [Tooltip("The amount of time upward acceleration is in effect. Gravity is ignored during this time.")]
    public float JumpAccelerationDuration;


    [Header("Variable Jump Height")]
    [Tooltip("The gravity multiplier added when jump is released early.")]
    public float JumpEndEarlyGravityModifier = 3;

    [Tooltip("The time window for cutting a jump short. Past this point, the jump will reach max height even if the jump key is not held.")]
    public float JumpEndEarlyWindow;


    [Header("Input")]
    [Tooltip("The amount of time a jump is buffered. This allows jump input before actually hitting the ground.")]
    public float JumpBuffer;

    [Tooltip("The amount of time jump input is ignored after executing a jump. Prevents accidental jumps caused by button mashing.")]
    public float JumpRefractoryPeriod;


    [Header("Coyote Time")]
    [Tooltip("The time before coyote jump becomes unusable. Coyote jump allows jump to execute even after leaving a ledge.")]
    public float CoyoteTime;


    [Header("Apex Modifier")]
    [Tooltip("Vertical speed below which apex modifier activates. The lower, the larger the apex modifier window.")]
    public float ApexThreshold;

    [Tooltip("Additional horizontal acceleration while near jump apex.")]
    public float ApexAccelerationBonus;

    [Tooltip("Counter-gravity applied while apex modifer is in effect.")]
    public float ApexAntiGravityBonus;


    [Header("Slope")]
    [Range(0f, 10f)]
    [Tooltip("Constant downward velocity applied while on the ground. Helps on slopes.")]
    public float GroundingVelocity;
}
