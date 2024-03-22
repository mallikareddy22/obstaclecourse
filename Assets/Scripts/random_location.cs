using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using System.Collections;
using System.Collections.Generic;
using static Score;
using System;
using TMPro;
using ViveSR.anipal.Eye;

public class random_location : MonoBehaviour
{
    Rigidbody cube_Rigidbody;
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
    int minNumCylinders;
    int maxNumCylinders;
    public GameObject cylinder;
    GameObject[] cylinders;
    public int numTrials;
    public int curTrialNum;
    public bool gameOver;
    int numFramesBeforeNextTrial;
    private bool loseLives;
    private Coroutine loseEverySecond;

    public int counter = 1; // remove later, band aid fix

    // time fields
    private float currTime;
    public TextMeshProUGUI currTimeText;
    public TextMeshProUGUI LivesRemainingText;
    public TextMeshProUGUI GameOverText;

    // vr ui fields
    public TextMeshPro VRLives;
    public TextMeshPro VRTimer;
    public TextMeshPro GameOver;

    //difficulties
    public int difficulty;

    // UI elements
    public GameObject startPanel;
    public bool showUI;
    bool prem;

    // Data tracking fields
    [SerializeField]
    List<Results> results;
    List<Vector3> positions;
    List<HeadTracking> headInfo;
    public bool showPath;

    SRanipal_GazeRaySampleDataCol eyeDataCollector;
    Vector3 oldCenter;


    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 25;
        numTrials = 2;
        if (debugManager.isSim)
        {
            oldCenter = GetComponent<CharacterController>().center;
        }

        //number of obstacles

        //find the cube in the scene
        cube_Rigidbody = GetComponent<Rigidbody>();
        startPanel = GameObject.Find("StartPanel");
        eyeDataCollector = gameObject.GetComponent<SRanipal_GazeRaySampleDataCol>();
        showUI = true;
        prem = false;
        results = new List<Results>();
        positions = new List<Vector3>();
        headInfo = new List<HeadTracking>();
        loseLives = false;

