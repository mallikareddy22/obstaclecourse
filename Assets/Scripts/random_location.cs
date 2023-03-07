using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using static Score;

public class random_location : MonoBehaviour
{
    public Text LivesRemainingText;
    float x;
    float y;
    float z;
    Vector3 pos;
    float timePrev;
    float numTimesBeforeScoreDecrease;
    Vector3 movement;
    // Start is called before the first frame update
    void Start()
    {
        Score.scoreStart(10);
        numTimesBeforeScoreDecrease = 50;
        x = Random.Range(-4, 4);
        y = 0.2F;
        z = Random.Range(-4, 4);
        pos = new Vector3(x, y, z);
        transform.position = pos;
        timePrev = 0;
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other);
        Debug.Log(other.gameObject.name);
        if (other.gameObject.name == "Player")
        {
            Debug.Log("hi");
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


    public float speed = 2;
    // Update is called once per frame
    void Update()
    {
        displayScore(LivesRemainingText);
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        timePrev = Time.deltaTime;
        MoveObject(x, z, timePrev);
    }

    void MoveObject(float x, float z, float time = 1) {
        movement = new Vector3(x, 0, z);
        movement = Vector3.ClampMagnitude(movement, 1);
        transform.Translate(movement * speed * time);
    }
}

