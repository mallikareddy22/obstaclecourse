using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

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
    public static void decreaseScore(Text GameOverText)
    {
        if (initScore > 0)
        {
            initScore--;
        }
        if (checkGameEnd()) {
            Debug.Log("game ended");
            displayGameOver(GameOverText, "You ran out of lives...");
        }
    }

    public static void decreaseScoreVR(TextMeshPro GameOverText)
    {
        if (checkGameEnd())
        {
            Debug.Log("game ended");
            displayGameOverVR(GameOverText, "You ran out of lives...");
        }
    }

    public static bool checkGameEnd()
    {
        return initScore == 0;
    }

    public static void displayScore(Text LivesRemainingText)
    {
        LivesRemainingText.text = "Lives Remain: " + getScore();
    }

    public static void displayScoreVR(TextMeshPro VRText)
    {
        VRText.text = "Lives Remain: " + getScore();
    }

    public static void displayGameOver(Text GameOverText, string message) 
    {
        GameOverText.text = message;
    }

    public static void displayGameOverVR(TextMeshPro GameOverText, string message)
    {
        GameOverText.text = message;
    }
}
