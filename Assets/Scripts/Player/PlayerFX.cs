using UnityEngine;
using UnityEngine.UI;

public class PlayerFX : MonoBehaviour
{
    [SerializeField] private readonly string HealthBarTag = "HealthBar";

    private PlayerController _player;
    private Animator _animator;

    private Slider _healthBar;

    private void Awake()
    {
        _healthBar = GameObject.FindGameObjectWithTag(HealthBarTag).GetComponent<Slider>();
    }

    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        _healthBar.value = currentHealth / maxHealth;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
    }
#endif
}
