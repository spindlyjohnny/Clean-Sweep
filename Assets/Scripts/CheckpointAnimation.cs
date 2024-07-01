using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointAnimation : MonoBehaviour
{
    Animator anim;
    Animator textanim;
    public GameObject text;
    AudioSource audio;
    bool active;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        audio = GetComponent<AudioSource>();
        text.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Player" && !active) {
            anim.SetTrigger("Player");
            text.SetActive(true);
            if (!active) {
                audio.Play();
                active = true;
            } else {
                audio.Stop();
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision) {
        if(collision.tag == "Player") {
            text.SetActive(false);
        }
    }
}
