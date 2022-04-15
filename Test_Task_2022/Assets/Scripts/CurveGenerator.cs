using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Contains all needed methods for generating new curve,
/// </summary>
public class CurveGenerator : MonoBehaviour
{
    /// <summary>
    /// An object instance implemented with a singleton.
    /// </summary>
    public static CurveGenerator Instance;

    [SerializeField]
    private LineRenderer lineRenderer;

    [SerializeField]
    private GameObject anchorPointPrefab;

    [SerializeField]
    private Transform groupOfAnchors;

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

    // Needed for crearing Bezier curve.
    private Dictionary<Vector3, List<Vector3>> controlPoints = 
        new Dictionary<Vector3, List<Vector3>>();

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

    private UnityEvent GenerationIsComplete = new UnityEvent();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        GenerationIsComplete.AddListener(ArrowController.Instance.GetReady);
        GenerateAnchors();
        CreateCurve();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            GenerateAnchors();
            CreateCurve();
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
        controlPoints.Clear();
    }

    /// <summary>
    /// Creating the Bezier curve, and drowing the points on scene.
    /// </summary>
    public void CreateCurve()
    {
        SetControlPoints();
        SmoothOut();
        DrawPoints();
        GenerationIsComplete.Invoke();
    }

    /// <summary>
    /// Setting up control points for each anchor.
    /// </summary>
    private void SetControlPoints()
    {
        // Setting up control points for each ahcnor of curve except extreme ahcnors (start and end).
        for (int index = 0; index < AnchorsCount - 2; index++)
        {
            SetControlPointsForAnchor(Anchors[index], Anchors[index + 1], Anchors[index + 2]);
        }

        // Setting up control points for extreme ahcnors.
        SetControlPointsForAnchor(Anchors[AnchorsCount - 1], Anchors[0], Anchors[1]);
        SetControlPointsForAnchor(Anchors[AnchorsCount - 2], Anchors[AnchorsCount - 1], Anchors[0]);
    }

    /// <summary>
    /// Setting up control points for anchor2.
    /// </summary>
    /// <param name="anchor1"></param>
    /// <param name="anchor2"></param>
    /// <param name="anchor3"></param>
    private void SetControlPointsForAnchor(Vector3 anchor1, Vector3 anchor2, Vector3 anchor3)
    {
        Vector3 tangent = (anchor3 - anchor1) / 3f;
        controlPoints.Add(anchor2, new List<Vector3>()
        {
            anchor2 - tangent,
            anchor2 + tangent
        });
    }

    /// <summary>
    /// Creating segments of curve with Bezier algorithm.
    /// </summary>
    private void SmoothOut()
    {
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, Anchors[0]);

        int index = 1;
        // Creating of segments between two next curve anchors.
        for (int i = 0; i < AnchorsCount - 1; i++)
        {
            CreateCurveSevment(Anchors[i], Anchors[i + 1], ref index);
        }

        // If curve is looping -> creating segmet between start and end ahcnors.
        if (Loop)
        {
            CreateCurveSevment(Anchors[AnchorsCount - 1], Anchors[0], ref index);
        }
    }

    /// <summary>
    /// Creating a Bezier curve segment between two ahcnors.
    /// </summary>
    /// <param name="anchor1"></param>
    /// <param name="anchor2"></param>
    /// <param name="index"></param>
    private void CreateCurveSevment(Vector3 anchor1, Vector3 anchor2, ref int index)
    {
        lineRenderer.positionCount += 10;
        for (float part = 0.1f; part < 1.1f; part += 0.1f)
        {
            lineRenderer.SetPosition(index,
                CubicLerp(
                    anchor1,
                    controlPoints[anchor1][1],
                    controlPoints[anchor2][0],
                    anchor2,
                    part));
            index++;
        }
    }

    /// <summary>
    /// Creating anchor points on the scene.
    /// </summary>
    private void DrawPoints()
    {
        // Destruction old drawn anchor points (if there are on scene).
        foreach (Transform child in groupOfAnchors)
        {
            Destroy(child.gameObject);
        }

        foreach (Vector3 anchor in Anchors)
        {
            Instantiate(
                anchorPointPrefab,
                anchor,
                Quaternion.identity,
                groupOfAnchors);
        }
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

    /// <summary>
    /// Bilinear interpolates between three points.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="c"></param>
    /// <param name="t"></param>
    /// <returns>Vector3, equals to Lerp(a, b, t) + (Lerp(b, c, t) - Lerp(a, b, t)) * t</returns>
    private Vector3 QuadraticLerp(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        Vector3 p0 = Vector3.Lerp(a, b, t);
        Vector3 p1 = Vector3.Lerp(b, c, t);
        return Vector3.Lerp(p0, p1, t);
    }

    /// <summary>
    /// Calculate cubic interpolation between four points.
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="c"></param>
    /// <param name="d"></param>
    /// <param name="t"></param>
    /// <returns>Vector3, equals to QuadraticLerp(a, b, c, t) + (QuadraticLerp(b, c, d, t) - QuadraticLerp(a, b, c, t)) * t</returns>
    private Vector3 CubicLerp(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
    {
        Vector3 p0 = QuadraticLerp(a, b, c, t);
        Vector3 p1 = QuadraticLerp(b, c, d, t);
        return Vector3.Lerp(p0, p1, t);
    }
}
