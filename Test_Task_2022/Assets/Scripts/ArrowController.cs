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

    [SerializeField]
    private LineRenderer lineRenderer;

    private bool isMoving = false;

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
        myTransform.position = lineRenderer.GetPosition(0);
        nextPointIndex = 0;
        ResetNextPoint();
        RotateToNextPoint();
        if (isMoving)
        {
            isMoving = false;
        }
    }

    private void RotateToNextPoint()
    {
        Vector3 direction = nextPoint - myTransform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void ResetNextPoint()
    {
        nextPointIndex++;
        if (nextPointIndex < lineRenderer.positionCount)
        {
            nextPoint = lineRenderer.GetPosition(nextPointIndex);
            RotateToNextPoint();
        }
        else
        {
            isMoving = false;
        }
    }

    private void Move()
    {
        if ((nextPoint - myTransform.position).magnitude > 0.01f)
        {
            myTransform.position = myTransform.position + myTransform.right * Time.deltaTime;
        }
        else
        {
            ResetNextPoint();
        }
    }
}
