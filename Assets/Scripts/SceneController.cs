using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour {
    public AudioSource audio;
    public void MainMenu() {
        GameObject.FindGameObjectWithTag("Music")?.GetComponent<menumusic>().PlayMusic();
        StartCoroutine(LoadScene("Main Menu"));
    }
    public void NewGame() {
        StartCoroutine(LoadScene("Tutorial"));
    }
    public void QuitGame() {
        Application.Quit();
    }
    public void CreditScreen() {
        StartCoroutine(LoadScene("Credit Screen"));
    }
    public void LoreMenu() {
        StartCoroutine(LoadScene("Lore Menu"));
    }public void Level1() {
        StartCoroutine(LoadScene("Level 1"));
    }
    public void Boss_level() {
       StartCoroutine(LoadScene("Boss_Level"));
    }
    public void Victory_Screen() {
        StartCoroutine(LoadScene("VictoryScreen"));
    }
    public void ArtGallery() {
        StartCoroutine(LoadScene("ArtGallery"));
    }
    public IEnumerator LoadScene(string scene) {
        yield return new WaitForSeconds(audio.clip.length - 1f);
        SceneManager.LoadScene(scene);
    }
}
