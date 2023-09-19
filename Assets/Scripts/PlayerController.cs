using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;


public class PlayerController : MonoBehaviour
{
    public float speed = 1.0f;
    public TMP_Text coinCountTxt;
    public TMP_Text timerTxt;
    public GameObject timeUpTxt;
    public GameObject instructionTextObject;
    public Collider spawnArea;
    public TMP_Text currentScoreTxt;
    public TMP_Text highScoreTxt;

    private Rigidbody rb;
    private AudioSource coinAudio;
    private int coinCount = 0;
    private bool isGameRunning = false;
    private float timer = 60f;
    private bool isTimeUp = false;

    private int highScore;

    private float movementX;
    private float movementY;

    //Awake is called when the instance of the script is loaded before the Start method
    private void Awake()
    {
        Time.timeScale = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        coinAudio = GetComponent<AudioSource>();
    }

    private void SetHighScore()
    {
        if (PlayerPrefs.HasKey("HIGHSCORE") == false)
        {
            highScore = coinCount;
            PlayerPrefs.SetInt("HIGHSCORE", highScore);
        }
        else
        {
            highScore = PlayerPrefs.GetInt("HIGHSCORE");
            if (coinCount > highScore)
            {
                highScore = coinCount;
                PlayerPrefs.SetInt("HIGHSCORE", highScore);
            }
        }

        currentScoreTxt.text = "Score: " + coinCount.ToString();
        highScoreTxt.text = "High Score: " + highScore.ToString();
    }

    private void StartGame()
    {
        if (Input.GetKey(KeyCode.Space) && isGameRunning == false)
        {
            instructionTextObject.SetActive(false);
            isGameRunning = true;
            Time.timeScale = 1;

        }
    }

    private void StartTimer()
    {
        timer = timer - Time.deltaTime;
    }

    // Update is called once per frame
    private void Update()
    {

        if (isGameRunning == true)
        {
            Move();
            StartTimer();
            UpdateTimerText();
            ResetGame();
        }
        else
        {
            StartGame();
        }
    }

    private void Move()
    {
        movementX = Input.GetAxis("Horizontal");
        movementY = Input.GetAxis("Vertical");
    }

    private void FixedUpdate()
    {
        PlayerMovement();
    }

    private void PlayerMovement()
    {
        Vector3 movement = new Vector3(movementX, 0.0f, movementY);

        rb.AddForce(movement.normalized * speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin"))
        {
            coinAudio.Play();
            //other.gameObject.SetActive(false);
            
            SetObjectToRandomPosition(other.gameObject.transform.parent);
            UpdateScore(1);

            SetCountText();
        }
    }

    private void UpdateScore(int value)
    {
        coinCount = coinCount + value;
    }

    private void SetCountText()
    {
        coinCountTxt.text = "COIN COUNT: " + coinCount.ToString();

        //if (coinCount >= 3)
        //{
        //    // Set the text value of your 'winText'
        //    timeUpTxt.SetActive(true);
        //}
    }

    private void UpdateTimerText()
    {
        timerTxt.text = "TIMER: " + timer.ToString("00");
        if (timer <= 0.0f)
        {
            timer = 0.0f;
            timerTxt.text = "TIMER: " + timer.ToString("00");

            SetHighScore();
            timeUpTxt.SetActive(true);
            Time.timeScale = 0;
            isTimeUp = true;

        }
    }

    private void ResetGame()
    {
        if (Input.GetKeyUp(KeyCode.R) && isTimeUp == true)
        {
            SceneManager.LoadScene("GameScene");
        }
        
    }

    private void SetObjectToRandomPosition(Transform objectTransform)
    {
        float randomPositionX = Random.Range(spawnArea.bounds.min.x, spawnArea.bounds.max.x);
        float randomPositionZ = Random.Range(spawnArea.bounds.min.z, spawnArea.bounds.max.z);

        objectTransform.position = new Vector3(randomPositionX, gameObject.transform.position.y, randomPositionZ);

        objectTransform.gameObject.SetActive(true);
    }
}
