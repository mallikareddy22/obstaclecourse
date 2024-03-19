using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GeneratePositionManager : MonoBehaviour
{
    public string pathName = "/randomPositions.txt";
    public GameObject cylinder;

    public int minEasy = 40;
    public int maxEasy = 50;
    public int minMedium = 60;
    public int maxMedium = 70;
    public int minHard = 80;
    public int maxHard = 90;

    private List<List<Vector3>>[] positionList = new List<List<Vector3>>[4];
    private List<GameObject> activeCylinders = new List<GameObject>();
    private int curDifficulty;

    public static GeneratePositionManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public void GeneratePositions(int difficulty)
    {
        int curMin = 0;
        int curMax = 0;
        switch (difficulty)
        {
            case 1:
                curMin = minEasy;
                curMax = maxEasy;
                break;
            case 2:
                curMin = minMedium;
                curMax = maxMedium;
                break;
            case 3:
                curMin = minHard;
                curMax = maxHard;
                break;
        }

        ClearCylinders();

        int numCylinders = UnityEngine.Random.Range(curMin, curMax);
        curDifficulty = difficulty;
        for (int i = 0; i < numCylinders; i++)
        {
            RandomCylinderGenerator();
        }
    }
    private void ClearCylinders()
    {
        foreach (GameObject activeCylinder in activeCylinders)
        {
            Destroy(activeCylinder);
        }
        activeCylinders.Clear();
    }
    private void RandomCylinderGenerator()
    {
        int MAX_GENERATE = 100;
        for (int i = 0; i < MAX_GENERATE; i++)
        {
            float x = UnityEngine.Random.Range(-23.0F, 23.0F);
            float y = 1.0f;
            float z = UnityEngine.Random.Range(-23.0f, 23.0f);

            if (!(x <= -18.0 && z >= -2.0 && z <= 5.0) && !(x > 22.0F && z >= -6.0F && z <= 6.0))
            {
                activeCylinders.Add(Instantiate(cylinder, new Vector3(x, y, z), Quaternion.identity));
                return;
            }
        }
    }

    public void StoreCurPositions()
    {
        Vector3[] curPositions = new Vector3[activeCylinders.Count];
        for (int i = 0; i < activeCylinders.Count; i++)
        {
            curPositions[i] = activeCylinders[i].transform.position;
        }
        WritePositions(curDifficulty, curPositions);
    }

    private void WritePositions(int difficulty, Vector3[] positions)
    {
        string path = "Assets" + pathName;
        StreamWriter writer = new StreamWriter(path, true);
        writer.Write(difficulty + ": ");
        foreach (Vector3 position in positions)
        {
            writer.Write(position + " ");
        }
        writer.WriteLine();
        writer.Close();
    }

    public void ReadPositions()
    {
        for (int i = 0; i < 4; i++)
        {
            positionList[i] = new List<List<Vector3>>();
        }
        string path = "Assets" + pathName;
        Debug.Log(path);
        StreamReader reader = new StreamReader(path);
        while (!reader.EndOfStream)
        {
            string curLine = reader.ReadLine();
            Debug.Log(curLine);
            int difficulty = Int32.Parse(curLine.Substring(0, curLine.IndexOf(':')));
            List<Vector3> curCombination = GetElements(curLine.Substring(curLine.IndexOf('(')));
            positionList[difficulty].Add(curCombination);
            Debug.Log(curCombination);
        }
        reader.Close();
    }
    private List<Vector3> GetElements(string vectors)
    {
        List<Vector3> list = new List<Vector3>();
        char[] array = vectors.ToCharArray();
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == '(')
            {
                int end = vectors.IndexOf(')', i);
                String[] vectorArray = vectors.Substring(i + 1, end - (i + 1)).Split(new[] { ", " }, StringSplitOptions.None);
                Vector3 curPosition = new Vector3((float) Convert.ToDouble(vectorArray[0]),
                    (float) Convert.ToDouble(vectorArray[1]),
                    (float) Convert.ToDouble(vectorArray[2]));
                list.Add(curPosition);
            }
        }
        return list;
    }

    public void LoadPosition(string input)
    {
        string[] array = input.Split(' ');
        int difficulty = Int32.Parse(array[0]);
        int index = Int32.Parse(array[1]);
        if (index >= positionList[difficulty].Count)
        {
            Debug.LogError("Index out of bounds");
            return;
        }

        ClearCylinders();

        List<Vector3> curPositions = positionList[difficulty][index];
        foreach (Vector3 position in curPositions)
        {
            activeCylinders.Add(Instantiate(cylinder, new Vector3(position.x, position.y, position.z), Quaternion.identity));
        }
    }
}
