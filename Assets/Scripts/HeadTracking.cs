using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class HeadTracking
{
    Vector3 headPosition;
    Quaternion headRotation;

    public HeadTracking(Vector3 headPosition, Quaternion headRotation)
    {
        this.headPosition = headPosition;
        this.headRotation = headRotation;
    }

    public string getHeadPosition()
    {
        return headPosition.x + "," + headPosition.y + "," + headPosition.z;
    }

    public string getHeadRotation()
    {
        return headRotation.x + "," + headRotation.y + "," + headRotation.z;
    }
}