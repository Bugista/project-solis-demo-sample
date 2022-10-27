using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rb;
    BoxCollider2D bc;
    private Animator anim;

    private GameObject audioSource;
    private AudioSource audioJumpRef;

    public float moveSpeed = 5f; //How fast Player moves
    public float jumpForce = 5f; //How high Player jumps
    public static bool isGrounded = false; //True when Player is standing on Ground tagged object

    public Transform basicAttackPoint;

    public Transform isGroundedCheckerBack;
    public Transform isGroundedCheckerMiddle;
    public Transform isGroundedCheckerFront;

    Vector2 bapOGoffset;
    Vector2 bapOGPosition;
    Vector2 bcOGoffset;
    Vector2 bcOGsize;

    public float checkGroundRadius; //Radius of the ground checker
    public LayerMask groundLayer;

    public bool isJumping = false; //True when Player is jumping
    public bool facingRight = true; //True when Player is facing right
    float moveX = 0; //Player movement in horizontal direction, positive when right
    float moveXFinal = 0;
    float moveBy; //Player movement input * speed

    public float fallMultiplier = 2.5f; //How many times faster Player falls after jump peak is reached
    public float lowJumpMultiplier = 2f; //Affects jump height: long press vs short tap

    public float rememberGroundedFor; //How many seconds after leaving ground player can still jump (coyote time)
    float lastTimeGrounded; //For checking time for coyote time

    public bool jumped = false; //True when Player jumps

    public float jumpRate = 2f; //How many jump Player can make in a second
    float nextJumpTime = 0f; //For checking when Player can jump again

    float jumpPressedRemember; //For checking time for when Player press Jump button (jump buffer)
    public float jumpPressedRememberTime = 0.2f; //How many seconds after Player press Jump button can jump happen if Plsyer is on ground before times up

    public bool isCrouching = false;

    Vector2 size = new Vector2(1, 1);

    private KnockbackManager knockbackManager;

    bool activateJumpRb = false;

    float controllerVertical;

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();

        knockbackManager = GetComponent<KnockbackManager>();

        bapOGoffset = basicAttackPoint.GetComponent<CircleCollider2D>().offset;

        bapOGPosition = basicAttackPoint.localPosition;

        bcOGoffset = bc.offset;
        bcOGsize = bc.size;

        audioSource = GameObject.Find("SoundController");
        audioJumpRef = audioSource.GetComponent<SoundController>().audioJump;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.gameIsPaused)
        {

            controllerVertical = Input.GetAxis("Vertical");

            CheckIfGrounded();

            jumpPressedRemember -= Time.deltaTime;

            //Player can't move or jump when Player is attacking and on the ground with fire1, and not even on ground with fire2 and air1
            if ((PlayerAttackFireOne.isPlayerAttackingFireOne || PlayerEarthOne.isPlayerAttackingEarthOne || PlayerEarthTwo.isPlayerAttackingEarthTwo || PlayerIceTwo.isPlayerAttackingIceTwo) && isGrounded)
            {
                rb.velocity = Vector2.zero;
                return;
            }
            else if (FindObjectOfType<DialogueManager>().canJumpAfterDialogue == false)
            {
                anim.SetBool("isJumping", false);
                moveBy = 0;
                anim.SetFloat("Speed", Mathf.Abs(moveBy));
                return;
            }
            else if (PlayerAttackFireTwo.isPlayerAttackingFireTwo || PlayerAttackAirOne.isPlayerAttackingAirOne || PlayerAttackAirTwo.isPlayerAttackingAirTwo)
            {
                return;
            }
            else if (PlayerIceTwo.iceTwoActive)
            {
                Move();
                Flip(moveX);
            }
            else if (!knockbackManager.IsKnockedback())
            {
                Jump();
                Move();
                Flip(moveX);
                Crouch();
            }

            if (Time.time >= nextJumpTime)
            {
                jumped = false;
            }

            if (jumped == false) { }

            if (!jumped && isGrounded)
            {
                isJumping = false;
                anim.SetBool("isJumping", false);
            }

            if (PlayerAttackFireOne.isPlayerAttackingFireOne || PlayerAttackFireTwo.isPlayerAttackingFireTwo || PlayerAttackAirOne.isPlayerAttackingAirOne || PlayerAttackIceOne.isPlayerAttackingIceOne || PlayerEarthOne.isPlayerAttackingEarthOne || PlayerEarthTwo.isPlayerAttackingEarthTwo || PlayerArcticFox.isFoxActive || PlayerIceTwo.iceTwoActive)
            {
                isCrouching = false;
            }

            if (!isCrouching)
            {
                anim.SetBool("isCrouching", false);
                basicAttackPoint.localPosition = new Vector2(bapOGPosition.x, bapOGPosition.y);
                bc.offset = bcOGoffset;
                bc.size = new Vector2(bcOGsize.x, bcOGsize.y);
            }
            else
            {
                basicAttackPoint.localPosition = new Vector2(bapOGPosition.x, -1.69f);
                //change box size
                bc.offset = new Vector2(bcOGoffset.x, -1.625848f);
                bc.size = new Vector2(bcOGsize.x, 1.747938f);
            }

        }
    }

    void FixedUpdate()
    {
        if (!PlayerAttackAirOne.isPlayerAttackingAirOne)
        {
            BetterJump();
        }

        if ((PlayerAttackFireOne.isPlayerAttackingFireOne || PlayerEarthOne.isPlayerAttackingEarthOne || PlayerEarthTwo.isPlayerAttackingEarthTwo || PlayerIceTwo.isPlayerAttackingIceTwo) && isGrounded)
        {
            return;
        }
        else if (PlayerAttackFireTwo.isPlayerAttackingFireTwo || PlayerAttackAirOne.isPlayerAttackingAirOne || PlayerAttackAirTwo.isPlayerAttackingAirTwo)
        {
            return;
        }
        else if (PlayerIceTwo.iceTwoActive)
        {
            MoveRb();
        }
        else if (!knockbackManager.IsKnockedback())
        {
            MoveRb();
        }

        if (activateJumpRb)
        {
            activateJumpRb = false;
            JumpRb();
        }
    }

    //Player's movement to left or right
    void Move()
    {
        moveX = Input.GetAxisRaw("Horizontal");

        if (moveX > 0.05)
        {
            moveXFinal = 1;
        }
        else if (moveX < -0.05)
        {
            moveXFinal = -1;
        }
        else
        {
            moveXFinal = 0;
        }

        if (!isCrouching)
        {
            moveBy = moveXFinal * moveSpeed;
        }
        else
        {
            moveBy = 0;
        }

        anim.SetFloat("Speed", Mathf.Abs(moveBy));
    }

    void MoveRb()
    {
        if (!isCrouching)
        {
            rb.velocity = new Vector2(moveBy, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    //Player's jumping action
    void Jump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            jumpPressedRemember = jumpPressedRememberTime;
        }

        if ((jumpPressedRemember > 0) && (isGrounded == true || Time.time - lastTimeGrounded <= rememberGroundedFor) && jumped == false)
        {
            if (audioJumpRef != null)
            {
                audioJumpRef.Play();
            }

            activateJumpRb = true;
            jumped = true;
            nextJumpTime = Time.time + 1f / jumpRate;

            isCrouching = false;
            isJumping = true;
            anim.SetBool("isJumping", true);
        }
    }

    void JumpRb()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    void Crouch()
    {
        if ((Input.GetButton("Crouch") || controllerVertical < 0) && isGrounded)
        {
            isCrouching = true;
            anim.SetBool("isCrouching", true);
            transform.hasChanged = false;
        }
        else
        {
            isCrouching = false;
        }
    }

    //Changes Players sprite direction depending on which way Player moves
    void Flip(float moveX)
    {
        if (moveX > 0 && !facingRight || moveX < 0 && facingRight)
        {
            facingRight = !facingRight;

            transform.Rotate(0f, 180f, 0f);
        }
    }

    //Used when moving left to the next scene (like from cave to forest)
    public void FlipWhenSceneStart()
    {
        facingRight = !facingRight;

        transform.Rotate(0f, 180f, 0f);
    }

    //Checks is Player standing on the ground
    void CheckIfGrounded()
    {
        Collider2D colliderBack = Physics2D.OverlapCircle(isGroundedCheckerBack.position, checkGroundRadius, groundLayer);

        Collider2D colliderMiddle = Physics2D.OverlapCircle(isGroundedCheckerMiddle.position, checkGroundRadius, groundLayer);

        Collider2D colliderFront = Physics2D.OverlapCircle(isGroundedCheckerFront.position, checkGroundRadius, groundLayer);

        if (colliderBack != null || colliderMiddle != null || colliderFront != null)
        {
            isGrounded = true;
        }
        else
        {
            if (isGrounded == true)
            {
                lastTimeGrounded = Time.time;
            }
            isGrounded = false;
        }
    }

    //Makes Player jump higher if Jump button is pressed longer and also makes Player's jump go faster down than going up (fallMultiplier)
    void BetterJump()
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && PlayerAttackFireTwo.isPlayerAttackingFireTwo)
        {
            rb.velocity += Vector2.up * Physics2D.gravity * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.velocity += Vector2.up * Physics2D.gravity * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    //For editor only, makes Player's ground checker visible
    void OnDrawGizmosSelected()
    {
        if (isGroundedCheckerBack == null || isGroundedCheckerMiddle == null || isGroundedCheckerFront == null)
        {
            return;
        }

        Gizmos.DrawWireSphere(isGroundedCheckerBack.position, checkGroundRadius);
        Gizmos.DrawWireSphere(isGroundedCheckerMiddle.position, checkGroundRadius);
        Gizmos.DrawWireSphere(isGroundedCheckerFront.position, checkGroundRadius);
    }
}