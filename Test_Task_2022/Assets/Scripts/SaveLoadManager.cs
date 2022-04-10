using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoadManager : MonoBehaviour
{
    
}

/// <summary>
/// Serializable struct for storing path data.
/// </summary>
[System.Serializable]
public struct ConfigFileStruct
{
    private List<Point> points; 
}

/// <summary>
/// Serializable struct for storing points coordinates.
/// </summary>
[System.Serializable]
public struct Point
{
    public float X;
    public float Y;

    public Point(float xCord, float yCord)
    {
        X = xCord;
        Y = yCord;
    }
}
