using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using static Score;

public class random_location : MonoBehaviour
{
    Rigidbody cube_Rigidbody;
    public Text LivesRemainingText;
    public Text GameOverText;
    public float speed = 3.5f;

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
    
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 25;
        numFramesBeforeScoreDecrease = Application.targetFrameRate;

        numTrials = 10;

        //number of obstacles
        minNumCylinders = 60;
        maxNumCylinders = 70;

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

        //Reset cube location
        x = 24.0f;
        y = 0.25f;
        z = Random.Range(-2, 4);
        startRot = -85.273f;

        //Time between previous frame and current frame
        timePrev = 0;

        //Destroy cylinders 
        if (curTrialNum != 1) {
            foreach (GameObject curCylinder in cylinders) {
                if (curCylinder != null) {
                    Destroy(curCylinder);
                }  
            }
        }
        
        //Generate random cylinders across the board
        cylinders = new GameObject[Random.Range(minNumCylinders, maxNumCylinders)];
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
            numFramesBeforeScoreDecrease = Application.targetFrameRate;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(Application.targetFrameRate);
        displayScore(LivesRemainingText);
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        timePrev = Time.deltaTime;
        if (!gameOver) {
            if (checkGameEnd(transform.position.x, transform.position.z)) {
                Score.displayGameOver(GameOverText, "You reached the target! Congrats!");
                gameOver = true;
                return;
            }
            else if (Score.getScore() == 0) {
                gameOver = true;
                return;
            }
            //MoveObject(x, z, timePrev); // TODO: Add VR check
        }
        else {
            numFramesBeforeNextTrial--;
            if (numFramesBeforeNextTrial == 0) {
                if (curTrialNum < numTrials) {
                    curTrialNum++;
                    StartNewTrial();
                }
                else {
                    Score.displayGameOver(GameOverText, "Congrats, you finished all trials!");
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
        float x = Random.Range(-23.0F, 23.0F);
        float y = 1.0f;
        float z = Random.Range(-23.0f, 23.0f);
        if (!(x <= -18.0 && z >= -2.0 && z <= 5.0) && !(x > 22.0F && z >= -6.0F && z <= 6.0)) {
            cylinders[idx] = Instantiate(cylinder, new Vector3(x, y, z), Quaternion.identity);
        }
    }

    bool checkGameEnd(float x, float z) {
        return x <= -20 && z >= -0.4 && z <= 4.1;
    }
    
}
