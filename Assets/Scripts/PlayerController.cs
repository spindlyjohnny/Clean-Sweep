using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float movespeed; // player move speed
    public float jumpspeed; // player jump height
    public LayerMask realground; // Lines 9 - 12: These variables are to check if player is on the ground
    public Transform groundcheck;
    public float groundcheckradius;
    public bool isgrounded;
    public bool IsHit; // Lines 14 - 17: These bools are here to activate player animations
    public bool IsCharging;
    public bool IsShooting;
    public bool isdead;
    public Transform wallcheck; // check if player touching wall to wall jump
    public int health = 5, maxhealth;
    public int heat; // stat that builds up to activate charge shot
    [Header("Dashing")]
    [SerializeField] private float _dashingVelocity = 20f; //dash speed
    [SerializeField] private float _dashingTime = 1f; //dash time
    private Vector2 _dashingDir; //dash direction
    private bool _isDashing;
    private bool _canDash = true;
    private float nextdashtime;
    public LevelManagerScript levelmanager; // reference to level manager
    public BulletScript BulletScript;
    [HideInInspector] public Vector3 respawnpoint; //  where player respawns after death
    public float walljumpduration; // how long the player sticks to a wall
    public Vector2 walljumpforce; // wall jump height
    public float knockbackforce;
    public float knockbacklength; // amount of time player is knocked back
    public bool canMove = true;
    public float wallslidingspeed;
    public AudioClip bulletSound, chargeshotSound, hitSound, jumpSound, dashSound, deathsound; // sound effects
    public AudioSource audio, chargingSound;
    public GameObject playerdeatheffect; // death particle effect
    public Rigidbody2D rb;
    public bool hasrespawned = false;
    private float knockbackcounter; // countdown of time for player being knocked back 
    private bool walljumping;
    private bool doublejump; // checks if player can double jump
    private bool istouchingwall;
    private bool issliding;
    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        audio = GetComponent<AudioSource>();
        respawnpoint = transform.position;
        levelmanager = FindObjectOfType<LevelManagerScript>();
        BulletScript = FindObjectOfType<BulletScript>();
        //levelmanager.UpdateHeatBar();
        heat = 0;
        maxhealth = health;
        Physics2D.IgnoreLayerCollision(6, 10); // ignores collsions between player and invisible walls
    }

    IEnumerator StopDashing()
    {
        yield return new WaitForSeconds(_dashingTime);
        gameObject.GetComponent<BoxCollider2D>().isTrigger = false;
        //Physics2D.IgnoreLayerCollision(6, 8, false);
        //Physics2D.IgnoreLayerCollision(6, 9, false);
        _isDashing = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (knockbackcounter <= 0 && canMove && !anim.GetCurrentAnimatorStateInfo(0).IsName("PlayerDeath"))
        { // if there is no knockback and game is not paused
            // Player Movement
            if (Input.GetAxisRaw("Horizontal") > 0)
            {
                rb.velocity = new Vector2(movespeed, rb.velocity.y);
                transform.localRotation = Quaternion.Euler(0f, 0f, 0f); // player facing right
            }
            else if (Input.GetAxisRaw("Horizontal") < 0)
            {
                rb.velocity = new Vector2(-movespeed, rb.velocity.y);
                transform.localRotation = Quaternion.Euler(0f, 180f, 0f); // player facing left
            }
            else
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
            // Jumping
            if (Input.GetButtonDown("Jump") && isgrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpspeed);
                audio.PlayOneShot(jumpSound);
                doublejump = true;
            }
            //Double jump
            else if (Input.GetButtonDown("Jump") && doublejump)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpspeed / 1.05f);
                audio.PlayOneShot(jumpSound);
                doublejump = false;
            }
            // Wall jumping
            else if (Input.GetButtonDown("Jump") && istouchingwall)
            {
                walljumping = true;
                Invoke("StopWallJumping", walljumpduration);
                if (walljumping)
                {
                    rb.velocity = new Vector2(0, walljumpforce.y);
                    audio.PlayOneShot(jumpSound);
                }
            }
            if (istouchingwall && !isgrounded && Input.GetAxisRaw("Horizontal") != 0)
            {
                issliding = true;
            }
            else
            {
                issliding = false;
            }
            if (issliding)
            {
                rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallslidingspeed, float.MaxValue));
            }
            // Dashing
            if (Input.GetButtonDown("Dash") && _canDash)
            {
                _isDashing = true;
                audio.PlayOneShot(dashSound);
                _canDash = false;
                _dashingDir = new Vector2(Input.GetAxisRaw("Horizontal"), 0);
                StartCoroutine(StopDashing());
            }

            if (_isDashing)
            {
                anim.SetBool("IsDashing", true);
                rb.velocity = _dashingDir.normalized * _dashingVelocity;
                gameObject.GetComponent<BoxCollider2D>().isTrigger = true;
                //Physics2D.IgnoreLayerCollision(layerPlayer, layerEnemy, true);
                //Physics2D.IgnoreLayerCollision(layerPlayer, layerTraps, true);
                return;
            }

            if (/*isgrounded && */(Mathf.Abs(rb.velocity.magnitude) > 0 || _isDashing) && Time.time >= nextdashtime)
            { // checks if player is moving or dashing(to dash again)
                _canDash = true;
                nextdashtime = Time.time + _dashingTime;
            }
            if (!_isDashing)
            {
                // Charge Shot Animation and Sound
                if (Input.GetButton("Charge Shot") && (heat >= 3 && heat <= 6))
                {
                    IsCharging = true;
                }
                else
                {
                    IsCharging = false;
                }
                if (Input.GetButtonDown("Charge Shot"))
                {
                    chargingSound.Play();
                }
                else if (Input.GetButtonUp("Charge Shot"))
                {
                    chargingSound.Stop();
                }
            }
            // Death
            if (health <= 0)
            {
                Death();
            }
            if (isdead && Input.GetKeyDown(KeyCode.X))
            {
                if (!hasrespawned)
                {
                    levelmanager.continues -= 1;
                    levelmanager.Respawn();
                    health = maxhealth;
                    heat = 0;
                    hasrespawned = true;
                }
            }
        }
        if (knockbackcounter > 0)
        {
            knockbackcounter -= Time.deltaTime; // count down time
            if (transform.localRotation.y == 0)
            {
                rb.velocity = new Vector3(-knockbackforce, knockbackforce, 0.0f); // push player back
            }
            else
            {
                rb.velocity = new Vector3(knockbackforce, knockbackforce, 0.0f);
            }
        }
        if (levelmanager.continues == 0 && !levelmanager.deathscreen.activeSelf) {
            SceneManager.LoadScene("Game Over Screen");
        }
        isgrounded = Physics2D.OverlapCircle(groundcheck.position, groundcheckradius, realground); // checks if player is on the ground
        istouchingwall = Physics2D.OverlapCircle(wallcheck.position, groundcheckradius, realground); // check if player is touching wall

        anim.SetBool("IsDashing", _isDashing);
        anim.SetFloat("Speed", Mathf.Abs(rb.velocity.x));

        anim.SetBool("IsCharging", IsCharging);
        anim.SetBool("IsShooting", IsShooting);
        anim.SetBool("Ground", isgrounded);
    }

    private void StopWallJumping()
    {
        walljumping = false;
    }
    private IEnumerator WaitForDeathAnimation()
    {
        yield return new WaitForSeconds(0.01f);
        Destroy(Instantiate(playerdeatheffect, transform.position, Quaternion.identity), 1f);
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length + anim.GetCurrentAnimatorStateInfo(0).normalizedTime);
        levelmanager.deathscreen.SetActive(true);

    }
    private void Death()
    {
        if (!isdead)
        {
            isdead = true;
            anim.SetTrigger("PlayerDeath");
            levelmanager.levelmusic.Pause();
            // deactivate all animations
            IsHit = false;
            IsCharging = false;
            IsShooting = false;
            StartCoroutine(WaitForDeathAnimation());
            StartCoroutine(PlayDeathSound());
        }
    }
    // Collision Detection
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "Death Zone" || collision.name == "acid")
        {
            StartCoroutine(waitbeforerespawn());
        }
        else if (collision.tag == "Trap" && !isdead && !_isDashing)
        {
            anim.SetTrigger("Hit");
            health -= 1;
            levelmanager.UpdateHealthBar();
            Knockback();
            audio.PlayOneShot(hitSound);
        }
        else if (collision.tag == "Enemy" && collision is CircleCollider2D && !isdead && !_isDashing)
        { // collision with enemy projectiles
            anim.SetTrigger("Hit");
            health -= 1;
            levelmanager.UpdateHealthBar();
            Knockback();
            audio.PlayOneShot(hitSound);
        }
        else if (collision.tag == "Respawn")
        {
            respawnpoint = collision.transform.position; // activates checkpoints
        }
        else if (collision.transform.tag == "Boss" && !isdead && !_isDashing)
        {
            anim.SetTrigger("Hit");
            health -= 1;
            levelmanager.UpdateHealthBar();
            Knockback();
            audio.PlayOneShot(hitSound);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "Enemy" && !isdead && !_isDashing && !(collision.gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsTag("Death")))
        {
            anim.SetTrigger("Hit");
            health -= 1;
            levelmanager.UpdateHealthBar();
            Knockback();
            audio.PlayOneShot(hitSound);
        }
        if (collision.transform.tag == "Mini Boss" && !isdead && !_isDashing && !(collision.gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsTag("Death")))
        {
            anim.SetTrigger("Hit");
            health -= 2;
            levelmanager.UpdateHealthBar();
            Knockback();
            audio.PlayOneShot(hitSound);
        }
    }
    public void Knockback()
    {
        knockbackcounter = knockbacklength;
    }
    private IEnumerator PlayDeathSound()
    {
        audio.PlayOneShot(deathsound);
        yield return new WaitForSeconds(deathsound.length);
    }
    IEnumerator waitbeforerespawn() {
        yield return new WaitForSeconds(.5f);
        levelmanager.Respawn();
    }
}
