using UnityEngine;

public class MiniGhost : MonoBehaviour
{
    public float speed = 5f;
    void Start()
    {
        //  player run animation
        GetComponentInChildren<Animator>().SetBool("Run", true);
        //  destroy self after 3 seconds
        Destroy(gameObject, 3f);
    }

    void Update()
    {
        //  move right (or left according to rotation) with speed 5
        transform.Translate(Vector2.right * 5f * Time.deltaTime);
    }
}
