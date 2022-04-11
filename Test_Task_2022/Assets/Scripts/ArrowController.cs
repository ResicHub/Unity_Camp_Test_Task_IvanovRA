using UnityEngine;

/// <summary>
/// Controller with movement logic for arrow.
/// </summary>
public class ArrowController : MonoBehaviour
{
    /// <summary>
    /// An object instance implemented with a singleton.
    /// </summary>
    public static ArrowController Instance = null;

    private Transform myTransform;

    /// <summary>
    /// Renderer of path line.
    /// </summary>
    [SerializeField]
    public LineRenderer MyLineRenderer;

    private bool isMoving = false;

    [SerializeField]
    [Range(1f, 10f)]
    private float movingSpeed = 1f;

    [SerializeField]
    private float passageTime = 0f;

    private int nextPointIndex;

    private Vector3 nextPoint;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        myTransform = GetComponent<Transform>();
        GetReady();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isMoving)
        {
            GetReady();
            isMoving = true;
        }
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
        myTransform.position = MyLineRenderer.GetPosition(0);
        nextPointIndex = 0;
        ResetNextPoint();
        RotateTo(nextPointIndex);
        CalculatePassageTime();

        if (isMoving)
        {
            isMoving = false;
        }
    }

    private void RotateTo(int pointIndex)
    {
        Vector3 direction = nextPoint - myTransform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void ResetNextPoint()
    {
        nextPointIndex++;
        if (nextPointIndex < MyLineRenderer.positionCount)
        {
            nextPoint = MyLineRenderer.GetPosition(nextPointIndex);
            RotateTo(nextPointIndex);
        }
        else
        {
            if (nextPointIndex == MyLineRenderer.positionCount && MyLineRenderer.loop)
            {
                nextPoint = MyLineRenderer.GetPosition(0);
                RotateTo(0);
            }
            else
            {
                isMoving = false;
            }
        }
    }

    private void Move()
    {
        Vector3 movingStep = myTransform.right * Time.deltaTime * movingSpeed;
        Vector3 stepToNextPoint = nextPoint - myTransform.position;
        if (stepToNextPoint.magnitude > movingStep.magnitude)
        {
            myTransform.position += movingStep;
        }
        else
        {
            myTransform.position += stepToNextPoint;
            ResetNextPoint();
            if (isMoving)
            {
                myTransform.position += myTransform.right * (movingStep - stepToNextPoint).magnitude;
            }
        }
    }

    private void CalculatePassageTime()
    {
        float pTime = 0f;
        for (int i = 0; i < MyLineRenderer.positionCount - 1; i++)
        {
            pTime += (MyLineRenderer.GetPosition(i + 1) 
                - MyLineRenderer.GetPosition(i)).magnitude; 
        }

        if (MyLineRenderer.loop)
        {
            pTime += (MyLineRenderer.GetPosition(MyLineRenderer.positionCount-1) 
                - MyLineRenderer.GetPosition(0)).magnitude;
        }

        passageTime = pTime / movingSpeed;
    }
}
