using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controller including arrow movement method.
/// </summary>
public class ArrowController : MonoBehaviour
{
    [SerializeField]
    private LineRenderer lineRenderer;

    private Transform myTransform;

    private void Awake()
    {
        myTransform = GetComponent<Transform>();
    }
}
