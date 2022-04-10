using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Stores methods to saveing and loading path data.
/// </summary>
public class SaveLoadManager : MonoBehaviour
{
    /// <summary>
    /// An object instance implemented with a singleton.
    /// </summary>
    public static SaveLoadManager Instance = null;

    private string pathFile;

    private UnityEvent LoadingIsComplete = new UnityEvent();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        pathFile = Application.dataPath + "/data.json";
    }

    private void Start()
    {
        LoadingIsComplete.AddListener(ArrowController.Instance.GetReady);
    }

    /// <summary>
    /// Creating config file with path data and saving it on device.
    /// </summary>
    [ContextMenu("Saving data")]
    public void Save()
    {
        ConfigFileStruct configFile = new ConfigFileStruct()
        {
            points = new List<Point>()
        };

        LineRenderer lineRenderer = ArrowController.Instance.MyLineRenderer;
        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            Vector3 vertex = lineRenderer.GetPosition(i);
            configFile.points.Add(new Point(vertex.x, vertex.y));
        }

        string json = JsonUtility.ToJson(configFile);

        try
        {
            File.WriteAllText(pathFile, json);
            Debug.Log("Data saving completed successfully.");
        }
        catch
        {
            Debug.LogError("Data saving failed.");
        }
    }

    /// <summary>
    /// Loading path data from config file and creating curve..
    /// </summary>
    [ContextMenu("Loading data")]
    public void Load()
    {
        if (!File.Exists(pathFile))
        {
            Debug.LogError("Data loading failed.");
            return;
        }

        string json = File.ReadAllText(pathFile);
        ConfigFileStruct configFileFromJson = JsonUtility.FromJson<ConfigFileStruct>(json);

        int pointCount = configFileFromJson.points.Count;
        ArrowController.Instance.MyLineRenderer.positionCount = pointCount;
        for (int i = 0; i < pointCount; i++)
        {
            Point point = configFileFromJson.points[i];
            ArrowController.Instance.MyLineRenderer.SetPosition(i, new Vector3(point.X, point.Y, 0));
        }
        LoadingIsComplete.Invoke();
        Debug.Log("Data loading completed successfully.");
    }
}

/// <summary>
/// Serializable struct for storing path data.
/// </summary>
[System.Serializable]
public struct ConfigFileStruct
{
    /// <summary>
    /// Points of path.
    /// </summary>
    public List<Point> points; 
}

/// <summary>
/// Serializable struct for storing points coordinates.
/// </summary>
[System.Serializable]
public struct Point
{
    /// <summary>
    /// X-coordinate of point.
    /// </summary>
    public float X;

    /// <summary>
    /// Y-coordinate of point.
    /// </summary>
    public float Y;

    /// <summary>
    /// Constructor of path point.
    /// </summary>
    /// <param name="xCord"></param>
    /// <param name="yCord"></param>
    public Point(float xCord, float yCord)
    {
        X = xCord;
        Y = yCord;
    }
}
