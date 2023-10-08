using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * JSON Write Utility
 */
public class WriteToJson { 

    [Serializable]
    private class Wrapper<T>
    {
        public T[] Data;
    }

    public static void SaveToFile<T>(T[] array, String name)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Data = array;

        string jsonTxt = JsonUtility.ToJson(wrapper, true);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/trial" + "_" + name + ".json", jsonTxt);
        Debug.Log("Saved to: " + Application.persistentDataPath + "/trial" + "_" + name + ".json");

    }
}
