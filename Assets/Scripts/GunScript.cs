using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour {
    public Transform firept; // fire point from which bullet is fired
    public GameObject bulletprefab; // normal shot
    public GameObject chargebullet; // charge shot
    public GameObject enemyprojectile; // enemy projectile 
    public LevelManagerScript levelManager; // reference to update heat bar
    public PlayerController player; // reference to control player shooting animation
    public float firerate;
    private float nextfiretime;
    // Update is called once per frame
    private void Start() {
        levelManager = FindObjectOfType<LevelManagerScript>();
        player = FindObjectOfType<PlayerController>();
    }
    void Update() {
        if (gameObject.tag != "Enemy") {
            if (Input.GetButtonDown("Fire1") && player.canMove ) {
                Fire();
            }
            if (Input.GetButtonUp("Fire1") && player.canMove && player.IsShooting) {
                player.IsShooting = false; // deactivates shooting animation
                player.audio.PlayOneShot(player.bulletSound);
            }
            if ((player.heat >= 3 && player.heat <= 6) && Input.GetButtonUp("Charge Shot")) {
                player.IsShooting = true; // activates shooting animation
                Instantiate(chargebullet, firept.position, firept.rotation); // generates bullet
                player.heat -= 3; // use up heat
                levelManager.UpdateHeatBar();
                player.IsShooting = false; // deactivate shooting animation
                player.audio.PlayOneShot(player.chargeshotSound); // play charge shot sound
            }
        }
    }
    private void FlyingEnemyAttack() { // flying enemy attack animation event
        Instantiate(enemyprojectile, firept.position, firept.rotation); // generates projectile
    }
    void Fire() {
        if (Time.time < nextfiretime) return;
        player.IsShooting = true; // activates shooting animation
        Instantiate(bulletprefab, firept.position, firept.rotation); // generates bullet
        nextfiretime = Time.time + firerate;
    }
}