        var lineRenderer = gameObject.GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
    }

    /**
     * Assigns pertinent patient information.
     * Errors if unsuccessful.
     * TODO: More validation.
     */
    public void SetupInfo()
    {
        
        subjectName = startPanel.transform.GetChild(0).GetComponent<InputField>().text;
        if (subjectName.Length == 0)
        {
            Debug.LogError("Name is empty!");
            return;
        }

        numTrials = int.Parse(startPanel.transform.GetChild(1).GetComponent<InputField>().text);
        List<Dropdown.OptionData> eyeOptions = startPanel.transform.GetChild(2).GetComponent<Dropdown>().options;
        eye = eyeOptions[startPanel.transform.GetChild(2).GetComponent<Dropdown>().value].text;

        difficulty = startPanel.transform.GetChild(3).GetComponent<Dropdown>().value + 1;

        isPatient = startPanel.transform.GetChild(4).GetComponent<Toggle>().isOn;

        //start the first trial
        curTrialNum = 1;
        showUI = false;
        startPanel.SetActive(false);
        StartNewTrial();
    }

    void StartNewTrial() {
        showUI = false;
        loseLives = false;
        positions = new List<Vector3>();
        headInfo = new List<HeadTracking>();
        eyeDataCollector.startCollectingData();
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

        // set difficulty levels

        switch (difficulty)
        {
            case 1:
                minNumCylinders = 40;
                maxNumCylinders = 45;
                break;
            case 2:
                minNumCylinders = 65;
                maxNumCylinders = 70;
                break;
            case 3:
                minNumCylinders = 85;
                maxNumCylinders = 90;
                break;
        }
        cylinder.SetActive(true);
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
        cylinder.SetActive(false);
        counter = 0;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            Destroy(other.gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.name == "Cylinder 1(Clone)")
        {
            loseLives = true;
            if (loseEverySecond == null)
            {
                loseEverySecond = StartCoroutine(LoseLivesPerSecond());
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.collider.name == "Cylinder 1(Clone)")
        {
            loseLives = false;
        }
    }

    void OnCollisionStay(Collision collision) {
        if (collision.collider.name == "Cylinder 1(Clone)")
        {
            loseLives = true;
        }
    }

    private IEnumerator LoseLivesPerSecond()
    {
        while (loseLives)
        {
            Score.decreaseScore(GameOverText);
            if (debugManager.isVR)
            {
                Score.decreaseScoreVR(GameOver);
            }
            yield return new WaitForSeconds(1f);
        }
        loseEverySecond = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (!startPanel.activeSelf)
        {
            RunExpt();
        }
    }

    public void RunPrem()
    {
        prem = true;
        SetupInfo();
    }

    void RunExpt()
    {
        displayScore(LivesRemainingText);

        if (debugManager.isVR)
        {
            displayScoreVR(VRLives);
            InputDevice device = InputDevices.GetDeviceAtXRNode(XRNode.Head);
            Vector3 headPosition;
            Quaternion headRotation;
            device.TryGetFeatureValue(CommonUsages.devicePosition, out headPosition);
            device.TryGetFeatureValue(CommonUsages.deviceRotation, out headRotation);
            headInfo.Add(new HeadTracking(headPosition, headRotation));

        }
        
        positions.Add(this.transform.position);
        if (!gameOver)
        {
            updateTime();
            if (checkGameEnd(transform.position.x, transform.position.z))
            {
                Score.displayGameOver(GameOverText, "You reached the target! Congrats!");
                if (debugManager.isVR)
                {
                    Score.displayGameOverVR(GameOver, "You reached the target! Congrats!");
                }

                gameOver = true;
                return;
            }
            else if (Score.getScore() == 0)
            {
                gameOver = true;
                return;
            }

            if (!debugManager.isVR && !debugManager.isSim)
            {
                // Scene orientation is inverted, so z and x are switched.
                float x = Input.GetAxis("Horizontal");
                float z = Input.GetAxis("Vertical");
                Vector3 movement = new Vector3(z, 0, -x);
                cube_Rigidbody.MovePosition(transform.position + movement * speed * Time.deltaTime);
            }

            if (debugManager.isSim)
            {
                Debug.Log("hi");
                GetComponent<CapsuleCollider>().center = GetComponent<CharacterController>().center - oldCenter;
                oldCenter = GetComponent<CharacterController>().center;
            }
        }
        else
        {
            Results result = new Results(subjectName, isPatient, difficulty, "28-11-2022", currTimeText.text, Int32.Parse(LivesRemainingText.text.Substring(14)), eye);

            results.Add(result);
            eyeDataCollector.stopCollectingData();
            WriteToCSV.SavePositionData(positions, subjectName, curTrialNum);
            WriteToCSV.SaveHeadData(headInfo, subjectName, curTrialNum);
            if (curTrialNum < numTrials)
            {
                curTrialNum++;
                StartNewTrial();
            }
            else
            {
                if (prem)
                {
                    results.Clear();
                    showUI = true;
                    prem = false;
                    return;
                }

                if (showPath)
                {
                    var pathColour = Color.magenta;
                    var lineRenderer = this.gameObject.GetComponent<LineRenderer>();
                    lineRenderer.enabled = true;
                    lineRenderer.positionCount = positions.Count;
                    lineRenderer.SetPositions(positions.ToArray());
                    lineRenderer.startColor = pathColour;
                    lineRenderer.endColor = pathColour;
                    lineRenderer.widthMultiplier = 1;
                }

                WriteToCSV.SaveTrialData(results.ToArray(), subjectName);
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

    void RandomCylinderGenerator(int idx)
    {
        float x;
        float y;
        float z;
        bool overlap;
        do
        {
            x = UnityEngine.Random.Range(-23.0F, 23.0F);
            y = 0.5f;
            z = UnityEngine.Random.Range(-23.0f, 23.0f);
            overlap = false;
            for (int i = 0; i < idx; i++)
            {
                if (cylinders[i] != null)
                {
                    float distance = Vector3.Distance(new Vector3(x, y, z), cylinders[i].transform.position);
                    if (distance <= 4.2)
                    {
                        overlap = true;
                        break;
                    }
                }
            }
        } while ((x <= -18.0 && z >= -2.0 && z <= 5.0) || (x > 18.0F && z >= -6.0F && z <= 6.0) || overlap);
        cylinders[idx] = Instantiate(cylinder, new Vector3(x, y, z), Quaternion.identity);
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

    public int getTrialNum()
    {
        return curTrialNum;
    }

    public string getName()
    {
        return subjectName;
    }

    public int getDiff()
    {
        return difficulty;
    }

}
