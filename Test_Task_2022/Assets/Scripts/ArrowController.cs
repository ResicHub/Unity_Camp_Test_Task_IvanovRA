using UnityEngine;

/// <summary>
/// Controller with movement logic for arrow.
/// </summary>
public class ArrowController : MonoBehaviour
{
    [SerializeField]
    private LineRenderer lineRenderer;

    private Transform myTransform;

    private void Awake()
    {
        myTransform = GetComponent<Transform>();
        myTransform.position = lineRenderer.GetPosition(0);
        RotateTo(lineRenderer.GetPosition(1));
    }

    private void RotateTo(Vector3 target)
    {
        Vector3 direction = target - myTransform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}
