using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour {
    public float speed = 20f; // speed of bullet
    public Rigidbody2D rb;
    public LevelManagerScript levelManager; // reference level manager to update heat bar
    public PlayerController player; // reference to increase heat
    public TrailRenderer trailRenderer; // reference to generate trail
    public GameObject impacteffect, chargeimpacteffect; // projectile particle effects 
    float trailtime = 3f; // time during which trail renders
    public CircleCollider2D hitbox;
    // Start is called before the first frame update
    void Start() {
        rb.velocity = transform.right * speed; // moves bullet
        Destroy(gameObject, 3f); // destroys bullet after 3 seconds
        trailRenderer.emitting = true; // emits trail when bullet is instantiated
        trailtime -= Time.deltaTime; // decrements time trail is active
        if (trailtime == 0) {
            trailRenderer.emitting = false;
        }
        levelManager = FindObjectOfType<LevelManagerScript>();
        player = FindObjectOfType<PlayerController>();
        trailRenderer = GetComponent<TrailRenderer>();
        hitbox = GetComponent<CircleCollider2D>();
    }
    private void OnTriggerEnter2D(Collider2D collision) {
        if (!hitbox.IsTouching(collision) || !(collision is BoxCollider2D)) return;
        if ((collision.tag == "Enemy" || collision.tag == "Mini Boss" || collision.tag == "Boss") && collision is BoxCollider2D && !(collision.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsTag("Death"))) {
            if (gameObject.tag == "Projectile") {
                player.heat += 1; // builds heat if successfully hit enemy
                levelManager.UpdateHeatBar();
                player.heat = Mathf.Clamp(player.heat, 0, 6); // limits heat to 6
                Destroy(Instantiate(impacteffect, transform.position, Quaternion.identity), 1f);
                Destroy(gameObject);
            } else if (gameObject.tag == "Charge Shot") { // checks if the projectile collider is colliding with the enemy's hitbox
                Destroy(gameObject, 7f);
                Destroy(Instantiate(chargeimpacteffect, transform.position, Quaternion.identity), 1f); // checks if the projectile collider is colliding with the enemy's hitbox
            }
        }
        }
    }

