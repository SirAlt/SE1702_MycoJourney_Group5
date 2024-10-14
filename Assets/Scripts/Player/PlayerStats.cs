using UnityEngine;

[CreateAssetMenu]
public class PlayerStats : ScriptableObject
{
    [Header("Ground Movement")]
    [Tooltip("Maximum horizontal movement speed on the ground.")]
    public float MaxGroundSpeed;

    [Tooltip("How fast the character gains horizontal movement speed.")]
    public float GroundAcceleration;

    [Tooltip("How fast the character's horizontal movement stops while on the ground.")]
    public float GroundDeceleration;


    [Header("Air Movement")]
    [Tooltip("Maximum horizontal movement speed in the air.")]
    public float MaxAirSpeed;

    [Tooltip("How fast the character gains horizontal movement speed.")]
    public float AirAcceleration;

    [Tooltip("How fast the character's horizontal movement stops while in the air.")]
    public float AirDeceleration;


    [Header("Gravity")]
    [Tooltip("Downward acceleration when in the air.")]
    public float GravitationalAcceleration;

    [Tooltip("Terminal velocity.")]
    public float FallSpeedClamp;


    [Header("Jump")]
    [Tooltip("_frameVelocity gained instantly at the start of a jump.")]
    public float JumpPower;

    [Tooltip("Continual upward acceleration applied during the beginning portion of a jump.")]
    public float InitialJumpAcceleration;

    [Range(0f, 1f)]
    [Tooltip("Gravity strength during the beginning portion of a jump. Helps make the jump feel less floaty.")]
    public float InitialJumpGravityModifier;

    [Tooltip("The amount of time upward acceleration and modified gravity is in effect.")]
    public float InitialJumpPeriod;


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
    [Tooltip("Constant downward velocity applied while on the ground. Helps on slopes.")]
    public float GroundingVelocity;


    [Header("Wall Jump")]
    [Tooltip("The amount of time the character is locked in animation at the start of a wall jump.")]
    public float WallJumpLockinTime;

    [Tooltip("Length of the period of time where the character strictly follows a wall jump's path, ignoring gravity. Can be interrupted by ending the jump early.")]
    public float WallJumpKickawayPeriod;


    [Header("Dash")]
    [Tooltip("The speed at which the character moves during a dash.")]
    public float DashSpeed;

    [Tooltip("The dimen in time of a dash.")]
    public float DashDuration;

    [Tooltip("The amount of time a dash is buffered.")]
    public float DashBuffer;

    [Tooltip("The time window for cutting a dash short. Past this point, the dash will go full distance even if the dash key is released.")]
    public float DashEndEarlyWindow;

    [Tooltip("Whether the character can interrupt a dash by jumping.")]
    public bool CanJumpCancelDash;

    [Tooltip("Scaling applied to the character's collision box during a dash.")]
    public Vector2 DashCollisionBoxScale;

    [Tooltip("Scaling applied to the character's Collider during a dash.")]
    public Vector2 DashHurtboxScale;


    [Header("Attack")]
    [Tooltip("The amount of time a slash is buffered.")]
    public float SlashBuffer;

    [Tooltip("Whether the character can interrupt attack animations by jumping.")]
    public bool CanJumpCancelAttack;

    [Tooltip("Whether the character can interrupt attack animations by dashing.")]
    public bool CanDashCancelAttack;

    [Tooltip("Enables an alternative version of ground attack that maintains ground movement.")]
    public bool CanAttackMove;

    [Tooltip("If enabled, attack-move will canel if the character stops moving mid-execution.")]
    public bool CancelAttackMoveWhenNoLongerMoving;

    [Tooltip("The number of consecutive frames the character must receive no movement input before an attack-move is cancelled. Has no effect if attack-move cannot be cancelled.")]
    public int AttackMoveCancelTolerance;

    [Tooltip("Allows the character to turn and redirect ground standing attacks.")]
    public bool CanTurnDuringStandingAttack;

    [Tooltip("Allows the character to turn and redirect ground moving attacks.")]
    public bool CanTurnDuringAttackMove;

    [Tooltip("Allows the character to turn and redirect air jumping attacks.")]
    public bool CanTurnDuringJumpAttack;

    [Tooltip("Horizontal speed while executing dash attacks.")]
    public float DashSlashMoveSpeed;
}
