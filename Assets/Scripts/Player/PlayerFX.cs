using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerFX : MonoBehaviour
{
    [SerializeField] private readonly string HealthBarTag = "HealthBar";

    private PlayerController _player;
    private SpriteRenderer _sprite;
    private Animator _animator;

    private Slider _healthBar;

    private void Awake()
    {
        _player = GetComponent<PlayerController>();
        _sprite = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _healthBar = GameObject.FindGameObjectWithTag(HealthBarTag).GetComponent<Slider>();
    }

    public void StartFlicker(float duration)
    {
        StartCoroutine(nameof(Flicker));
        Invoke(nameof(StopFlicker), duration);
    }

    private IEnumerator Flicker()
    {
        while (true)
        {
            _sprite.enabled = !_sprite.enabled;
            yield return new WaitForSeconds(_player.Stats.InvincibilityFlickerInterval);
        }
    }

    public void StopFlicker()
    {
        StopCoroutine(nameof(Flicker));
        _sprite.enabled = true;
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
