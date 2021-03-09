using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform target;
    public float dampTime = 0.75f;
    public Transform leftBorder;
    public Transform rightBorder;

    void FixedUpdate()
    {
        Vector3 pos = GetNextCameraPosition();
        if (target == null || IsOnCameraBorder(pos))
            return;

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
        Vector3 velocity = Vector3.zero;
        Vector3 point = Camera.main.WorldToViewportPoint(target.position);
        Vector3 delta = target.position - Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z));
        Vector3 destination = transform.position + delta;
        return Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
    }
}
