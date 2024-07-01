using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeIn : MonoBehaviour
{
    public float fadetime;
    private Image blackscreen;
    // Start is called before the first frame update
    void Start()
    {
        blackscreen = GetComponent<Image>();
        blackscreen.CrossFadeAlpha(0f, fadetime, false);
        Destroy(gameObject, fadetime);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
