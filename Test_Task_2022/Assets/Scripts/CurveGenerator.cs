using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// That class contains all needed methods for generating new curve,
/// You should enter a number of curve vertices before starting generator.
/// The vertex coordinates generates randomly in a certain area.
/// </summary>
public class CurveGenerator : MonoBehaviour
{
    [SerializeField]
    private int pointsCount;

    [SerializeField]
    private LineRenderer lineRenderer;

    private UnityEvent GenerationDone;

    private float[] xBorders = new float[2] // Curve zone borders by X-coordinate
    { 
        -6f, 6f 
    };

    private float[] yBorders = new float[2] // Curve zone borders by Y-coordinate
    {
        -4f, 4f
    };

    [ContextMenu("Start generation")]
    private void Generate()
    {
        if (pointsCount > 1 && pointsCount <= 100)
        {
            lineRenderer.positionCount = pointsCount;
            for (int i = 0; i < pointsCount; i++)
            {
                lineRenderer.SetPosition(i, GetRandomVertex());
            }
            GenerationDone.Invoke();
        }
        else
        {
            Debug.LogWarning("Number of points should be not less than 2 and not more than 100.");
        }
    }

    private Vector3 GetRandomVertex()
    {
        float x = Random.Range(xBorders[0], xBorders[1]);
        float y = Random.Range(yBorders[0], yBorders[1]);
        return new Vector3(x, y, 0);
    }
}
