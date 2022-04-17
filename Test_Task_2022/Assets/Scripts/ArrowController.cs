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

    /// <summary>
    /// Renderer of curve.
    /// </summary>
    [SerializeField]
    private LineRenderer lineRenderer;

    private bool isMoving = false;

    /// <summary>
    /// Arrow's movement speed.
    /// </summary>
    [SerializeField]
    [Range(1f, 20f)]
    public float MovementSpeed = 1f;

    // That parameter helps updating PassageTime in real time when movement speed has changed.
    private float oldMovementSpeed;

    /// <summary>
    /// The time it takes for the arrow to pass the entire path with certain movement speed.
    /// </summary>
    [SerializeField]
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
        oldMovementSpeed = MovementSpeed;
    }

    private void Update()
    {
        // If 'Space' button was pressed and arrow is not moving now -> arrow starts moving.
        if (Input.GetKeyDown(KeyCode.S) && !isMoving)
        {
            GetReady();
            isMoving = true;
        }
        // If 'R' button was pressed -> arrow is setting up to start of curve.
        else if (Input.GetKeyDown(KeyCode.R))
        {
            GetReady();
        }

        // If movement speed has changed -> calculating new PassageTime.
        if (oldMovementSpeed != MovementSpeed)
        {
            oldMovementSpeed = MovementSpeed;
            CalculatePassageTime();
        }
    }

    private void FixedUpdate()
    {
        if (isMoving)
        {
            Move();
        }
    }

    /// <summary>
    /// A method for setting the arrow to the start of the path.
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

    /// <summary>
    /// Main arrow's method with moving logic.
    /// </summary>
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
    private void CalculatePassageTime()
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
    }
}
