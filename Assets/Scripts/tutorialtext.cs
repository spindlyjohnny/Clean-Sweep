using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class tutorialtext : MonoBehaviour
{
    public GameObject[] text;
    public BossBehaviour boss;
    private LevelManagerScript levelManager;
    private CameraController camera;
    private PlayerController player;
    private void Start()
    {
        levelManager = FindObjectOfType<LevelManagerScript>();
        camera = FindObjectOfType<CameraController>();
        player = FindObjectOfType<PlayerController>();
        boss = FindObjectOfType<BossBehaviour>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            foreach (GameObject i in text)
            {
                i.SetActive(true);
            }
            if (SceneManager.GetActiveScene().name == "Boss_Level")
            {
                StartCoroutine(CameraPan());
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            foreach (GameObject i in text)
            {
                i.SetActive(false);
            }
        }
    }
    private IEnumerator CameraPan()
    {
        levelManager.prebossmusic.Stop();
        player.rb.velocity = Vector2.zero;
        boss.speedModifier = 0f;
        player.canMove = false;
        yield return new WaitForSeconds(1.2f);
        camera.target = boss.transform;
        camera.smoothing = 3.5f;
        camera.transform.position = Vector3.Lerp(camera.transform.position, camera.targetposition, Time.deltaTime * camera.smoothing);
        levelManager.bosshealth.gameObject.SetActive(true);
        levelManager.bossname.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        levelManager.levelmusic.Play();
        boss.mvmtAllowed = true;
        boss.speedModifier = 9f;
        camera.target = player.transform;
        yield return new WaitForSeconds(1f);
        player.canMove = true;
        player.respawnpoint = transform.position;
        gameObject.SetActive(false);
        camera.smoothing = 9999;
    }
}