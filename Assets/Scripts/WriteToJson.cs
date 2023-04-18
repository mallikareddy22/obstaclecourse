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
        public string desc;
        public string date;
        public string timeElapsed;
        public string livesRemain;
    }

    private WriteToJsonHelper writeAttributes = new WriteToJsonHelper();

    public WriteToJson(string name, string desc, string date, string timeElapsed, string livesRemain)
    {
        writeAttributes.name = name;
        writeAttributes.desc = desc;
        writeAttributes.date = DateTime.Now.ToString("dd-MM-yyyy"); ;
        writeAttributes.timeElapsed = timeElapsed;
        writeAttributes.livesRemain = livesRemain;
    }

    public string SaveToString()
    {
        return JsonUtility.ToJson(writeAttributes);
    }

    public void SaveToFile(int trialNum)
    {
        string jsonTxt = JsonUtility.ToJson(writeAttributes);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/trial" + trialNum + ".json", jsonTxt);
        Debug.Log("Saved to: " + Application.persistentDataPath + "/trial" + trialNum + ".json");

    }
}
