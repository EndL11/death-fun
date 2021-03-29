using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private Transform target;
    public float dampTime = 0.75f;
    public Transform leftBorder;
    public Transform rightBorder;

    private void Start()
    {
        //  find player on start 
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void FixedUpdate()
    {
        Vector3 pos = GetNextCameraPosition();
        if (target == null || IsOnCameraBorder(pos))
            return;
        //  set height position of start (dont change y coordinate)
        pos.y = transform.position.y;
        transform.position = pos;
    }

    private bool IsOnCameraBorder(Vector3 nextPos)
    {
        return nextPos.x < transform.position.x && leftBorder.position.x > Camera.main.ViewportToWorldPoint(Vector2.zero).x 
            || nextPos.x > transform.position.x && rightBorder.position.x < Camera.main.ViewportToWorldPoint(Vector2.right).x;     
    }

    private Vector3 GetNextCameraPosition()
    {
        if(target == null)
            return Vector3.zero;

        //  calculating and returning next smooth pos for camera
        Vector3 velocity = Vector3.zero;
        Vector3 point = Camera.main.WorldToViewportPoint(target.position);
        Vector3 delta = target.position - Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z));
        Vector3 destination = transform.position + delta;
        return Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
    }
}
