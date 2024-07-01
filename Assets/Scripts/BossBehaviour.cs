using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossBehaviour : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private LevelManagerScript levelManager;
    private PlayerController player;
    public BoxCollider2D box;
    [SerializeField]
    private Transform[] phase1routes, phase2routes, phase3routes;
    private int routeToGo;
    [SerializeField]
    public float speedModifier;
    public bool mvmtAllowed;
    public int health;
    public bool isattacking;
    public bool isattacking2;
    public bool isattacking3;
    //public bool isdead;
    public float enemyknockbackcounter; // countdown of time for enemy being knocked back 
    public float enemyknockbackforce;
    public float enemyknockbacklength; // amount of time enemy is knocked back
    public float attackdist;
    public GameObject enemydeatheffect, trigger, loreitem,dustparticle, glow; // death particle effect, level exit trigger, lore collectible, victory text, ground slam dust particle effect, glow
    public AudioSource audio;
    public AudioClip hitsound, attack1sound, attack2sound, attack3sound;
    private RaycastHit2D IsPlayerinView; // raycast to detect if player in view
    public LayerMask IsPlayer; // checks player layer
    // Start is called before the first frame update
    void Start()
    {
        levelManager = FindObjectOfType<LevelManagerScript>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        audio = GetComponent<AudioSource>();
        player = FindObjectOfType<PlayerController>();
        routeToGo = 0;
        mvmtAllowed = false;
        Physics2D.IgnoreLayerCollision(8, 3);
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            mvmtAllowed = false;
            rb.velocity = Vector2.zero;
            anim.SetTrigger("Death");
            StartCoroutine(Death());
            levelManager.levelmusic.Stop();
            levelManager.bosshealth.gameObject.SetActive(false);
            levelManager.bossname.gameObject.SetActive(false);
            levelManager.audio.PlayOneShot(levelManager.bossdeathsound);
        }
        if (player.hasrespawned) {
            health = 20;
        }
        if (enemyknockbackcounter <= 0)
        {
            //check that we are not currently charging downwards (If that is the case, isattacking3 would have been true)
            if (!isattacking3 && mvmtAllowed)
            {

                //get what set of route positions we should be following right now
                var currentRoute = phase1routes;
                isattacking = true;
                if (health <= 15 && health > 10)
                {
                    currentRoute = phase2routes; //if our health is between 10-15, we will follow phase2routes
                    isattacking = false;
                }
                else if (health <= 10)
                {
                    currentRoute = phase3routes; //if our health is <10, we follow phase3routes
                    isattacking2 = false;
                    isattacking = false;
                }

                Vector2 targetPosition = currentRoute[routeToGo].position;
                //get the direction from our current position to the targetPosition variable we just stored (In radians)
                float direction = (float)Mathf.Atan2(targetPosition.y - transform.position.y, targetPosition.x - transform.position.x);
                //get the velocity in order to move at scalar speed <speedModifier> in direction <direction> (towards the targetPosition)
                Vector2 velocity = new Vector2(Mathf.Cos(direction) * speedModifier, Mathf.Sin(direction) * speedModifier);
                if (mvmtAllowed) rb.velocity = velocity;
                //if we have reached our target position (By estimation of distance), we will increase the current route position by 1,
                //thus progressing the movement to the next target position
                if (Vector2.Distance(transform.position, targetPosition) <= 0.5f)
                {
                    routeToGo++;
                    if (routeToGo >= currentRoute.Length)
                    {
                        routeToGo = 0; //if we are at the last position in the route, we loop back to the first position by setting routeToGo to 0
                    }
                }
                
            }
        }
        if (enemyknockbackcounter > 0)
        {
            enemyknockbackcounter -= Time.deltaTime; // count down time
            Vector2 knockbackdir = (transform.position - player.transform.position).normalized; // direction of knockback
            rb.velocity = knockbackdir * enemyknockbackforce;
        }
        anim.SetBool("IsAttacking", isattacking);
        anim.SetFloat("Speed", speedModifier);
        anim.SetBool("IsAttacking2", isattacking2);
        anim.SetBool("IsAttacking3", isattacking3);
        levelManager.bosshealth.value = health;
        //these are hitmasks. These basically dictate what layer we want to check for collision with
        var playerHitMask = LayerMask.GetMask("Player");
        var groundHitMask = LayerMask.GetMask("Ground");
        var groundraycast = Physics2D.Raycast(transform.position, player.transform.position - transform.position, Vector2.Distance(transform.position, player.transform.position), groundHitMask);
        var downwardbox = Physics2D.OverlapBox((Vector2)transform.position + new Vector2(0, -15), new Vector2(2.627115f, 30), 0f, playerHitMask);
        var sidewaybox = Physics2D.OverlapBox(transform.position, new Vector2(40, 2.229366f), 0f, playerHitMask);
        //check if the player (or any object with the layer 'Player' is within a box directly underneath us
        if (downwardbox)
        {
            //if there is the player underneath us, check if there is line of sight between us and the player by using a raycast
            //raycasts function on a basis of vector direction, which can be calculated by subtracting our current position from the target's position (in this case, the player's)
            if (!groundraycast)
            {
                //if so, set our attacks to true based on our current health
                if (health <= 10)
                {
                    isattacking3 = true;
                }
            }
        } else if(sidewaybox && health <= 15 && health > 10) {
            isattacking = false;
            isattacking2 = true;
        }
        
    }
    private void OnDrawGizmos() {
        Gizmos.DrawWireCube(transform.position, new Vector3(40, 2.229366f,0));
        Gizmos.DrawWireCube(transform.position + new Vector3(0, -15, 0), new Vector3(2.627115f, 30, 0));
    }
    private IEnumerator Death()
    { // Death Code
        Physics2D.IgnoreLayerCollision(gameObject.layer, player.gameObject.layer);
        rb.velocity = new Vector2(rb.velocity.x, -0.177f) * 3;
        yield return new WaitForSeconds(5.3f); // waits to play death animation before deactivating the boss
        gameObject.SetActive(false);
        Instantiate(enemydeatheffect, transform.position, Quaternion.identity);
        Instantiate(trigger, transform.position, Quaternion.identity);
        Instantiate(glow, transform.position +new Vector3(0, 0, -0.74f), Quaternion.identity);
        loreitem.SetActive(true);
    }
    public void EnemyKnockback()
    {
        enemyknockbackcounter = enemyknockbacklength;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Projectile" && !(collision.GetComponent<CircleCollider2D>().IsTouching(box)))
        {
            health -= 1;
            EnemyKnockback();
            audio.PlayOneShot(hitsound);
        }
        else if (collision.tag == "Charge Shot" && collision.GetComponent<CircleCollider2D>().IsTouching(box))
        {
            health -= 5;
            EnemyKnockback();
            audio.PlayOneShot(hitsound);
        }

    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            if (isattacking)
            {
                isattacking = false;
                
            }
            else if (isattacking2)
            {
                isattacking2 = false;
            }
            else
            {
                isattacking3 = false;
            }
        }
    }
    public void EndAttack()
    {
        isattacking = false;
        isattacking2 = false;
        isattacking3 = false;
    }
    private void GroundSlam()
    {
        rb.AddForce(Vector2.down * 20,ForceMode2D.Impulse);
        audio.PlayOneShot(attack3sound);
    }
    private void PlayAttack1Sound() {
        audio.PlayOneShot(attack1sound);
    }
    private void PlayAttack2Sound() {
        audio.PlayOneShot(attack2sound);
    }
    private void Dust() {
        Destroy(Instantiate(dustparticle, transform.position, Quaternion.identity), 1f);
    }
}