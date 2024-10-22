using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealthText : MonoBehaviour
{


    public Vector3 movedSpeed = new Vector3(0,75,0);
    RectTransform textTranform;
    private float timeElapsed = 0f;
    public float timeToFace = 1f;
    private Color startColor;
    TextMeshProUGUI textMeshPro;

    private void Awake()
    {
        textTranform = GetComponent<RectTransform>();
        textMeshPro = GetComponent<TextMeshProUGUI>();
        startColor = textMeshPro.color;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        textTranform.position += movedSpeed * Time.deltaTime;

        timeElapsed += Time.deltaTime;
        if (timeElapsed < timeToFace)
        { 
            float fadeAlpha = startColor.a * (1 - (timeElapsed / timeToFace));
            textMeshPro.color = new Color(startColor.r, startColor.g, startColor.b, fadeAlpha);

        }
        else
        {
            Destroy(gameObject);
        }
    }
}
