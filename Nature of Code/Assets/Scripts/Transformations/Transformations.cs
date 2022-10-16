using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transformations : MonoBehaviour
{
    public Transform target;

    public Vector3 targetPos;

    public bool isLocalSpace;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            isLocalSpace = !isLocalSpace;

        if (isLocalSpace)
        {
            var worldOffset = transform.right * targetPos.x + transform.up * targetPos.y;
            target.position = transform.position + worldOffset;//transform.TransformPoint(targetPos);
        }
        else
        {
            target.position = targetPos;
        }
    }
}
