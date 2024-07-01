using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleScript : MonoBehaviour {
    public PlayerController player; // reference player to access health variable
    public LevelManagerScript levelManager;
    public Vector3 rotationrate;
    private void Start() {
        player = FindObjectOfType<PlayerController>();
        levelManager = FindObjectOfType<LevelManagerScript>();
    }
    private void Update() {
        if(gameObject.tag == "Collectible") {
            transform.Rotate(rotationrate * Time.deltaTime);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.tag == "Player" ) {
            if(gameObject.tag == "Health" && player.health < player.maxhealth && !player.isdead) {
                player.health += 1;
                //player.health = Mathf.Min(player.maxhealth, player.health + 1);
                levelManager.UpdateHealthBar();
                levelManager.audio.PlayOneShot(levelManager.healthsoundeffect);
                gameObject.SetActive(false);
            } 
            else if (gameObject.tag == "Collectible") {
                levelManager.audio.PlayOneShot(levelManager.pickupsoundeffect);
                gameObject.SetActive(false);
            }
        }
    }
}
