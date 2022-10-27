using UnityEngine;

public class EnemyController : MonoBehaviour
{
    Rigidbody2D rb;
    private Animator anim;

    [HideInInspector] // Hides var below
    public Vector3 startPosition;

    Color origColor;
    private GameObject audioSource;
    public int maxHp = 2;
    public int currentHp;

    public bool enemyReset = true;

    public bool bossEnemy = false;

    public bool frozen;

    public bool freezeNow;
    public float frozenTime = 4f;
    float frozenEndTime;

    private Material matWhite;
    private Material matDefault;

    private SpriteRenderer renderer2;

    private Object explosionRef;

    private AudioSource audioHurtRef;

    public float knockbackSpeed = 3.5f;
    public float knockbackDuration = 0.5f;
    private float knockbackTime;
    private bool knockFromRight;

    public float invincibleDuration = 0.33f;
    private float invincibleTime;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        anim = GetComponent<Animator>();

        startPosition = transform.position;

        origColor = gameObject.GetComponent<Renderer>().material.color;
        currentHp = maxHp;

        renderer2 = GetComponent<SpriteRenderer>();

        matWhite = Resources.Load("WhiteFlash", typeof(Material)) as Material;
        matDefault = renderer2.material;

        explosionRef = Resources.Load("EnemyExplosion");

        audioSource = GameObject.Find("SoundController");
        audioHurtRef = audioSource.GetComponent<SoundController>().audioHurt;



        renderer2 = GetComponent<SpriteRenderer>();

        matWhite = Resources.Load("WhiteFlash", typeof(Material)) as Material;
        matDefault = renderer2.material;

        explosionRef = Resources.Load("EnemyExplosion");
    }

    void FixedUpdate()
    {
        if (gameObject.GetComponentInParent<EnemyActivation>().dead == false && enemyReset)
        {
            currentHp = maxHp;
            transform.position = startPosition;

            //Reset animations to entry state
            anim.Rebind();
            anim.Update(0f);

            enemyReset = false;
        }

        if (frozen && freezeNow)
        {
            freezeNow = false;

            //Change enemy's layer to Ground
            gameObject.layer = 9;

            //Change enemy's color to blueish
            gameObject.GetComponent<Renderer>().material.color = new Color(0f, 0f, 50f);

            gameObject.GetComponent<Animator>().enabled = false;

            if (gameObject.GetComponent<EnemyFireBreath>() != null)
            {
                gameObject.GetComponent<EnemyFireBreath>().enabled = false;
            }

            if (gameObject.GetComponent<EnemyStatue>() != null)
            {
                gameObject.GetComponent<EnemyStatue>().enabled = false;
            }

            if (gameObject.GetComponent<EnemyHoming>() != null)
            {
                gameObject.GetComponent<EnemyHoming>().enabled = false;
            }

            frozenEndTime = Time.time + frozenTime;
        }

        if (Time.time >= frozenEndTime)
        {
            //Change enemy's layer back to Enemy
            gameObject.layer = 8;

            //Change enemy's color back to normal
            gameObject.GetComponent<Renderer>().material.color = origColor;

            gameObject.GetComponent<Animator>().enabled = true;

            if (gameObject.GetComponent<EnemyFireBreath>() != null)
            {
                gameObject.GetComponent<EnemyFireBreath>().enabled = true;
            }

            if (gameObject.GetComponent<EnemyStatue>() != null)
            {
                gameObject.GetComponent<EnemyStatue>().enabled = true;
            }

            frozen = false;
        }

        if (knockbackTime > 0)
        {
            rb.velocity = new Vector2(knockFromRight ? -knockbackSpeed : knockbackSpeed, rb.velocity.y);

            knockbackTime -= Time.deltaTime;
        }

        if (invincibleTime > 0)
        {
            invincibleTime -= Time.deltaTime;
        }
    }

    public void TakeDamage(int damage)
    {
        if (invincibleTime <= 0)
        {
            if (audioHurtRef != null)
            {
                audioHurtRef.Play();
            }

            currentHp -= damage;

            if (currentHp <= 0)
            {
                Die();
            }
            else
            {
                renderer2.material = matWhite;
                Invoke("ResetMaterial", 0.08f);
            }

            invincibleTime = invincibleDuration;
        }
    }

    public void TakeShurikenDamage(int damage)
    {
        currentHp -= damage;

        if (currentHp <= 0)
        {
            Die();
        }
    }

    void Knockback(bool fromRight)
    {
        knockFromRight = fromRight;

        if (knockbackTime <= 0)
        {
            knockbackTime = knockbackDuration;
            rb.velocity = new Vector2(rb.velocity.x, 0);
        }

    }

    void Die()
    {
        GameObject.Find("Canvas").GetComponent<PointsKills>().points += maxHp;
        GameObject.Find("Canvas").GetComponent<PointsKills>().UpdatePoints();

        if (gameObject.GetComponentInParent<EnemyActivation>() != null)
        {
            gameObject.GetComponentInParent<EnemyActivation>().dead = true;
        }

        GameObject explosion = (GameObject)Instantiate(explosionRef);

        if (bossEnemy)
        {
            explosion.transform.position = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);
            explosion.transform.localScale = new Vector3(transform.localScale.x + 2f, transform.localScale.y + 2f, transform.localScale.z + 2f);
        }
        else
        {
            explosion.transform.position = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);
        }

        PlayerPrefs.SetInt("isEnemyDeadWithID: " + gameObject.GetComponentInParent<EnemyActivation>().enemyId, 1);

        gameObject.SetActive(false);
    }

    void ResetMaterial()
    {
        renderer2.material = matDefault;
    }

}
