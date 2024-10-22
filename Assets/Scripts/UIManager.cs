using Assets.Scripts.Player.Events;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    public GameObject damageTextPrefab;
    public GameObject healthTextPrefab;
    // Start is called before the first frame update
    public Canvas gameCanvas;


    private void Awake()
    {
        gameCanvas = FindObjectOfType<Canvas>();
    }
    private void OnEnable()
    {
        CharacterEvents.characterDamaged += CharacterTookDamage;
        CharacterEvents.characterHealed += CharacterTookHealed;
    }

    private void OnDisable()
    {
        CharacterEvents.characterDamaged -= CharacterTookDamage;
        CharacterEvents.characterHealed -= CharacterTookHealed;
    }

    public void CharacterTookDamage(GameObject character, float damageReceived)
    {
        if (character == null) return;  // Ki?m tra n?u ??i t??ng ?ã b? h?y

        // L?y v? trí c?a nhân v?t trên màn hình
        Vector3 spawnPosition = Camera.main.WorldToScreenPoint(character.transform.position);

        // T?o text hi?n th? sát th??ng
        TMP_Text tmpText = Instantiate(damageTextPrefab, spawnPosition, Quaternion.identity, gameCanvas.transform).GetComponent<TMP_Text>();

        // Hi?n th? l??ng sát th??ng nh?n ???c
        tmpText.text = damageReceived.ToString();
    }

    public void CharacterTookHealed(GameObject character, float healthRestored)
    {
        if (character == null) return;  // Ki?m tra n?u ??i t??ng ?ã b? h?y

        // L?y v? trí c?a nhân v?t trên màn hình
        Vector3 spawnPosition = Camera.main.WorldToScreenPoint(character.transform.position);

        // T?o text hi?n th? h?i ph?c
        TMP_Text tmpText = Instantiate(healthTextPrefab, spawnPosition, Quaternion.identity, gameCanvas.transform).GetComponent<TMP_Text>();

        // Hi?n th? l??ng h?i ph?c
        tmpText.text = healthRestored.ToString();
    }
}
