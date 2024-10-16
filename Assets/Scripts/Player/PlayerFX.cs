using UnityEngine;

public class PlayerFX : MonoBehaviour
{
    private PlayerController _playerController;
    private Animator _animator;

#if UNITY_EDITOR
    private void OnValidate()
    {
    }
#endif
}
