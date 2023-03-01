using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public static class Score
{
    private static int initScore;

    public static int getScore() {
        return initScore;
    }

    // Start is called before the first frame update
    public static void scoreStart(int score)
    {
        initScore = score;
    }
    // Update is called once per frame
    public static void decreaseScore()
    {
        initScore--;
        checkGameEnd();
    }

    public static void checkGameEnd()
    {
        if (initScore == 0) {
            Debug.Log("game ended");
            Application.Quit();
            Debug.Break(); //remove in production
        }
    }

    public static void displayScore(Text LivesRemainingText)
    {
        LivesRemainingText.text = "Score: " + getScore();
    }
}
