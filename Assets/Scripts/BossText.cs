using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossText : MonoBehaviour
{
    BossBehaviour boss;
    Animator anim;
    [SerializeField]
    float activetime, soundtime;
    LevelManagerScript levelManager;
    AudioSource audio;
    public AudioClip textsound;
    public bool bossdeath;
    // Start is called before the first frame update
    void Start()
    {
        boss = FindObjectOfType<BossBehaviour>();
        levelManager = FindObjectOfType<LevelManagerScript>();
        anim = GetComponent<Animator>();
        audio = GetComponent<AudioSource>();
        soundtime = textsound.length;
    }

    // Update is called once per frame
    void Update()
    {
        if (!levelManager.audio.isPlaying && !levelManager.prebossmusic.isPlaying && !boss.gameObject.activeSelf) {
            bossdeath = true;
            audio.PlayOneShot(textsound);
            soundtime -= Time.deltaTime;
            activetime -= Time.deltaTime;
        }
        if (activetime <= 0) {
            StartCoroutine(WaitforAnim());
        }
        if (soundtime <= 0) {
            audio.Stop();
        }
        anim.SetBool("Death", bossdeath);
        anim.SetFloat("Active", activetime);
    }
    IEnumerator WaitforAnim() {
        bossdeath = false;
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length + anim.GetCurrentAnimatorStateInfo(0).normalizedTime);
        gameObject.SetActive(false);
    }
}
