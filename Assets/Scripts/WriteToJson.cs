using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WriteToJson : MonoBehaviour
{
    [Serializable]
    public class WriteToJsonHelper
    {
        public string name;
        public bool desc;
        public string date;
        public string timeElapsed;
        public int livesRemain;
        public int diff;
        public string eye;
    }

    private WriteToJsonHelper writeAttributes = new WriteToJsonHelper();

    public WriteToJson(string name, bool desc, int diff, string date, string timeElapsed, int livesRemain, string eye)
    {
        writeAttributes.name = name;
        writeAttributes.desc = desc;
        writeAttributes.diff = diff;
        writeAttributes.date = DateTime.Now.ToString("dd-MM-yyyy"); ;
        writeAttributes.timeElapsed = timeElapsed;
        writeAttributes.livesRemain = livesRemain;
        writeAttributes.eye = eye;
    }

    public string SaveToString()
    {
        return JsonUtility.ToJson(writeAttributes);
    }

    public void SaveToFile(int trialNum)
    {
        string jsonTxt = JsonUtility.ToJson(writeAttributes);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/trial" + trialNum + "_" + writeAttributes.name + ".json", jsonTxt);
        Debug.Log("Saved to: " + Application.persistentDataPath + "/trial" + trialNum + "_" + writeAttributes.name + ".json");

    }
}
