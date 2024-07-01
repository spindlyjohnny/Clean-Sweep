using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    private float angle;
    private float rotatespeed = 5f;
    private float radius = 0.5f;
    private Vector2 centre;
    private Animator anim;
    private PlayerController player;
    public LayerMask IsPlayer; // checks player layer
    public Rigidbody2D rb;
    public int health,maxhealth;
    public bool IsAttacking; // Lines 11 - 12: activate animations
    public bool IsDead;
    public float enemyknockbackcounter; // countdown of time for enemy being knocked back 
    public float enemyknockbackforce;
    public float enemyknockbacklength; // amount of time enemy is knocked back
    public float detectionrange;
    public GameObject enemydeatheffect; // death particle effect
    public AudioSource audio;
    public AudioClip hitsound, attacksound;
    // Start is called before the first frame update
    void Start() {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        audio = GetComponent<AudioSource>();
        player = FindObjectOfType<PlayerController>();
        centre = transform.position;
        maxhealth = health;
    }

    // Update is called once per frame
    void Update() {
        Patrol();
        // Death
        if (health <= 0) {
            IsDead = true;
            anim.SetTrigger("Death");
            StartCoroutine(Death());
            Physics2D.IgnoreLayerCollision(gameObject.layer, player.gameObject.layer, true);
        } else {
            Physics2D.IgnoreLayerCollision(gameObject.layer, player.gameObject.layer, false);
        }
        var sightbox = Physics2D.OverlapBox(transform.position + new Vector3(detectionrange * Mathf.Sign(transform.localRotation.y) * 0.5f, 0f, 0f), new Vector2(detectionrange, 3f), 0, IsPlayer);
        if (sightbox && !player.isdead) {
            IsAttacking = true; // activate attack animation
        } else {
            IsAttacking = false;
        }
        anim.SetFloat("Speed", Mathf.Abs(transform.position.x));
        anim.SetBool("IsAttacking", IsAttacking);
        anim.SetBool("IsDead", IsDead);

    }
    private void OnDrawGizmos() {
        Gizmos.DrawWireCube(transform.position + new Vector3(detectionrange * Mathf.Sign(transform.localRotation.y) * 0.5f, 0f, 0f), new Vector2(detectionrange, 3f));
    }
    //  Player Projectile Collision Detection
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Projectile") {
            health -= 1;
            EnemyKnockback();
            audio.PlayOneShot(hitsound);
        } else if (collision.tag == "Charge Shot") {
            health -= 5;
            EnemyKnockback();
            audio.PlayOneShot(hitsound);
        }
    }
    private void Patrol() {
        if (enemyknockbackcounter <= 0) {
            angle += rotatespeed * Time.deltaTime;
            var offset = new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
            transform.position = centre + offset;
        }
        if (enemyknockbackcounter > 0) {
            enemyknockbackcounter -= Time.deltaTime; // count down time
            Vector2 knockbackdir = (transform.position - player.transform.position).normalized; // direction of knockback
            rb.velocity = knockbackdir * enemyknockbackforce;
        }
    }
    private IEnumerator Death() { // Death Code
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length); // waits to play death animation before deactivating the enemy
        gameObject.SetActive(false);
        Instantiate(enemydeatheffect, transform.position, Quaternion.identity);
    }
    public void EnemyKnockback() {
        enemyknockbackcounter = enemyknockbacklength;
    }
    private void Fall() {
        rb.velocity = Vector2.zero;
        rb.AddForce(Vector2.down * 25f, ForceMode2D.Force);
    }
    private void AttackSound() {
        audio.PlayOneShot(attacksound);
    }
}