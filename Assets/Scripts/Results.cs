using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Utility class for storing data.
 */
[Serializable]
public class Results
{

    public string name;
    public bool desc;
    public string date;
    public string timeElapsed;
    public int livesRemain;
    public int diff;
    public string eye;

    public Results(string name, bool desc, int diff, string date, string timeElapsed, int livesRemain, string eye)
    {
        this.name = name;
        this.desc = desc;
        this.diff = diff;
        this.date = DateTime.Now.ToString("dd-MM-yyyy"); ;
        this.timeElapsed = timeElapsed;
        this.livesRemain = livesRemain;
        this.eye = eye;
    }
}
