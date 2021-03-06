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

    /// <summary>
    /// Cureve anchor points count.
    /// </summary>
    public int AnchorsCount;

    /// <summary>
    /// Looping of curve.
    /// </summary>
    public bool Loop;

    /// <summary>
    /// If True - generator will create a curve that does not cross itself.
    /// </summary>
    public bool NonCrossingCurve;

    /// <summary>
    /// Anchor points of curve.
    /// </summary>
    public List<Vector3> Anchors = new List<Vector3>();

    private List<Vector3> curvePositions;

    /// <summary>
    /// Curve zone borders by X-coordinate.
    /// </summary>
    private float[] xBorders = new float[2] 
    { 
        -3.5f, 7.5f 
    };

    /// <summary>
    /// Curve zone borders by Y-coordinate.
    /// </summary>
    private float[] yBorders = new float[2] 
    {
        -4f, 3f
    };

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    /// <summary>
    /// Generates new curve with certain parameters.
    /// </summary>
    public void Generate()
    {
        CleanOldCurve();
        if (NonCrossingCurve)
        {
            GenerateNonCrossingCurve();
        }
        else
        {
            GenerateAnchors(AnchorsCount);
        }
        BezierSmoother.Instance.CreateCurve();
    }

    /// <summary>
    /// Generates new random anchors.
    /// </summary>
    /// <param name="count"></param>
    private void GenerateAnchors(int count)
    {
        for (int index = 0; index < count; index++)
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

    private void GenerateNonCrossingCurve()
    {
        // Needed for restart generating if the generation process is too long.
        int attempts = 0;
        // Generate 3 anchors (because they are definitely would not cross).
        GenerateAnchors(3);
        for (int index = 0; index < AnchorsCount - 3; index++)
        {
            GenerateAnchors(1);
            // Creating curve after each new anchor to checking for crossing.
            BezierSmoother.Instance.CreateCurve();

            // If the curve is crosses itself -> remove last ahcnor and try again.
            if (CheckCurveCrossing())
            {
                Anchors.RemoveAt(Anchors.Count - 1);
                index--;
                attempts++;
            }
            // If generation process is too long -> generation stops.
            if (attempts > 100)
            {
                break;
            }
        }
        // If generation process is too long -> new generation starts.
        if (attempts > 100)
        {
            Generate();
        }
    }

    /// <summary>
    /// Cleans data about old anchors.
    /// </summary>
    public void CleanOldCurve()
    {
        Anchors.Clear();
    }

    private Vector3 GetRandomVertex()
    {
        float x = Random.Range(xBorders[0], xBorders[1]);
        float y = Random.Range(yBorders[0], yBorders[1]);
        return new Vector3(x, y);
    }

    /// <summary>
    /// Return true if curve is crosses itself.
    /// </summary>
    private bool CheckCurveCrossing()
    {
        int segmentsCount = BezierSmoother.Instance.LineRenderer.positionCount - 1;
        curvePositions = BezierSmoother.Instance.GetCurvePositions();
        // Check each segment in pairs for crossing.
        for (int i = 1; i < segmentsCount - 2; i++)
        {
            for (int j = i + 2; j < segmentsCount + 1; j++)
            {
                if (CheckVectorsCrossing(
                    curvePositions[i - 1], curvePositions[i], 
                    curvePositions[j - 1], curvePositions[j]))
                {
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Returns true if vectors (Vector1Start, Vector1End) and (Vector2Start, Vector2End) are crossing.
    /// </summary>
    /// <param name="Vector1Start"></param>
    /// <param name="Vector1End"></param>
    /// <param name="Vector2Start"></param>
    /// <param name="Vector2End"></param>
    private bool CheckVectorsCrossing(
        Vector3 Vector1Start, Vector3 Vector1End,
        Vector3 Vector2Start, Vector3 Vector2End)
    {
        Vector3 vector1 = Vector1End - Vector1Start;
        Vector3 vector2 = Vector2End - Vector2Start;

        // If vectors are parallel.
        if (vector1.x / vector2.x == vector1.y / vector2.y)
        {
            // If vectors lay on the same line.
            if (Vector3.Angle(Vector1Start, Vector3.up) % 90 ==
                Vector3.Angle(Vector2Start, Vector3.up) % 90)
            {
                Vector3 center1 = (Vector1End + Vector1Start) / 2;
                Vector3 center2 = (Vector2End + Vector2Start) / 2;
                // If part of the first vector lay on the second vector.
                if ((center2 - center1).magnitude <
                    (vector1.magnitude + vector2.magnitude) / 2)
                {
                    return true;
                }
            }
        }
        else
        {
            // If vector1 points on different sides of vector2 and vice versa.
            if (CheckDifferentLineSides(Vector1Start, Vector1End, Vector2Start, Vector2End) &&
                CheckDifferentLineSides(Vector2Start, Vector2End, Vector1Start, Vector1End))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Returns true if vectorStart an vectorEnd on diffenert sides of line (lineStart, lineEnd).
    /// </summary>
    /// <param name="vectorStart"></param>
    /// <param name="vectorEnd"></param>
    /// <param name="lineStart"></param>
    /// <param name="lineEnd"></param>
    private bool CheckDifferentLineSides(
        Vector3 vectorStart, Vector3 vectorEnd,
        Vector3 lineStart, Vector3 lineEnd)
    {
        // If points on different sides od line, one of the angle sign will be > 0 and other one < 0.
        // And their multiplication will be < 0.
        if (GetAngleSign(vectorStart, lineStart, lineEnd) *
            GetAngleSign(vectorEnd, lineStart, lineEnd) < 0)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Returns 0 if angle = 0 or 180 deg.
    /// Returns value less than 0 if angle between -180 and 0 deg.
    /// Returns value more than 0 if angle between 0 and 180 deg.
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="p3"></param>
    /// <returns>Value, equals (p2, p1).x * (p2, p3).y - (p2, p1).y * (p2, p3).x</returns>
    private float GetAngleSign(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        Vector3 vector1 = p1 - p2;
        Vector3 vector2 = p3 - p2;
        return vector1.x * vector2.y - vector1.y * vector2.x;
    }
}
