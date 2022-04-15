using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains all needed methods for generating anchors for new curve.
/// </summary>
public class CurveGenerator : MonoBehaviour
{
    /// <summary>
    /// An object instance implemented with a singleton.
    /// </summary>
    public static CurveGenerator Instance;

    [SerializeField]
    [Range(3, 10)]
    public int AnchorsCount = 5;

    /// <summary>
    /// Looping of curve.
    /// </summary>
    [SerializeField]
    public bool Loop;

    /// <summary>
    /// Anchor points of curve.
    /// </summary>
    public List<Vector3> Anchors = new List<Vector3>();

    // Curve zone borders by X-coordinate.
    private float[] xBorders = new float[2] 
    { 
        -6f, 6f 
    };

    // Curve zone borders by Y-coordinate.
    private float[] yBorders = new float[2] 
    {
        -4f, 4f
    };

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        GenerateAnchors();
        BezierSmoother.Instance.CreateCurve();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            GenerateAnchors();
            BezierSmoother.Instance.CreateCurve();
        }
    }

    /// <summary>
    /// Generating new random anchors.
    /// </summary>
    private void GenerateAnchors()
    {
        CleanOldCurve();
        // Generating new random anchors.
        for (int index = 0; index < AnchorsCount; index++)
        {
            Vector3 newAnchor = GetRandomVertex();
            // Making sure that the coordinates of the anchors are not repeated.
            while (Anchors.Contains(newAnchor))
            {
                newAnchor = GetRandomVertex();
            }
            Anchors.Add(newAnchor);
        }
    }

    /// <summary>
    /// Cleaning data about old anchors and control points.
    /// </summary>
    public void CleanOldCurve()
    {
        Anchors.Clear();
    }

    /// <summary>
    /// Random vertex in certain area.
    /// </summary>
    /// <returns>Vector3 with random values: x, y.</returns>
    private Vector3 GetRandomVertex()
    {
        float x = Random.Range(xBorders[0], xBorders[1]);
        float y = Random.Range(yBorders[0], yBorders[1]);
        return new Vector3(x, y);
    }
}
