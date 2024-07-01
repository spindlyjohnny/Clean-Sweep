using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour {
    public float speed = 2f; // speed of projectile
    public Rigidbody2D rb;
    public CircleCollider2D hitbox;
    AudioSource impactsound;
    Animator anim;
    // Start is called before the first frame update
    void Start() {
        rb.velocity = transform.right * speed; // moves projectile
        Destroy(gameObject,0.75f); // destroys projectile after 0.75 seconds
        hitbox = GetComponent<CircleCollider2D>();
        anim = GetComponent<Animator>();
        impactsound = GetComponent<AudioSource>();
    }
    private void OnTriggerEnter2D(Collider2D collision) {
        if (!hitbox.IsTouching(collision) || !(collision is BoxCollider2D)) {
            return;
        } else if (collision.tag == "Player") {
            anim.SetTrigger("Hit");
            rb.velocity = Vector2.zero;
            impactsound.Play();
        }
    }
}
