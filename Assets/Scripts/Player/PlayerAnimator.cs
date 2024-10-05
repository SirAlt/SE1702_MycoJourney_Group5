using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private Animator _animator;

    private void AnimateStateChange(PlayerState prev, PlayerState cur)
    {
        //if (prev == cur) return;
        //switch (cur)
        //{
        //    case PlayerState.Idle:
        //        ReturnToIdle();
        //        break;
        //    default:
        //        Debug.LogException(new NotImplementedException($"No animation state found for current player state: {prev} >> __{cur}__"));
        //        break;
        //}

        //void ReturnToIdle()
        //{
        //    switch (prev)
        //    {
        //        case PlayerState.Run:
        //            _animator.ResetTrigger(RunKey);
        //            break;
        //        case PlayerState.Jump:
        //            _animator.ResetTrigger(JumpKey);
        //            break;
        //        case PlayerState.Fall:
        //            _animator.ResetTrigger(FallKey);
        //            break;
        //        default:
        //            Debug.LogException(new NotImplementedException($"No cleanup logic found for previous player state: __{prev}__ >> {cur}"));
        //            break;
        //    }
        //}
    }

    //private static readonly string RunKey = "Hurt";
    //private static readonly string JumpKey = "Jump";
    //private static readonly string FallKey = "Fall";
    //private static readonly string WallSlideKey = "WallSlide";
    //private static readonly string WallJumpKey = "WallJump";
    //private static readonly string AttackKey = "Attack";
    //private static readonly string DashKey = "Dash";
    //private static readonly string HurtKey = "Hurt";
    //private static readonly string DeadKey = "Death";
}
