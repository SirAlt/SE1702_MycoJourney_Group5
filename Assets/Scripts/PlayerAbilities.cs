using UnityEngine;

[CreateAssetMenu]
public class PlayerAbilities : ScriptableObject
{
    [Header("Wall Slide")]
    [Tooltip("Drastically reduces falling speed while moving against a wall.")]
    public bool WallSlideLearnt;

    [Range(0f, 10f)]
    [Tooltip("The velocity at which the character slides down a wall, expressed as a ratio of gravitational acceleration. Set to 0 to cling to walls.")]
    public float WallSlideGravityModifier;


    [Header("Wall Jump")]
    [Tooltip("Can kick against walls to perform jumps.")]
    public bool WallJumpLearnt;

    [Tooltip("Velocity gained instantly at the start of a wall jump.")]
    public float WallJumpPower;

    [Range(0f, 90f)]
    [Tooltip("The angle at which jumps performed against walls will launch the character. 0 means straight up.")]
    public float WallJumpAngle;


    [Header("Air Jump")]
    [Tooltip("The number of an additional jumps while in the air. Double jump (= 1 charge) is the most common.")]
    public float AirJumpCharges;

    [Tooltip("Velocity gained instantly at the start of an air jump.")]
    public float AirJumpPower;
}
