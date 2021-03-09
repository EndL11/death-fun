using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform target;
    public float dampTime = 0.75f;

    void Start()
    {
        
    }

    void FixedUpdate()
    {
        Vector3 velocity = Vector3.zero;
        Vector3 point = Camera.main.WorldToViewportPoint(target.position);
        Vector3 delta = target.position - Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z));
        Vector3 destination = transform.position + delta;
        Vector3 pos = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
        pos.y = transform.position.y;
        transform.position = pos;
    }
}
