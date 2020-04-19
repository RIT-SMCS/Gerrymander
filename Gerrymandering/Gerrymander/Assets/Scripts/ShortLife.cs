using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShortLife : MonoBehaviour
{

    float age = 0.0f;
    float lifeSpan = 1.25f;
    [SerializeField] TMPro.TextMeshPro childText;
    new Renderer renderer;
    Color transparentWhite = new Color(1.0f, 1.0f, 1.0f, 0.0f);
    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Color color = childText.color;
        float alpha = Mathf.Lerp(1.0f, 0.0f, age / lifeSpan);
        color.a = alpha;
        childText.color = color;
        renderer.material.color = Color.Lerp(Color.white, transparentWhite, age / lifeSpan);
        this.transform.localScale = Vector3.one * Mathf.Lerp(7.0f, 7.5f, age / lifeSpan);
        age += Time.deltaTime;
        if(age >= lifeSpan)
        {
            DestroyImmediate(this.gameObject);
        }
    }
}
