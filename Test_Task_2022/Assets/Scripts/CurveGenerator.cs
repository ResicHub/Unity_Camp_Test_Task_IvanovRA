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
    public static CurveGenerator Instance = null;

    [SerializeField]
    private LineRenderer lineRenderer;

    [SerializeField]
    private GameObject anchorPointPrefab;

    [SerializeField]
    private Transform groupOfAnchors;

    [SerializeField]
    [Range(3, 10)]
    private int anchorsCount = 5;

    [SerializeField]
    private bool loop;

    private List<Vector3> anchors = new List<Vector3>();

    private Dictionary<Vector3, List<Vector3>> controlPoints = new Dictionary<Vector3, List<Vector3>>();

    private float[] xBorders = new float[2] // Curve zone borders by X-coordinate.
    { 
        -6f, 6f 
    };

    private float[] yBorders = new float[2] // Curve zone borders by Y-coordinate.
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
        Generate();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            Generate();
        }
    }

    /// <summary>
    /// Generate the new curve.
    /// </summary>
    public void Generate()
    {
        anchors.Clear();
        controlPoints.Clear();
        for (int i = 0; i < anchorsCount; i++)
        {
            Vector3 newAnchor = GetRandomVertex();
            while (anchors.Contains(newAnchor))
            {
                newAnchor = GetRandomVertex();
            }
            anchors.Add(newAnchor);
        }

        SetControlPoints();
        SmoothOut();
        DrawPoints();
        GenerationIsComplete.Invoke();
    }

    private void SetControlPoints()
    {
        for (int i = 0; i < anchorsCount - 2; i++)
        {
            SetControlPointsForAnchor(anchors[i], anchors[i + 1], anchors[i + 2]);
        }

        SetControlPointsForAnchor(anchors[anchorsCount - 1], anchors[0], anchors[1]);
        SetControlPointsForAnchor(anchors[anchorsCount - 2], anchors[anchorsCount - 1], anchors[0]);
    }

    private void SetControlPointsForAnchor(Vector3 a, Vector3 b, Vector3 c)
    {
        Vector3 tangent = (c - a) / 3f;
        controlPoints.Add(b, new List<Vector3>()
        {
            b - tangent,
            b + tangent
        });
    }

    private void SmoothOut()
    {
        lineRenderer.positionCount = (anchorsCount - 1) * 10 + 1;
        lineRenderer.SetPosition(0, anchors[0]);

        int index = 1;
        for (int i = 0; i < anchorsCount - 1; i++)
        {
            CreateCurveSevment(anchors[i], anchors[i + 1], ref index);
        }

        if (loop)
        {
            lineRenderer.positionCount += 10;
            CreateCurveSevment(anchors[anchorsCount - 1], anchors[0], ref index);
        }
    }

    private void CreateCurveSevment(Vector3 anchor1, Vector3 anchor2, ref int index)
    {
        for (float j = 0.1f; j < 1.1f; j += 0.1f)
        {
            lineRenderer.SetPosition(index,
                CubicCurve(
                    anchor1,
                    controlPoints[anchor1][1],
                    controlPoints[anchor2][0],
                    anchor2,
                    j));
            index++;
        }
    }

    private void DrawPoints()
    {
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
    /// Returns random vertex of curve in certain area.
    /// </summary>
    private Vector3 GetRandomVertex()
    {
        float x = Random.Range(xBorders[0], xBorders[1]);
        float y = Random.Range(yBorders[0], yBorders[1]);
        return new Vector3(x, y);
    }

    private Vector3 Lerp(Vector3 a, Vector3 b, float t)
    {
        return a + (b - a) * t;
    }

    private Vector3 QuadraticCurve(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        Vector3 p0 = Lerp(a, b, t);
        Vector3 p1 = Lerp(b, c, t);
        return Lerp(p0, p1, t);
    }

    private Vector3 CubicCurve(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
    {
        Vector3 p0 = QuadraticCurve(a, b, c, t);
        Vector3 p1 = QuadraticCurve(b, c, d, t);
        return Lerp(p0, p1, t);
    }
}
