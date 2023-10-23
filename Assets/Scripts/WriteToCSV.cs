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

    public static void SaveTrialData<T>(T[] array, string name)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Data = array;

        string filePath = Application.persistentDataPath + "/" + name + "/trial" + "_" + name + ".csv";
        string header = "name,is_patient,diff,date,time_elapsed,lives_remain,eye";
        (new FileInfo(filePath)).Directory.Create();
        StreamWriter streamWriter = File.AppendText(filePath);
        streamWriter.WriteLine(header);

        foreach (var result in wrapper.Data)
        {
            streamWriter.WriteLine(result.ToString());
        }

        streamWriter.Close();
        Debug.Log("Saved to: " + Application.persistentDataPath + "/" + name + "/trial" + "_" + name + ".csv");

    }

    public static void SavePositionData(List<Vector3> positions, string name, int trialNum)
    {
        string filePath = Application.persistentDataPath + "/" + name + "/trial" + "_" + trialNum + "_pos.csv";
        (new FileInfo(filePath)).Directory.Create();
        StreamWriter streamWriter = File.AppendText(filePath);
        string header = "x,y,z";
        streamWriter.WriteLine(header);

        foreach (Vector3 pos in positions)
        {
            string line = pos.x + "," + pos.y + "," + pos.z;
            streamWriter.WriteLine(line);
        }

        streamWriter.Close();
        Debug.Log("Saved position to: " + Application.persistentDataPath + "/" + name + "/trial" + "_" + trialNum + "_pos.csv");
    }

    public static void SaveHeadData(List<HeadTracking> headInfo, string name, int trialNum)
    {
        string filePath = Application.persistentDataPath + "/" + name + "/trial" + "_" + trialNum + "_head.csv";
        (new FileInfo(filePath)).Directory.Create();
        StreamWriter streamWriter = File.AppendText(filePath);
        string header = "pos_x,pos_y,pos_z,rot_x,rot_y,rot_z";
        streamWriter.WriteLine(header);

        foreach (HeadTracking headNode in headInfo)
        {
            string line = headNode.getHeadPosition() + "," + headNode.getHeadRotation();
            streamWriter.WriteLine(line);
        }

        streamWriter.Close();
        Debug.Log("Saved position to: " + Application.persistentDataPath + "/" + name + "/trial" + "_" + trialNum + "_head.csv");
    }
}
