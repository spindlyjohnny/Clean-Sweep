using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {
    private Animator anim;
    private PlayerController player;
    float dist;
    Collider2D sightbox;
    public LayerMask IsPlayer; // checks player layer
    public Rigidbody2D rb;
    public int health,maxhealth;
    public bool IsAttacking1; // Lines 11 - 13: activate animations
    public bool IsAttacking2;
    public bool IsDead;
    public float enemyknockbackcounter; // countdown of time for enemy being knocked back 
    public float enemyknockbackforce;
    public float enemyknockbacklength; // amount of time enemy is knocked back
    public float movespeed; // enemy move speed;
    public Transform groundcheck; // position from which ray is cast
    public float groundcheckradius; // distance of raycast
    public LayerMask realground; // layer to check if ray is in contact with
    public RaycastHit2D isgrounded; // raycast to check for ground
    public LayerMask traps; // trap layer to check if ray is in contact with
    public RaycastHit2D istrap; // raycast to check for traps
    public bool movingright = true; // checks for edge of platform
    public GameObject enemydeatheffect; // death particle effect
    public float attackdist; // minimum distance for attack
    public AudioSource audio;
    public AudioClip hitsound, attacksound, chargingsound;
    public float detectionrange,detectionboxheight;
    // Start is called before the first frame update
    void Start() {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        audio = GetComponent<AudioSource>();
        player = FindObjectOfType<PlayerController>();
        maxhealth = health;
    }

    // Update is called once per frame
    void Update() {
        StartCoroutine(Patrol());
        dist = Vector2.Distance(transform.position, player.transform.position); // distance between enemy and player
        // Death
        if (health <= 0) {
            StartCoroutine(Death());
        } 
        else {
            Physics2D.IgnoreLayerCollision(gameObject.layer, player.gameObject.layer, false);
        }
        // Attacking Behaviour
        if (gameObject.tag == "Mini Boss" && dist <= attackdist) {
            IsAttacking1 = true;
        } 
        else if(gameObject.tag == "Mini Boss" && dist > attackdist) {
            IsAttacking1 = false;
        }
        else {
            sightbox = Physics2D.OverlapBox(transform.position + new Vector3(detectionrange * Mathf.Sign(transform.localRotation.y) * 0.5f, 0f, 0f), new Vector2(detectionrange, detectionboxheight), 0, IsPlayer);
            if (sightbox && dist <= attackdist && !player.isdead) { // check if player is in close range
                IsAttacking1 = true; // activate close range attack animation
            } 
            else if (sightbox && dist > attackdist && !player.isdead) { // checks if player is in long range
                IsAttacking2 = true; // activate long range attack animation
                if (!(dist > attackdist)) {
                    IsAttacking2 = false;
                }
            } 
            else if (!sightbox) {
                IsAttacking1 = false; // deactivates attack animation when player is no longer within sight of enemy
                IsAttacking2 = false;
            }
        }
        anim.SetFloat("Speed", Mathf.Abs(transform.position.x));
        anim.SetBool("IsAttacking1", IsAttacking1);
        anim.SetBool("IsAttacking2", IsAttacking2);
        anim.SetBool("IsDead", IsDead);
    }
    private void OnDrawGizmos() {
        Gizmos.DrawWireCube(transform.position + new Vector3(detectionrange * Mathf.Sign(transform.localRotation.y) * 0.5f, 0f, 0f), new Vector2(detectionrange, detectionboxheight));
    }
    // Collision Detection
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Projectile") {
            health -= 1;
            EnemyKnockback();
            audio.PlayOneShot(hitsound);
        } else if (collision.tag == "Charge Shot") {
            health -= 5;
            EnemyKnockback();
            audio.PlayOneShot(hitsound);
        } else if (collision.name == "acid") health = 0;
    }
    private IEnumerator Patrol() {   
        yield return new WaitForSeconds(2.1f); // waits to play spawn animation
       if(enemyknockbackcounter <= 0) {
            if (Vector2.Distance(player.transform.position, gameObject.transform.position) > 1) { // checks if player is not in attack range
                transform.Translate(Vector2.right * movespeed * Time.deltaTime); // moves enemy
            }
            isgrounded = Physics2D.Raycast(groundcheck.position, Vector2.down, groundcheckradius, realground); // raycast casts a ray of length groundcheckradius downwards at position of grouncheck, and checks if collider that ray hits is in ground layer.
            istrap = Physics2D.Raycast(groundcheck.position, Vector2.down, groundcheckradius, traps);
            if (isgrounded.collider == false || istrap.collider == true) { // ray no longer detects collider or detects traps
                if (movingright) {
                    transform.eulerAngles = new Vector3(0f, 180f, 0f); // flips enemy
                    movingright = false;
                } else {
                    transform.eulerAngles = new Vector3(0f, 0f, 0f);
                    movingright = true;
                }
            }
        }
        if (enemyknockbackcounter > 0) {
            enemyknockbackcounter -= Time.deltaTime; // count down time
            Vector2 knockbackdir = (transform.position - player.transform.position).normalized; // direction of knockback
            rb.velocity = knockbackdir * enemyknockbackforce;
        }
    }
    public void EnemyKnockback() {
        enemyknockbackcounter = enemyknockbacklength;
    }
    private void LongRangeAttack() { // long range attack animation event
        rb.AddForce(Vector2.right * 3 * (player.transform.position - transform.position).normalized,ForceMode2D.Impulse);
    }
    private void PlayAttack1Sound() {
        audio.PlayOneShot(attacksound);
    }
    private void PlayAttack2Sound() { // charging attack animation event
        audio.PlayOneShot(chargingsound);
    }
    private IEnumerator Death() {
        Physics2D.IgnoreLayerCollision(gameObject.layer, player.gameObject.layer, true);
        IsDead = true;
        anim.SetTrigger("Death");
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length + anim.GetCurrentAnimatorStateInfo(0).normalizedTime);
        gameObject.SetActive(false);
        Instantiate(enemydeatheffect, transform.position, Quaternion.identity);
    }
}

