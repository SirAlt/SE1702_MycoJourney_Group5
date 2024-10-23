using Assets.Scripts.Player.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public int HealthRestore = 30;
    // Start is called before the first frame update
    private PlayerController _player;


    private void OnTriggerEnter2D(Collider2D collision)
    {      
        if (collision.gameObject.CompareTag("Player"))
        {
            if (_player.CurrentHealth > 0)
            {
                CharacterEvents.characterHealed.Invoke(_player.gameObject,HealthRestore);
                _player.CurrentHealth += HealthRestore;
                Destroy(gameObject);
            }
        }
    }
    private void FixedUpdate()
    {
        transform.eulerAngles += new Vector3(0f, 4f, 0f);
    }
    private void Awake()
    {

        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }
}
