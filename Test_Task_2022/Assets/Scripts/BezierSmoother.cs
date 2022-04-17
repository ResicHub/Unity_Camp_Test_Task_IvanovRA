using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Contains all needed methods for create Bezier curve.
/// </summary>
public class BezierSmoother : MonoBehaviour
{
    /// <summary>
    /// An object instance implemented with a singleton.
    /// </summary>
    public static BezierSmoother Instance;

    /// <summary>
    /// Renders the curve line.
    /// </summary>
    [SerializeField]
    public LineRenderer LineRenderer;

    [SerializeField]
    private GameObject anchorPointPrefab;

    [SerializeField]
    private Transform groupOfAnchors;

    /// <summary>
    /// Used for crearing Bezier curve.
    /// </summary>
    private Dictionary<Vector3, List<Vector3>> controlPoints =
        new Dictionary<Vector3, List<Vector3>>();

    private int anchorsCount;

    private bool loop;

    private List<Vector3> anchors;

    private UnityEvent GenerationIsComplete = new UnityEvent();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        GenerationIsComplete.AddListener(ArrowController.Instance.GetReady);
    }

    /// <summary>
    /// Creates the Bezier curve, and drowing the points on scene.
    /// </summary>
    public void CreateCurve()
    {
        anchorsCount = CurveGenerator.Instance.Anchors.Count;
        loop = CurveGenerator.Instance.Loop;
        anchors = CurveGenerator.Instance.Anchors;

        SetControlPoints();
        SmoothOut();
        DrawPoints();
        GenerationIsComplete.Invoke();
    }

    /// <summary>
    /// Sets up control points for each anchor.
    /// </summary>
    private void SetControlPoints()
    {
        controlPoints.Clear();
        // Setting up control points for each ahcnor of curve except extreme ahcnors (start and end).
        for (int index = 0; index < anchorsCount - 2; index++)
        {
            SetControlPointsForAnchor(anchors[index], anchors[index + 1], anchors[index + 2]);
        }

        // Setting up control points for extreme ahcnors.
        SetControlPointsForAnchor(anchors[anchorsCount - 1], anchors[0], anchors[1]);
        SetControlPointsForAnchor(anchors[anchorsCount - 2], anchors[anchorsCount - 1], anchors[0]);
    }

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
    /// Creates segments of curve with Bezier algorithm.
    /// </summary>
    private void SmoothOut()
    {
        LineRenderer.positionCount = 1;
        LineRenderer.SetPosition(0, anchors[0]);

        int index = 1;
        // Creating of segments between two next curve anchors.
        for (int i = 0; i < anchorsCount - 1; i++)
        {
            CreateCurveSevment(anchors[i], anchors[i + 1], ref index);
        }

        // If curve is looping -> creating segmet between start and end ahcnors.
        if (loop)
        {
            CreateCurveSevment(anchors[anchorsCount - 1], anchors[0], ref index);
        }
    }

    /// <summary>
    /// Creates a Bezier curve segment between two ahcnors.
    /// </summary>
    /// <param name="anchor1"></param>
    /// <param name="anchor2"></param>
    /// <param name="index"></param>
    private void CreateCurveSevment(Vector3 anchor1, Vector3 anchor2, ref int index)
    {
        LineRenderer.positionCount += 10;
        for (float part = 0.1f; part < 1.1f; part += 0.1f)
        {
            LineRenderer.SetPosition(index,
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
    /// Returns list of all LineRenderer positions.
    /// </summary>
    public List<Vector3> GetCurvePositions()
    {
        List<Vector3> positions = new List<Vector3>();
        for (int index = 0; index < LineRenderer.positionCount; index++)
        {
            positions.Add(LineRenderer.GetPosition(index));
        }
        return positions;
    }

    private void DrawPoints()
    {
        // Destruction old drawn anchor points (if there are on scene).
        foreach (Transform child in groupOfAnchors)
        {
            Destroy(child.gameObject);
        }

        foreach (Vector3 anchor in anchors)
        {
            Instantiate(
                anchorPointPrefab,
                anchor,
                Quaternion.identity,
                groupOfAnchors);
        }
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
    /// Calculates cubic interpolation between four points.
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
