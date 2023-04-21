using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using static System.TimeSpan;
using System.Collections;
using System.Collections.Generic;
using static Score;
using System;
using TMPro;

public class random_location : MonoBehaviour
{
    Rigidbody cube_Rigidbody;
    public Text LivesRemainingText;
    public Text GameOverText;
    public float speed = 3.5f;
    public string subjectName;
    public bool isPatient;
    public string eye;

    public DebugManager debugManager;

    [SerializeField]
    float x;

    [SerializeField]
    float y;

    [SerializeField]
    float z;

    public float startRot;

    Vector3 pos;
    float timePrev;
    float numFramesBeforeScoreDecrease;
    int minNumCylinders;
    int maxNumCylinders;
    public GameObject cylinder;
    GameObject[] cylinders;
    public int numTrials;
    public int curTrialNum;
    Vector3 movement;
    public bool gameOver;
    int numFramesBeforeNextTrial;

    public int counter = 1; // remove later, band aid fix

    // time fields
    private float currTime;
    public Text currTimeText;

    // vr ui fields
    public TextMeshPro VRLives;
    public TextMeshPro VRTimer;
    public TextMeshPro GameOver;

    //difficulties
    public int difficulty;

    
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 25;
        numFramesBeforeScoreDecrease = debugManager.isVR ? XRDevice.refreshRate : Application.targetFrameRate;

        numTrials = 10;

        //number of obstacles

        //find the cube in the scene
        cube_Rigidbody = GetComponent<Rigidbody>();
        
        //start the first trial
        curTrialNum = 1;
        StartNewTrial();

    }

    void StartNewTrial() {
        gameOver = false;
        numFramesBeforeNextTrial = 200;
        Score.scoreStart(10);

        Score.displayGameOver(GameOverText, "");
        if (debugManager.isVR)
        {
            Score.displayGameOverVR(GameOver, "");
        }

        // Reset timer
        currTime = 0f;

        //Reset cube location
        x = 24.0f;
        y = 0.25f;
        z = UnityEngine.Random.Range(-2, 4);
        startRot = -85.273f;

        if (!debugManager.isVR || (debugManager.isVR && debugManager.isSim))
        {
            pos = new Vector3(x, y, z);
            transform.position = pos;
        }

        //Time between previous frame and current frame
        timePrev = 0;

        // set difficulty levels

        switch (difficulty)
        {
            case 1:
                minNumCylinders = 40;
                maxNumCylinders = 50;
                break;
            case 2:
                minNumCylinders = 60;
                maxNumCylinders = 70;
                break;
            case 3:
                minNumCylinders = 80;
                maxNumCylinders = 90;
                break;
        }

        //Destroy cylinders 
        if (curTrialNum != 1) {
            foreach (GameObject curCylinder in cylinders) {
                if (curCylinder != null) {
                    Destroy(curCylinder);
                }  
            }
        }
        
        //Generate random cylinders across the board
        cylinders = new GameObject[UnityEngine.Random.Range(minNumCylinders, maxNumCylinders)];
        for (int i = 0; i < cylinders.Length; i++) {
            RandomCylinderGenerator(i);
        }

        counter = 0;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            Destroy(other.gameObject);
        }
    }

    void OnTriggerStay(Collider other) {
        //move the object back
        transform.Translate(-movement * Time.deltaTime * speed);
        numFramesBeforeScoreDecrease--;
        if (numFramesBeforeScoreDecrease <= 0) {
            Score.decreaseScore(GameOverText);

            if (debugManager.isVR)
            {
                Score.decreaseScoreVR(GameOver);
            }

            numFramesBeforeScoreDecrease = debugManager.isVR ? XRDevice.refreshRate : Application.targetFrameRate;
        }
    }

    // Update is called once per frame
    void Update()
    {
        displayScore(LivesRemainingText);

        if (debugManager.isVR)
        {
            displayScoreVR(VRLives);
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        timePrev = Time.deltaTime;
        updateTime();
        if (!gameOver) {
            if (checkGameEnd(transform.position.x, transform.position.z)) {
                Score.displayGameOver(GameOverText, "You reached the target! Congrats!");
                if (debugManager.isVR)
                {
                    Score.displayGameOverVR(GameOver, "You reached the target! Congrats!");
                }

                gameOver = true;
                return;
            }
            else if (Score.getScore() == 0) {
                gameOver = true;
                return;
            }

            if (!debugManager.isVR)
            {
                MoveObject(x, z, timePrev);
            }

        }
        else {
            numFramesBeforeNextTrial--;
            if (numFramesBeforeNextTrial == 0) {
                WriteToJson writeHelper = new WriteToJson(subjectName, isPatient, difficulty, "28-11-2022", currTimeText.text, Int32.Parse(LivesRemainingText.text.Substring(14)), eye);
                writeHelper.SaveToFile(curTrialNum);
                if (curTrialNum < numTrials) {
                    curTrialNum++;
                    StartNewTrial();
                }
                else {
                    Score.displayGameOver(GameOverText, "Congrats, you finished all trials!");
                    if (debugManager.isVR)
                    {
                        Score.displayGameOverVR(GameOver, "Congrats, you finished all trials!");
                    }

                    Debug.Log("Congrats, you finished all trials!");
                    Application.Quit();
                    Debug.Break(); //remove in production
                }
            }
        }
    }

    void MoveObject(float x, float z, float time = 1) 
    {
        movement = new Vector3(x, 0, z);
        movement = Vector3.ClampMagnitude(movement, 1);
        transform.Translate(movement * speed * time);
    }

    void RandomCylinderGenerator(int idx) 
    {
        float x = UnityEngine.Random.Range(-23.0F, 23.0F);
        float y = 1.0f;
        float z = UnityEngine.Random.Range(-23.0f, 23.0f);
        if (!(x <= -18.0 && z >= -2.0 && z <= 5.0) && !(x > 22.0F && z >= -6.0F && z <= 6.0)) {
            cylinders[idx] = Instantiate(cylinder, new Vector3(x, y, z), Quaternion.identity);
        }
    }

    bool checkGameEnd(float x, float z) {
        return x <= -20 && z >= -0.4 && z <= 4.1;
    }

    private void updateTime()
    {
        if (!gameOver)
        {
            currTime += Time.deltaTime;
        }

        TimeSpan time = TimeSpan.FromSeconds(currTime);
        currTimeText.text = time.ToString(@"mm\:ss\:ff");
        if (debugManager.isVR)
        {
            VRTimer.text = time.ToString(@"mm\:ss\:ff");
        }
    }
    
}
