using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private bool gameHasEnded;

    private Animator transition;

    private GameObject canvas;
    private GameObject panelPause;
    private GameObject textPause;
    private GameObject uiHpUpgrade;
    private GameObject uiHealAvailableUpgrade;
    private GameObject uiHealPerHealUpgrade;
    private GameObject hpUpgradeNumber;
    private GameObject healAvailableUpgradeNumber;
    private GameObject healPerHealUpgradeNumber;

    private GameObject audioSource;
    private AudioSource audioTrackOneRef;
    private AudioSource audioTrackTwoRef;
    private AudioSource audioTrackThreeRef;
    private AudioSource audioTrackFountainSpaceRef;
    private AudioSource audioTrackCaveRef;
    private AudioSource audioTrackPrisonRef;

    private GameObject player;
    public GameObject bossDoor;
    public GameObject fireOrbOne;
    public GameObject orb;
    public bool bossTeleportActive = false;
    public GameObject bridgeTrigger;

    public static bool gameIsPaused = false;

    public float transitionDuration = 1f;

    public float restartDelay = 1f;

    public bool debugPlayerAttackUp = false;

    public int hpUpgradesCollectedNumber;
    public int healAvailableUpgradesCollectedNumber;
    public int healPerHealUpgradesCollectedNumber;

    void Start()
    {
        Application.targetFrameRate = 60;

        transition = GameObject.Find("Canvas").GetComponent<Animator>();

        canvas = GameObject.Find("Canvas");
        panelPause = canvas.transform.Find("PanelPause").gameObject;
        textPause = canvas.transform.Find("TextPause").gameObject;
        uiHpUpgrade = canvas.transform.Find("UIHpUpgrade").gameObject;
        uiHealAvailableUpgrade = canvas.transform.Find("UIHAUpgrade").gameObject;
        uiHealPerHealUpgrade = canvas.transform.Find("UIHHUpgrade").gameObject;
        hpUpgradeNumber = canvas.transform.Find("TextHpUpgradeNumber").gameObject;
        healAvailableUpgradeNumber = canvas.transform.Find("TextHAUpgradeNumber").gameObject;
        healPerHealUpgradeNumber = canvas.transform.Find("TextHHUpgradeNumber").gameObject;

        hpUpgradesCollectedNumber = PlayerPrefs.GetInt("HpUpgradesCollected");
        UpdateHpUpgradeNumber();

        healAvailableUpgradesCollectedNumber = PlayerPrefs.GetInt("HealAvailableUpgradesCollected");
        UpdateHealAvailableUpgradeNumber();

        healPerHealUpgradesCollectedNumber = PlayerPrefs.GetInt("healPerHealUpgradesCollected");
        UpdateHealPerHealUpgradeNumber();

        audioSource = GameObject.Find("SoundController");
        audioTrackOneRef = audioSource.GetComponent<SoundController>().audioTrackOne;
        audioTrackTwoRef = audioSource.GetComponent<SoundController>().audioTrackTwo;
        audioTrackThreeRef = audioSource.GetComponent<SoundController>().audioTrackThree;
        audioTrackFountainSpaceRef = audioSource.GetComponent<SoundController>().audioTrackFountainSpace;
        audioTrackCaveRef = audioSource.GetComponent<SoundController>().audioTrackCave;
        audioTrackPrisonRef = audioSource.GetComponent<SoundController>().audioTrackPrison;

        player = GameObject.Find("Player");

        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            audioTrackThreeRef.Play();
        }
        else if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            audioTrackOneRef.Play();
        }
        else if (SceneManager.GetActiveScene().buildIndex == 4)
        {
            audioTrackFountainSpaceRef.Play();
        }
        else if (SceneManager.GetActiveScene().buildIndex == 5)
        {
            audioTrackCaveRef.Play();
        }
        else if (SceneManager.GetActiveScene().buildIndex == 7)
        {
            audioTrackPrisonRef.Play();
        }

        //Check if player can use FireOne skill when restarting
        if (PlayerPrefs.GetInt("OrbFireOneTaken") == 1)
        {
            player.GetComponent<PlayerAttackFireOne>().isFireOneOrbTaken = true;
        }

        //Check if player can use AirOne skill when restarting
        if (PlayerPrefs.GetInt("OrbAirOneTaken") == 1)
        {
            player.GetComponent<PlayerAttackAirOne>().isAirOneOrbTaken = true;
        }

        if (PlayerPrefs.GetInt("BossTeleportActive") == 1)
        {
            bossTeleportActive = true;
        }

        if (PlayerPrefs.GetInt("StartFromFountain1") == 1)
        {
            StartSceneFromFountain1();
            PlayerPrefs.SetInt("StartFromFountain1", 0);
        }

        if (PlayerPrefs.GetInt("StartFromFountain2") == 1)
        {
            StartSceneFromFountain2();
            PlayerPrefs.SetInt("StartFromFountain2", 0);
        }

        if (PlayerPrefs.GetInt("StartFromFountainSpace1") == 1)
        {
            StartSceneFromFountainSpace1();
            PlayerPrefs.SetInt("StartFromFountainSpace1", 0);
        }

        if (PlayerPrefs.GetInt("StartFromFountainSpace2") == 1)
        {
            StartSceneFromFountainSpace2();
            PlayerPrefs.SetInt("StartFromFountainSpace2", 0);
        }

        if (PlayerPrefs.GetInt("StartFromBossFireBear") == 1)
        {
            Teleport();
        }

        if (PlayerPrefs.GetInt("StartFromBossJumpLizard") == 1)
        {
            Teleport();
            PlayerPrefs.SetInt("StartFromBossJumpLizard", 0);
        }

        if (PlayerPrefs.GetInt("StartFromBossSmashGolem") == 1)
        {
            Teleport();
            PlayerPrefs.SetInt("StartFromBossSmashGolem", 0);
        }

        if (PlayerPrefs.GetInt("StartFromCave1") == 1)
        {
            StartSceneFromCave();
            PlayerPrefs.SetInt("StartFromCave1", 0);
        }

        if (PlayerPrefs.GetInt("StartFromCave2") == 1)
        {
            StartSceneFromCave2();
            PlayerPrefs.SetInt("StartFromCave2", 0);
        }

        if (PlayerPrefs.GetInt("StartFromForest1") == 1)
        {
            StartSceneFromForest();
            PlayerPrefs.SetInt("StartFromForest1", 0);
        }

        if (PlayerPrefs.GetInt("StartFromPrison1") == 1)
        {
            StartSceneFromPrison();
            PlayerPrefs.SetInt("StartFromPrison1", 0);
        }


    }

    void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            if (!gameIsPaused)
            {
                gameIsPaused = true;
                Time.timeScale = 0;
                panelPause.SetActive(true);
                textPause.SetActive(true);
                uiHpUpgrade.SetActive(true);
                uiHealAvailableUpgrade.SetActive(true);
                uiHealPerHealUpgrade.SetActive(true);
                hpUpgradeNumber.SetActive(true);
                healAvailableUpgradeNumber.SetActive(true);
                healPerHealUpgradeNumber.SetActive(true);
            }
            else
            {
                gameIsPaused = false;
                Time.timeScale = 1;
                panelPause.SetActive(false);
                textPause.SetActive(false);
                uiHpUpgrade.SetActive(false);
                uiHealAvailableUpgrade.SetActive(false);
                uiHealPerHealUpgrade.SetActive(false);
                hpUpgradeNumber.SetActive(false);
                healAvailableUpgradeNumber.SetActive(false);
                healPerHealUpgradeNumber.SetActive(false);
            }
        }

        if (Input.GetKeyDown("t"))
        {
            EndGame();
        }

        if (Input.GetKey("b") && Input.GetKey("z"))
        {
            player.GetComponent<PlayerController>().godMode = true;
        }

        if (Input.GetKey("z") && Input.GetKey("n"))
        {
            debugPlayerAttackUp = true;
        }

        if (Input.GetKey("z") && Input.GetKey("v"))
        {
            GameObject.Find("Canvas").GetComponent<PointsKills>().points += 10;
            GameObject.Find("Canvas").GetComponent<PointsKills>().UpdatePoints();
        }

        if (Input.GetKey("z") && Input.GetKey("c"))
        {
            StartCoroutine(LoadLevel(0));
        }
    }

    public void UpdateHpUpgradeNumber()
    {
        if (hpUpgradesCollectedNumber == 0)
        {
            hpUpgradeNumber.GetComponent<Text>().text = "0/4";
        }
        else if (hpUpgradesCollectedNumber == 1)
        {
            hpUpgradeNumber.GetComponent<Text>().text = "1/4";
        }
        else if (hpUpgradesCollectedNumber == 2)
        {
            hpUpgradeNumber.GetComponent<Text>().text = "2/4";
        }
        else if (hpUpgradesCollectedNumber == 3)
        {
            hpUpgradeNumber.GetComponent<Text>().text = "3/4";
        }
    }

    public void UpdateHealAvailableUpgradeNumber()
    {
        if (healAvailableUpgradesCollectedNumber == 0)
        {
            healAvailableUpgradeNumber.GetComponent<Text>().text = "0/4";
        }
        else if (healAvailableUpgradesCollectedNumber == 1)
        {
            healAvailableUpgradeNumber.GetComponent<Text>().text = "1/4";
        }
        else if (healAvailableUpgradesCollectedNumber == 2)
        {
            healAvailableUpgradeNumber.GetComponent<Text>().text = "2/4";
        }
        else if (healAvailableUpgradesCollectedNumber == 3)
        {
            healAvailableUpgradeNumber.GetComponent<Text>().text = "3/4";
        }
    }

    public void UpdateHealPerHealUpgradeNumber()
    {
        if (healPerHealUpgradesCollectedNumber == 0)
        {
            healPerHealUpgradeNumber.GetComponent<Text>().text = "0/4";
        }
        else if (healPerHealUpgradesCollectedNumber == 1)
        {
            healPerHealUpgradeNumber.GetComponent<Text>().text = "1/4";
        }
        else if (healPerHealUpgradesCollectedNumber == 2)
        {
            healPerHealUpgradeNumber.GetComponent<Text>().text = "2/4";
        }
        else if (healPerHealUpgradesCollectedNumber == 3)
        {
            healPerHealUpgradeNumber.GetComponent<Text>().text = "3/4";
        }
    }

    public void BossFightStart()
    {
        audioTrackOneRef.Stop();
        audioTrackCaveRef.Stop();
        audioTrackPrisonRef.Stop();
        audioTrackTwoRef.Play();

        bossTeleportActive = true;
        PlayerPrefs.SetInt("BossTeleportActive", 1);
    }

    public void BossFightEndForest()
    {
        audioTrackTwoRef.Stop();
        audioTrackOneRef.Play();

        bossTeleportActive = false;
        PlayerPrefs.SetInt("BossTeleportActive", 0);
    }

    public void BossFightEndCave()
    {
        audioTrackTwoRef.Stop();
        audioTrackCaveRef.Play();

        bossTeleportActive = false;
        PlayerPrefs.SetInt("BossTeleportActive", 0);
    }

    public void BossFightEndPrison()
    {
        audioTrackTwoRef.Stop();
        audioTrackPrisonRef.Play();

        bossTeleportActive = false;
        PlayerPrefs.SetInt("BossTeleportActive", 0);
    }

    public void EndGame()
    {
        if (!gameHasEnded)
        {
            gameHasEnded = true;

            Invoke("RestartLevel", restartDelay);
        }
    }

    public void Teleport()
    {
        if (PlayerPrefs.GetInt("StartFromBossFireBear") == 1)
        {
            player.transform.position = new Vector2(-282.25f, 20.57f);
            PlayerPrefs.SetInt("StartFromBossFireBear", 0);
        }
        else if (PlayerPrefs.GetInt("StartFromBossJumpLizard") == 1)
        {
            player.transform.position = new Vector2(-281f, -326.4f);
            PlayerPrefs.SetInt("StartFromBossJumpLizard", 0);
        }
        else if (PlayerPrefs.GetInt("StartFromBossSmashGolem") == 1)
        {
            player.transform.position = new Vector2(-600.54f, -210.47f);
            PlayerPrefs.SetInt("StartFromBossSmashGolem", 0);
        }
    }

    void RestartLevel()
    {
        StartCoroutine(LoadLevel(1));
    }

    //When going from Moon Temple tp Tpwn
    public void GoToLevel()
    {
        PlayerPrefs.SetInt("PointsKills", 0);

        PlayerPrefs.SetInt("isBridgeTriggerActivated", 0);

        StartCoroutine(LoadLevelFromMoonTemple(2));
    }
    public void EndDemo()
    {
        StartCoroutine(LoadLevel(3));
    }

    public void GoToDemoSecret()
    {
        StartCoroutine(LoadLevel(6));
    }

    public void GoToFountainSpace(int id)
    {
        if (id == 1)
        {
            PlayerPrefs.SetInt("StartFromFountainSpace1", 1);
            PlayerPrefs.SetInt("ActivatedFountain1", 1);
            StartCoroutine(LoadLevel(4));
        }
        else if (id == 2)
        {
            PlayerPrefs.SetInt("StartFromFountainSpace2", 1);
            PlayerPrefs.SetInt("ActivatedFountain2", 1);
            StartCoroutine(LoadLevel(4));
        }
    }

    public void GoBackFromFountainSpace(int id)
    {
        if (id == 1)
        {
            if (PlayerPrefs.GetInt("ActivatedFountain1") == 1)
            {
                PlayerPrefs.SetInt("StartFromFountain1", 1);
                StartCoroutine(LoadLevel(2));
            }
        }
        else if (id == 2)
        {
            if (PlayerPrefs.GetInt("ActivatedFountain2") == 1)
            {
                PlayerPrefs.SetInt("StartFromFountain2", 1);
                StartCoroutine(LoadLevel(5));
            }
        }
    }

    public void StartSceneFromFountain1()
    {
        player.transform.position = new Vector2(-81.06f, -7.48f);
    }

    public void StartSceneFromFountain2()
    {
        player.transform.position = new Vector2(-320.58f, -237.48f);
    }

    public void StartSceneFromFountainSpace1()
    {
        player.transform.position = new Vector2(-3.54f, 7.52f);
    }

    public void StartSceneFromFountainSpace2()
    {
        player.transform.position = new Vector2(-10.656f, -3.486f);
    }


    public void TeleportToBossFireBear()
    {
        StartCoroutine(LoadLevel(2));
        PlayerPrefs.SetInt("StartFromBossFireBear", 1);
    }

    public void TeleportToBossJumpLizard()
    {
        StartCoroutine(LoadLevel(5));
        PlayerPrefs.SetInt("StartFromBossJumpLizard", 1);
    }

    public void TeleportToBossSmashGolem()
    {
        StartCoroutine(LoadLevel(7));
        PlayerPrefs.SetInt("StartFromBossSmashGolem", 1);
    }

    public void StartSceneFromCave()
    {
        player.transform.position = new Vector2(-418.48f, -120.45f);
    }

    public void StartSceneFromCave2()
    {
        player.transform.position = new Vector2(-577.6902f, -280.4852f);
    }

    public void StartSceneFromForest()
    {
        player.transform.position = new Vector2(-422.93f, -120.45f);
        player.GetComponent<PlayerMovement>().FlipWhenSceneStart();
    }

    public void StartSceneFromPrison()
    {
        player.transform.position = new Vector2(-583.63f, -280.4853f);
        player.GetComponent<PlayerMovement>().FlipWhenSceneStart();
    }

    public void LoadAreaFromForestToCave()
    {
        PlayerPrefs.SetInt("StartFromCave1", 1);
        StartCoroutine(LoadLevel(5));
    }

    public void LoadAreaFromCaveToForest()
    {
        PlayerPrefs.SetInt("StartFromForest1", 1);
        StartCoroutine(LoadLevel(2));
    }

    public void LoadAreaFromCaveToPrison()
    {
        PlayerPrefs.SetInt("StartFromPrison1", 1);
        StartCoroutine(LoadLevel(7));
    }

    public void LoadAreaFromPrisonToCave()
    {
        PlayerPrefs.SetInt("StartFromCave2", 1);
        StartCoroutine(LoadLevel(5));
    }

    IEnumerator LoadLevel(int index)
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionDuration);

        SceneManager.LoadScene(index);
    }

    IEnumerator LoadLevelFromMoonTemple(int index)
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionDuration);

        for (int i = 1; i < 1000; i++)
        {
            PlayerPrefs.SetInt("isEnemyDeadWithID: " + i, 0);
        }

        SceneManager.LoadScene(index);
    }
}