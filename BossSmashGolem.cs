using System.Collections;
using UnityEngine;

public class BossSmashGolem : MonoBehaviour
{
    private Animator anim;

    private GameObject audioSource;
    private AudioSource audioSmashGolemWallRef;
    private AudioSource audioSmashGolemGroundSmallerRef;
    private AudioSource audioSmashGolemGroundBigRef;

    public bool startActivation = false;

    private int bossHpCurrent;
    private bool isNextAttackOk = false;
    private bool isAttackWallActive = false;

    public Transform attackPointSmall;
    public Transform attackPointMedium;
    public Transform attackPointBig;
    public GameObject projectileGroundSmall;
    public GameObject projectileGroundMedium;
    public GameObject projectileGroundBig;

    private int attackCounter = 0;
    private int randomNumber = 0;
    private bool isAttackBig = false;

    public Transform attackPointWall;
    public GameObject projectileWall;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        audioSource = GameObject.Find("SoundController");
        audioSmashGolemWallRef = audioSource.GetComponent<SoundController>().audioSmashGolemWall;
        audioSmashGolemGroundSmallerRef = audioSource.GetComponent<SoundController>().audioSmashGolemGroundSmaller;
        audioSmashGolemGroundBigRef = audioSource.GetComponent<SoundController>().audioSmashGolemGroundBig;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (startActivation)
        {
            startActivation = false;
            StartCoroutine(WaitAfterSpawn());
        }

        if (bossHpCurrent > gameObject.GetComponent<EnemyController>().currentHp)
        {
            bossHpCurrent = gameObject.GetComponent<EnemyController>().currentHp;
            isAttackWallActive = true;
            isNextAttackOk = false;
            anim.SetBool("AttackWall", true);
        }
        else if (!isAttackWallActive && isNextAttackOk)
        {
            isNextAttackOk = false;
            anim.SetBool("AttackGround", true);
        }

    }

    //Used from animation
    void AttackGround()
    {
        attackCounter++;

        if (attackCounter == 5)
        {
            attackCounter = 0;
            isAttackBig = true;
            audioSmashGolemGroundBigRef.Play();
            Instantiate(projectileGroundBig, attackPointBig.position, attackPointBig.rotation);
        }
        else
        {
            audioSmashGolemGroundSmallerRef.Play();

            randomNumber = Random.Range(0, 2);

            if (randomNumber == 0)
            {
                Instantiate(projectileGroundSmall, attackPointSmall.position, attackPointSmall.rotation);
            }
            else if (randomNumber == 1)
            {
                Instantiate(projectileGroundMedium, attackPointMedium.position, attackPointMedium.rotation);
            }
        }
    }

    //Used from animation
    void AttackGroundEnd()
    {
        anim.SetBool("AttackGround", false);

        if (isAttackBig)
        {
            isAttackBig = false;
            StartCoroutine(WaitAfterAttackGround(4f));
        }
        else if (randomNumber == 0)
        {
            StartCoroutine(WaitAfterAttackGround(0.5f));
        }
        else if (randomNumber == 1)
        {
            StartCoroutine(WaitAfterAttackGround(1f));
        }
    }

    //Used from animation
    public void AttackWallAudioStart()
    {
        audioSmashGolemWallRef.Play();
    }

    //Used from animation
    void AttackWall()
    {
        attackCounter = 0;
        Instantiate(projectileWall, attackPointWall.position, attackPointWall.rotation);
    }

    //Used from animation
    void AttackWallEnd()
    {
        anim.SetBool("AttackWall", false);
        StartCoroutine(WaitAfterAttackWall(2f));
    }

    IEnumerator WaitAfterSpawn()
    {
        yield return new WaitForSeconds(1f);
        bossHpCurrent = gameObject.GetComponent<EnemyController>().currentHp + 1;
    }

    IEnumerator WaitAfterAttackGround(float time)
    {
        yield return new WaitForSeconds(time);
        isNextAttackOk = true;
    }

    IEnumerator WaitAfterAttackWall(float time)
    {
        yield return new WaitForSeconds(time);
        isAttackWallActive = false;
        isNextAttackOk = true;
    }
}
