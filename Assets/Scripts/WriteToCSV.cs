using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/**
 * CSV Write Utility
 */
public class WriteToCSV { 

    [Serializable]
    private class Wrapper<T>
    {
        public T[] Data;
    }

    public static void SaveToFile<T>(T[] array, String name)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Data = array;

        String filePath = Application.persistentDataPath + "/trial" + "_" + name + ".csv";
        String header = "name,desc,diff,date,time_elapsed,lives_remain,eye";
        StreamWriter streamWriter = File.AppendText(filePath);
        streamWriter.WriteLine(header);

        foreach (var result in wrapper.Data)
        {
            streamWriter.WriteLine(result.ToString());
        }

        streamWriter.Close();
        Debug.Log("Saved to: " + Application.persistentDataPath + "/trial" + "_" + name + ".csv");

    }
}
