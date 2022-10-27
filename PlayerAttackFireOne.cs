using UnityEngine;

public class PlayerAttackFireOne : MonoBehaviour
{
    private Animator anim;

    private GameObject audioSource;
    private AudioSource audioFireOneRef;

    private GameObject dialogueManager;
    private DialogueManager dm;

    public GameObject fireOnePoint;

    public Transform attackPoint;
    public LayerMask enemyLayers;
    public LayerMask burnableLayers;

    public static bool isPlayerAttackingFireOne = false;

    public bool isFireOneOrbTaken = false;

    public float attackRange = 0.5f;
    public int attackDamage = 1;
    public float attackRate = 2f; //Allow how many attacks per second
    float nextAttackTime = 0f;

    bool fireTriggerLast;
    bool fireTriggerPressed = false;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animator>();

        audioSource = GameObject.Find("SoundController");
        audioFireOneRef = audioSource.GetComponent<SoundController>().audioFireOne;

        dialogueManager = GameObject.Find("DialogueManager");
        dm = dialogueManager.GetComponent<DialogueManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.gameIsPaused)
        {
            if (isFireOneOrbTaken)
            {
                //Do when enough time has passed from previous Fire 1 attack
                if (Time.time >= nextAttackTime)
                {
                    isPlayerAttackingFireOne = false;

                    anim.SetBool("isAttackingFireOne", false);

                    //Controller trigger input
                    float controllerVertical = Input.GetAxis("Vertical");

                    //Make controller trigger work like button
                    if (Input.GetAxisRaw("Fire") > 0)
                    {
                        if (fireTriggerLast == false)
                        {
                            fireTriggerPressed = true;
                        }
                        fireTriggerLast = true;
                    }
                    else
                    {
                        fireTriggerLast = false;
                    }

                    if (!dm.dialogueActive || dm.orbFireOneDialogue)
                    {
                        //Make using Fire 1 skill possible if no cooldown active and also no other attacks are active
                        if (!gameObject.GetComponent<PlayerController>().isCooldownActive && !PlayerAttackBasic.isPlayerAttacking && !PlayerAttackFireTwo.isPlayerAttackingFireTwo && !PlayerAttackAirOne.isPlayerAttackingAirOne && !PlayerAttackAirTwo.isPlayerAttackingAirTwo && !PlayerAttackIceOne.isPlayerAttackingIceOne && !PlayerEarthOne.isPlayerAttackingEarthOne && !PlayerEarthTwo.isPlayerAttackingEarthTwo && !PlayerArcticFox.isFoxActive && !PlayerIceTwo.iceTwoActive)
                        {
                            if ((Input.GetButtonDown("Fire") || fireTriggerPressed) && !this.gameObject.GetComponent<PlayerMovement>().isCrouching)
                            {
                                FireAttackOne();
                                fireTriggerPressed = false;
                                nextAttackTime = Time.time + 1f / attackRate;
                            }
                            else
                            {
                                fireTriggerPressed = false;
                            }
                        }
                        else
                        {
                            fireTriggerPressed = false;
                        }
                    }
                }
            }

            if (isPlayerAttackingFireOne)
            {
                //Detect burnable objects in range of attack
                Collider2D[] hitBurnables = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, burnableLayers);

                //Destroy burnable objects
                foreach (Collider2D burnable in hitBurnables)
                {
                    Destroy(burnable.gameObject);
                }
            }
        }
    }

    void FireAttackOne()
    {
        //Disable Player's movement during attack animation
        isPlayerAttackingFireOne = true;

        gameObject.GetComponent<PlayerController>().SkillCooldown();

        attackPoint.localPosition = new Vector2(-0.023f, 1.135f);
        attackRange = 0.7f;

        //Play an attack animation
        anim.SetTrigger("FireOne");

        anim.SetBool("isAttackingFireOne", true);

        audioFireOneRef.Play();
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
        {
            return;
        }

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}