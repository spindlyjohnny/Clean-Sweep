using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    public float movespeed;
    public float jumpspeed;
    public LayerMask realground;
    public Transform groundcheck;
    public float groundcheckradius;
    public bool isgrounded;
    private Rigidbody2D rb;
    private Animator anim;

    [Header("Dashing")]
    [SerializeField] private float _dashingVelocity = 25f;
    [SerializeField] private float _dashingTime = 0.1f;
    private Vector2 _dashingDir;
    private bool _isDashing;
    private bool _canDash = true;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update() {
        isgrounded = Physics2D.OverlapCircle(groundcheck.position, groundcheckradius, realground);

        if (Input.GetButtonDown("Jump") && isgrounded) {
            rb.velocity = new Vector2(0, jumpspeed);
        }

            if (Input.GetAxisRaw("Horizontal") > 0) {
            rb.velocity = new Vector2(movespeed, rb.velocity.y);
            transform.localScale = new Vector3(4.5f, 4.5f, 4.5f); // these scale values are not to be changed

        } else if (Input.GetAxisRaw("Horizontal") < 0) {
            rb.velocity = new Vector2(-movespeed, rb.velocity.y);
            transform.localScale = new Vector3(-4.5f, 4.5f, 4.5f);

        } else {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }

        var dashInput = Input.GetButtonDown("Dash");

        if (dashInput && _canDash /*&& isgrounded*/) {
            _isDashing = true;
            _canDash = false;
            _dashingDir = new Vector2(x: Input.GetAxisRaw("Horizontal"), y: Input.GetAxisRaw("Vertical"));
            StartCoroutine(routine:StopDashing());
        }

        anim.SetBool(name: "IsDashing", _isDashing);

        if (_isDashing) {
            rb.velocity = _dashingDir.normalized * _dashingVelocity;
            return;
        }

        if (isgrounded) {
            _canDash = true;
        }

        IEnumerator StopDashing() {
            yield return new WaitForSeconds(_dashingTime);
            _isDashing = false;
        }
    }
}
