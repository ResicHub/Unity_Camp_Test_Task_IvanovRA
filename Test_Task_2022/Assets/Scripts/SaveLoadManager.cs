using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Stores methods to saveing and loading curve data.
/// </summary>
public class SaveLoadManager : MonoBehaviour
{
    /// <summary>
    /// An object instance implemented with a singleton.
    /// </summary>
    public static SaveLoadManager Instance = null;

    private string filePath;

    private UnityEvent LoadingIsComplete = new UnityEvent();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        filePath = Application.dataPath + "/data.json";
    }
    
    private void Start()
    {
        LoadingIsComplete.AddListener(BezierSmoother.Instance.CreateCurve);
    }

    /// <summary>
    /// Creates config file with curve data and saving it on device.
    /// </summary>
    [ContextMenu("Saving data")]
    public void Save()
    {
        ConfigFileStruct configFile = new ConfigFileStruct()
        {
            Points = new List<Point>(),
            Loop = false
        };

        List<Vector3> anchors = CurveGenerator.Instance.Anchors;
        for (int i = 0; i < anchors.Count; i++)
        {
            Vector3 vertex = anchors[i];
            configFile.Points.Add(new Point(vertex.x, vertex.y));
        }

        configFile.Loop = CurveGenerator.Instance.Loop;
        configFile.MovementSpeed = ArrowController.Instance.MovementSpeed;
        configFile.PassageTime = ArrowController.Instance.PassageTime;

        string json = JsonUtility.ToJson(configFile);

        try
        {
            File.WriteAllText(filePath, json);
            Debug.Log("Data saving completed successfully.");
        }
        catch
        {
            Debug.LogError("Data saving failed.");
        }
    }

    /// <summary>
    /// Loads data from config file and creating curve.
    /// </summary>
    [ContextMenu("Loading data")]
    public void Load()
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError("Data loading failed. Configuration file does not exist.");
            return;
        }

        string json = File.ReadAllText(filePath);
        ConfigFileStruct configFileFromJson = JsonUtility.FromJson<ConfigFileStruct>(json);

        int pointsCount = configFileFromJson.Points.Count;
        CurveGenerator.Instance.CleanOldCurve();
        CurveGenerator.Instance.AnchorsCount = pointsCount;
        for (int i = 0; i < pointsCount; i++)
        {
            Point point = configFileFromJson.Points[i];
            CurveGenerator.Instance.Anchors.Add(new Vector3(point.X, point.Y, 0));
        }

        CurveGenerator.Instance.Loop = configFileFromJson.Loop;
        ArrowController.Instance.MovementSpeed = configFileFromJson.MovementSpeed;
        ArrowController.Instance.PassageTime = configFileFromJson.PassageTime;

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
    /// Points of curve.
    /// </summary>
    public List<Point> Points;

    /// <summary>
    /// Parameter for looping the curve.
    /// </summary>
    public bool Loop;

    /// <summary>
    /// Arrow's movement speed.
    /// </summary>
    public float MovementSpeed;

    /// <summary>
    /// The time to takes the arrow to pass the entire curve with certain movement speed.
    /// </summary>
    public float PassageTime;
}

/// <summary>
/// Serializable struct for storing coordinates of anchors.
/// </summary>
[System.Serializable]
public struct Point
{
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

    /// <summary>
    /// X-coordinate of point.
    /// </summary>
    public float X;

    /// <summary>
    /// Y-coordinate of point.
    /// </summary>
    public float Y;
}
