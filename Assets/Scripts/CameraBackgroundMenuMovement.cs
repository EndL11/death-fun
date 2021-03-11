using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBackgroundMenuMovement : MonoBehaviour
{
    public float speed = 0.75f;
    public Transform leftBorder;
    public Transform rightBorder;

    void FixedUpdate()
    {
        Vector3 pos = new Vector3(transform.position.x + speed * Time.fixedDeltaTime, transform.position.y, -10f);
        if (IsOnCameraBorder(pos))
            speed = -speed;

        transform.position = pos;
    }

    private bool IsOnCameraBorder(Vector3 nextPos)
    {
        return nextPos.x < transform.position.x && leftBorder.position.x > GetComponent<Camera>().ViewportToWorldPoint(Vector2.zero).x
            || nextPos.x > transform.position.x && rightBorder.position.x < GetComponent<Camera>().ViewportToWorldPoint(Vector2.right).x;
    }
}

