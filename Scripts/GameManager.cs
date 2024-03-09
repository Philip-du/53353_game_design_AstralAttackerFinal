using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum GameState { Menu, PreRound, Playing, PostRound, GameOver};

public class GameManager : MonoBehaviour
{
    public static GameManager S; // define the singleton

    public TextMeshProUGUI messageOverlayObject;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI livesText;
    public GameState currentState;

    public int score = 0;
    private int livesRemaining;
    private int MAX_START_LIVES = 3;

    // public GameObject motherShipPrefab;
    private GameObject currentMotherShip;

    private string currentLevel;


    private void Awake()
    {
        if (GameManager.S)
        {
            // the game manager already exists, destroy myself
            Destroy(this.gameObject);
        } else
        {
            S = this; 
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        StartANewGame();
        DontDestroyOnLoad(this);
    }


    // Update is called once per frame
    void Update()
    {



    }


    private void StartANewGame()
    {
        //reset our score
        score = 0;

        // reset our lives
        livesRemaining = MAX_START_LIVES;

        // start a new round
        
    }

    public void LevelStarted()
    {
        // level manager tells me what level we are on
        string thisLevel = LevelManager.S.levelName;

        if (currentLevel != thisLevel)
        {

            // update the current level value
            currentLevel = thisLevel;
            // this is a new level, run first time level co-routine
            StartCoroutine(ShowLevelName());
        } else
        {
            ResetRound();
        }



    }

    private IEnumerator ShowLevelName()
    {
        messageOverlayObject.text = currentLevel;
        yield return new WaitForSeconds(3.0f);

        ResetRound();
    }

    private void ResetRound()
    {
        // does current mothership exist?
        if (currentMotherShip)
        {
            // if so, get rid of it
            Destroy(currentMotherShip);
        }

        // get the mothership object that level manager is holding for me.
        currentMotherShip = LevelManager.S.levelMotherShipObject;

        // run the Get Ready coroutine
        StartCoroutine(GetReady());

    }

    private IEnumerator GetReady()
    {
        messageOverlayObject.enabled = true;
        messageOverlayObject.text = "Get Ready!!!";

        yield return new WaitForSeconds(3.0f);


        messageOverlayObject.enabled = false;

        StartRound();
    }

    private void StartRound()
    {
        // start the music
        SoundManager.S.StartTheMusic();

        // start the attack
        currentMotherShip.GetComponent<MotherShipScript>().StartTheAttack();

        // set our current state to playing
        currentState = GameState.Playing;

    }

    public void PlayerObjectDestroyed()
    {
        currentState = GameState.PostRound;

        // this is a tragedy!  stop the enemies
        currentMotherShip.GetComponent<MotherShipScript>().StopTheAttack();


        // stop all sounds and play the player explosion sound
        SoundManager.S.PlayerExplosionSequence();

        // process my lives remaining
        livesRemaining--;
        UpdateUI();


        if (livesRemaining > 0)
        {
            StartCoroutine(OopsState());
        } 
        // else the game is over and you lost

    }

    private IEnumerator OopsState()
    {
        messageOverlayObject.text = "You Died!!!";
        messageOverlayObject.enabled = true;

        yield return new WaitForSeconds(3.0f);

        LevelManager.S.ReloadLevel();
    }

    private IEnumerator WinState()
    {
        messageOverlayObject.text = "Round Complete";
        messageOverlayObject.enabled = true;

        yield return new WaitForSeconds(3.0f);

        /*
        messageOverlayObject.text = "Replay?";
        messageOverlayObject.enabled = true;
        */

        LevelManager.S.GoToNextRound();
    }

    public void AllEnemiesDestroyed()
    {
        Debug.Log("no more enemies");
        currentMotherShip.GetComponent<MotherShipScript>().StopTheAttack();
        StartCoroutine(WinState());
    }

    public void AddScore(int thisValue)
    {
        score += thisValue;
        UpdateUI();
    }

    private void UpdateUI()
    {
        scoreText.text = "Score: " + score;
        livesText.text = "Lives: " + livesRemaining;
    }


}
