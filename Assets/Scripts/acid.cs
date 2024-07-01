using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class acid : MonoBehaviour
{
    public GameObject splash;
    AudioSource audio;
    private void Start() {
        audio = GetComponent<AudioSource>();
    }
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.GetComponent<PlayerController>()) {
            Destroy(Instantiate(splash, collision.transform.position, Quaternion.identity),1f);
            audio.Play();
        }
    }
}
