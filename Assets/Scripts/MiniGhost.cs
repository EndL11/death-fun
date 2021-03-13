using UnityEngine;

public class MiniGhost : MonoBehaviour
{
    public float speed = 5f;
    void Start()
    {
        GetComponentInChildren<Animator>().SetBool("Run", true);
        Destroy(gameObject, 3f);
    }

    void Update()
    {
        transform.Translate(Vector2.right * 5f * Time.deltaTime);
    }
}
