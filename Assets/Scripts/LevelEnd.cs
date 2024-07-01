using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEnd : MonoBehaviour
{
    private bool moveplayer;
    private PlayerController player;
    private CameraController camera;
    private LevelManagerScript levelManager;
    public float waittomove; //
    public float waittoload; // wait before loading level
    public string leveltoload; // for level transitions
    public GameObject key,bosstext;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        camera = FindObjectOfType<CameraController>();
        levelManager = FindObjectOfType<LevelManagerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (moveplayer)
        {
            player.rb.velocity = new Vector2(player.movespeed, player.rb.velocity.y) * player.transform.position.normalized;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if(SceneManager.GetActiveScene().name != "Level 1") {
                if (SceneManager.GetActiveScene().name == "Boss_Level" && bosstext.gameObject.activeSelf == false) {
                    SceneManager.LoadScene("Story 3");
                } 
                else if (SceneManager.GetActiveScene().name != "Boss_Level") {
                    StartCoroutine(LevelEndCo());
                }
            } 
            else if (SceneManager.GetActiveScene().name == "Level 1") {
                if (key.activeSelf == false) {
                    StartCoroutine(LevelEndCo());
                } 
                else {
                    levelManager.nokeymsg.SetActive(true);
                }
            }

        }
        
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        levelManager.nokeymsg.SetActive(false);
    }
    private IEnumerator LevelEndCo()
    {
        levelManager.levelmusic.Stop();
        levelManager.audio.PlayOneShot(levelManager.winsoundeffect);
        player.canMove = false;
        camera.followtarget = false;
        player.rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(waittomove);
        moveplayer = true;
        yield return new WaitForSeconds(waittoload);
        SceneManager.LoadScene(leveltoload);
    }
}