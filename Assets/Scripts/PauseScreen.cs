using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScreen : MonoBehaviour {
    public PlayerController player;
    public LevelManagerScript levelManager;
    public GameObject pausescreen;
    // Start is called before the first frame update
    void Start() {
        player = FindObjectOfType<PlayerController>();
        levelManager = FindObjectOfType<LevelManagerScript>();
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetButtonDown("Cancel")) {
            if (Time.timeScale == 0) {
                ResumeGame();
            } else {
                PauseGame();
            }
        }
    }
    public void PauseGame() {
        levelManager.levelmusic.Pause();
        Time.timeScale = 0; // freeze game
        pausescreen.SetActive(true);
        player.canMove = false;
    }
    public void ResumeGame() {
        Time.timeScale = 1;
        pausescreen.SetActive(false);
        player.canMove = true;
        levelManager.levelmusic.UnPause();
    }
    public void BackToMainMenu() {
        Time.timeScale = 1;
        SceneManager.LoadScene("Main Menu");
        GameObject.FindGameObjectWithTag("Music")?.GetComponent<menumusic>().PlayMusic();
    }
}
