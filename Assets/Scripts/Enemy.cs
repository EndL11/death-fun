using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public float hp = 100f;
    private float speed = 3f;
    private Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ApplyDamage(float damage)
    {
        hp -= damage;
        if (hp <= 0)
            Destroy(gameObject);
        PushBack();
    }

    private void PushBack()
    {
        rb.velocity = Vector2.zero;
        rb.AddForce(new Vector2(transform.position.x + 0.1f, transform.position.y + 5f) * .4f, ForceMode2D.Impulse);
    }
}
