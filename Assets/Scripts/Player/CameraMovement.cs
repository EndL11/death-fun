using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private Transform _target;
    public float dampTime = 0.75f;
    public Transform leftBorder;
    public Transform rightBorder;
    private Vector3 _nextPos;

    private void Start()
    {
        //  find player on start 
        _target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void FixedUpdate()
    {
        _nextPos = GetNextCameraPosition();
        if (_target == null || IsOnCameraBorder())
            return;
        //  set height position of start (dont change y coordinate)
        _nextPos.y = transform.position.y;
        transform.position = _nextPos;
    }

    private bool IsOnCameraBorder()
    {
        return _nextPos.x < transform.position.x && leftBorder.position.x > Camera.main.ViewportToWorldPoint(Vector2.zero).x 
            || _nextPos.x > transform.position.x && rightBorder.position.x < Camera.main.ViewportToWorldPoint(Vector2.right).x;     
    }

    private Vector3 GetNextCameraPosition()
    {
        if(_target == null)
            return Vector3.zero;

        //  calculating and returning next smooth pos for camera
        Vector3 velocity = Vector3.zero;
        Vector3 point = Camera.main.WorldToViewportPoint(_target.position);
        Vector3 delta = _target.position - Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z));
        Vector3 destination = transform.position + delta;
        return Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
    }
}
