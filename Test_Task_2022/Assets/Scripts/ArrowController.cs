using UnityEngine;

/// <summary>
/// Controller with movement logic for arrow.
/// </summary>
public class ArrowController : MonoBehaviour
{
    private Transform myTransform;

    [SerializeField]
    private LineRenderer lineRenderer;

    private bool isMoving = false;

    private int nextPointIndex;

    private Vector3 nextPoint;

    private void Awake()
    {
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

    private void GetReady()
    {
        myTransform.position = lineRenderer.GetPosition(0);
        nextPointIndex = 0;
        ResetNextPoint();
        RotateToNextPoint();
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
