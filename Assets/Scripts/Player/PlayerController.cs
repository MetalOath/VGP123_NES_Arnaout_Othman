using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(SpriteRenderer))]
public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    Animator anim;
    SpriteRenderer sr;

    
    public float speed;
    public int jumpForce;
    public bool isGrounded;
    public bool isClimbing = false;
    public bool lookingUp;
    public LayerMask isGroundLayer;
    public Transform groundCheck;
    public float groundCheckRadius;
    bool coroutineRunning = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        //Could be used as a double check to ensure groundcheck is set if we are null.
        if (!groundCheck)
        {
            groundCheck = GameObject.FindGameObjectWithTag("Ground Check").transform;
        }

        if (speed <= 0)
        {
            speed = 5.0f;
        }

        if (jumpForce <= 0)
        {
            jumpForce = 350;
        }

        if (groundCheckRadius <= 0)
        {
            groundCheckRadius = 0.2f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        AnimatorClipInfo[] curPlayingClip = anim.GetCurrentAnimatorClipInfo(0);

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, isGroundLayer);

        if (Input.GetButtonDown("Jump") && isGrounded && !lookingUp)
        {
            rb.velocity = Vector2.zero;
            rb.AddForce(Vector2.up * jumpForce);
            GameManager.instance.sfxManager.Play(GameManager.instance.jumpSound, GameManager.instance.soundFXGroup);
        }
        if (curPlayingClip.Length > 0)
        {
            if (curPlayingClip[0].clip.name == "Lookup")
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    anim.SetBool("combo", true);
                }
            }
            else if (curPlayingClip[0].clip.name != "Fire")
            {
                Vector2 moveDirection;
                if (verticalInput > 0.1 && isClimbing)
                {
                    moveDirection = new Vector2(horizontalInput * speed, verticalInput * speed);
                    anim.SetBool("isClimbing", true);
                }
                else
                {
                    moveDirection = new Vector2(horizontalInput * speed, rb.velocity.y);
                }
                rb.velocity = moveDirection;
            }
            else
            {
                rb.velocity = Vector2.zero;
            }
        }
        
        if (Input.GetButtonUp("Fire1") || verticalInput < 0.1)
        {
            anim.SetBool("combo", false);
        }

        if (isClimbing == false)
        {
            anim.SetBool("isClimbing", false);
        }

        anim.SetFloat("speed", Mathf.Abs(horizontalInput));
        anim.SetFloat("vert", verticalInput);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("lookUp", isGrounded);
        lookingUp = curPlayingClip[0].clip.name == "Lookup";


        //check for flipped
        //if (horizontalInput > 0 && sr.flipX || horizontalInput < 0 && !sr.flipX )
        //    sr.flipX = !sr.flipX;

        if (horizontalInput != 0)
            sr.flipX = (horizontalInput < 0);

        //sr.flipX = (horizontalInput < 0) ? true : (horizontalInput > 0) ? false : sr.flipX;

    }

    public void StartJumpForceChange()
    {
        if (!coroutineRunning)
        {
            StartCoroutine("JumpForceChange");
        }
        else
        {
            StopCoroutine("JumpForceChange");
            jumpForce /= 2;
            StartCoroutine("JumpForceChange");
        }

    }

    IEnumerator JumpForceChange()
    {
        coroutineRunning = true;
        jumpForce *= 2;

        yield return new WaitForSeconds(5.0f);

        jumpForce /= 2;
        coroutineRunning = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Squish")
        {
            collision.gameObject.GetComponentInParent<EnemyWalker>().IsSquished();

            rb.velocity = Vector2.zero;
            rb.AddForce(Vector2.up * jumpForce);
        }

        if (collision.gameObject.tag == "EnemyProjectile")
        {
            
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            GameManager.instance.lives--;
        }
    }
}
