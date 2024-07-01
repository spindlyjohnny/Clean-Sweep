using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManagerScript : MonoBehaviour
{
    public float waitToRespawn;
    PlayerController player;
    EnemyController[] enemies;
    CollectibleScript[] collectibles;
    FlyingEnemy[] flyingEnemies;
    public Image healthBar, heatBar, secondHeatBar;
    public Sprite noheat, oneheat, twoheat, fullheat, nohealth, onehealth, twohealth, threehealth, fourhealth, fullhealth;
    public Text continuestext, bossname;
    public int continues;
    public GameObject deathscreen, loreitem, nokeymsg;
    public AudioSource audio, levelmusic, prebossmusic;
    public AudioClip healthsoundeffect, pickupsoundeffect, winsoundeffect, losesoundeffect, bossdeathsound;
    public Slider bosshealth;
    public string levellore;

    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
        enemies = FindObjectsOfType<EnemyController>();
        flyingEnemies = FindObjectsOfType<FlyingEnemy>();
        collectibles = FindObjectsOfType<CollectibleScript>();
        GameObject.FindGameObjectWithTag("Music")?.GetComponent<menumusic>().StopMusic();
        nokeymsg.SetActive(false);
    }
    // Update is called once per frame
    IEnumerator RespawnCo()
    {
        player.gameObject.SetActive(false); // deactivates player
        yield return new WaitForSeconds(waitToRespawn); // how long to wait before respawning player
        player.gameObject.SetActive(true); // reactivates player
        UpdateHealthBar();
        UpdateHeatBar();
        player.transform.position = player.respawnpoint; // moves player to respawn point
        levelmusic.UnPause();
        //player.audio.UnPause();
        player.isdead = false;
        player.hasrespawned = false;
        deathscreen.SetActive(false);
        foreach (var i in enemies)
        {
            if (!i.gameObject.activeSelf) i.gameObject.SetActive(true);
            i.health = i.maxhealth;
        }
        foreach (var i in flyingEnemies)
        {
            if (!i.gameObject.activeSelf) i.gameObject.SetActive(true);
            i.health = i.maxhealth;
        }
        foreach (var i in collectibles)
        {
            if (i.gameObject.tag == "Health" && !i.gameObject.activeSelf) i.gameObject.SetActive(true);
        }
    }
    private void Update()
    {

        if (loreitem.activeSelf == false)
        {
            PlayerPrefs.SetInt(levellore, 1);
        }
        continuestext.text = "Continues: " + continues;
    }
    public void Respawn()
    {
        StartCoroutine(RespawnCo());
    }
    public void UpdateHealthBar()
    {
        switch (player.health)
        {
            case 5:
                healthBar.sprite = fullhealth;
                break;
            case 4:
                healthBar.sprite = fourhealth;
                break;
            case 3:
                healthBar.sprite = threehealth;
                break;
            case 2:
                healthBar.sprite = twohealth;
                break;
            case 1:
                healthBar.sprite = onehealth;
                break;
            case 0:
                healthBar.sprite = nohealth;
                break;
            default:
                healthBar.sprite = nohealth;
                break;
        }
    }
    public void UpdateHeatBar()
    {
        switch (player.heat)
        {
            case 6:
                heatBar.sprite = fullheat;
                secondHeatBar.sprite = fullheat;
                break;
            case 5:
                heatBar.sprite = fullheat;
                secondHeatBar.sprite = twoheat;
                break;
            case 4:
                heatBar.sprite = fullheat;
                secondHeatBar.sprite = oneheat;
                break;
            case 3:
                heatBar.sprite = fullheat;
                secondHeatBar.sprite = noheat;
                break;
            case 2:
                heatBar.sprite = twoheat;
                secondHeatBar.sprite = noheat;
                break;
            case 1:
                heatBar.sprite = oneheat;
                secondHeatBar.sprite = noheat;
                break;
            case 0:
                heatBar.sprite = noheat;
                secondHeatBar.sprite = noheat;
                break;
            default:
                heatBar.sprite = fullheat;
                secondHeatBar.sprite = fullheat;
                break;
        }
    }
}