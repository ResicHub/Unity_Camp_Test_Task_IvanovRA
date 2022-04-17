using UnityEngine;

/// <summary>
/// Controller with movement logic for arrow.
/// </summary>
public class ArrowController : MonoBehaviour
{
    /// <summary>
    /// An object instance implemented with a singleton.
    /// </summary>
    public static ArrowController Instance;

    private Transform myTransform;

    [SerializeField]
    private LineRenderer lineRenderer;

    private bool isMoving = false;

    /// <summary>
    /// Arrow's movement speed.
    /// </summary>
    public float MovementSpeed = 1f;

    /// <summary>
    /// The time it takes for the arrow to pass the entire path with certain movement speed.
    /// </summary>
    public float PassageTime = 0f;

    private int nextPointIndex;

    private Vector3 nextPoint;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        myTransform = GetComponent<Transform>();
    }

    private void FixedUpdate()
    {
        if (isMoving)
        {
            Move();
        }
    }

    /// <summary>
    /// Starts the arrow.
    /// </summary>
    public void StartMoving()
    {
        GetReady();
        isMoving = true;
    }

    /// <summary>
    /// Sets up the arrow to the start of the curve.
    /// </summary>
    public void GetReady()
    {
        myTransform.position = lineRenderer.GetPosition(0);
        nextPointIndex = 0;
        ResetNextPoint();
        RotateTo(nextPointIndex);
        CalculatePassageTime();

        if (isMoving)
        {
            isMoving = false;
        }
    }

    /// <summary>
    /// Turns the arrow to next point of curve.
    /// </summary>
    /// <param name="pointIndex"></param>
    private void RotateTo(int pointIndex) 
    {
        Vector3 direction = nextPoint - myTransform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    /// <summary>
    /// Sets next point of curve.
    /// </summary>
    private void ResetNextPoint()
    {
        nextPointIndex++;
        // If present point was not the ending -> setting next point.
        if (nextPointIndex < lineRenderer.positionCount)
        {
            nextPoint = lineRenderer.GetPosition(nextPointIndex);
            RotateTo(nextPointIndex);
        }
        else
        {
            // If present point is ending and curve is looped -> setting first point of curve as next point.
            if (nextPointIndex == lineRenderer.positionCount && lineRenderer.loop)
            {
                nextPoint = lineRenderer.GetPosition(0);
                RotateTo(0);
            }
            // Else present point is the end of curve -> movement stops.
            else
            {
                isMoving = false;
            }
        }
    }

    private void Move()
    {
        float movingStep = Time.deltaTime * MovementSpeed;
        float stepToNextPoint = (nextPoint - myTransform.position).magnitude;

        // While arrow jums over the next point of curve -> move the arrow to next point.
        // Also reduce movingStep by stepToNextPoint.
        // This is necessary so that the arrow does not go off the trajectory on high speed. 
        while (stepToNextPoint < movingStep && isMoving)
        {
            myTransform.position = nextPoint;
            ResetNextPoint();
            movingStep -= stepToNextPoint;
            stepToNextPoint = (nextPoint - myTransform.position).magnitude;
        }

        // If the arrow is not stopped -> simply moving the arrow.
        if (isMoving)
        {
            myTransform.position += myTransform.right * movingStep;
        }
    }

    /// <summary>
    /// Calculates the passage time with present movement speed.
    /// </summary>
    public void CalculatePassageTime()
    {
        float pTime = 0f;
        // Calculate the sum of each curve segment.
        for (int pointIndex = 0; pointIndex < lineRenderer.positionCount - 1; pointIndex++)
        {
            pTime += (lineRenderer.GetPosition(pointIndex + 1) 
                - lineRenderer.GetPosition(pointIndex))
                .magnitude; 
        }

        PassageTime = pTime / MovementSpeed;

        UIManager.Instance.PassageTimeHasCHanged();
    }
}
