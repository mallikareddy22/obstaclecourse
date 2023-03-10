using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using static Score;

public class random_location : MonoBehaviour
{
    public Text LivesRemainingText;
    public Text GameOverText;
    float x;
    float y;
    float z;
    Vector3 pos;
    float timePrev;
    float numTimesBeforeScoreDecrease;
    public GameObject cylinder;
    Vector3 movement;
    public float speed = 2;
    // Start is called before the first frame update
    void Start()
    {
        Score.scoreStart(10);
        speed = 2;
        numTimesBeforeScoreDecrease = 50;
        x = 24.0F;
        y = 0.25F;
        z = Random.Range(-4, 4);
        pos = new Vector3(x, y, z);
        transform.position = pos;
        timePrev = 0;
        for (int i = 0; i < Random.Range(30, 35); i++) {
            RandomCylinderGenerator();
        }
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
        transform.Translate(-movement * Time.deltaTime);
        numTimesBeforeScoreDecrease--;
        if (numTimesBeforeScoreDecrease == 0) {
            Score.decreaseScore();
            numTimesBeforeScoreDecrease = 50;
            Debug.Log(Score.getScore());
        }
    }

    // Update is called once per frame
    void Update()
    {
        displayScore(LivesRemainingText);
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        timePrev = Time.deltaTime;
        MoveObject(x, z, timePrev);
        if (checkGameEnd(transform.position.x, transform.position.z)) {
            gameOver("You reached the target! Congrats!");
        }
    }

    void MoveObject(float x, float z, float time = 1) {
        movement = new Vector3(x, 0, z);
        movement = Vector3.ClampMagnitude(movement, 1);
        transform.Translate(movement * speed * time);
    }

    void RandomCylinderGenerator() {
        float x = Random.Range(-24.0F, 24.0F);
        float y = 5.0f;
        float z = Random.Range(-24.0f, 24.0f);
        if (!(x <= -19.8 && z >= -2.7 && z <= 2.7) && !(x > 22.0F && z >= -6.0F && z <= 6.0)) {
            Instantiate(cylinder, new Vector3(x, y, z), Quaternion.identity);
        }
        
    }

    bool checkGameEnd(float x, float z) {
        return x <= -19.8 && z >= -2.7 && z <= 2.7;
    }

    void gameOver(string message) {
        GameOverText.text = message;
    }
}

