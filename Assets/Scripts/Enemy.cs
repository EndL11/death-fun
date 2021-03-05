using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public float hp = 100f;
    private float speed = 3f;
    private Rigidbody2D rb;
    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ApplyDamage(float damage)
    {
        hp -= damage;
        if (hp <= 0)
            DestroyEnemy();
        PushBack();
    }

    private void PushBack()
    {
        //rb.velocity = Vector2.zero;
        //rb.AddForce(new Vector2(transform.position.x + 0.1f, transform.position.y + 5f) * .4f, ForceMode2D.Impulse);

        StartCoroutine(PushBackAnimation());

        StartCoroutine(HurtAnimation());
    }

    private IEnumerator PushBackAnimation()
    {
        Vector2 pos = transform.position;
        float y = transform.position.y;
        Vector2 target = new Vector2(pos.x + .3f, y + 0.3f);
        while (Vector2.Distance(target, transform.position) >= 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, target, Time.deltaTime * 5f);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        target = new Vector2(transform.position.x, transform.position.y - .3f);
        while (Vector2.Distance(target, transform.position) >= 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, target, Time.deltaTime * 5f);
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    private IEnumerator HurtAnimation()
    {
        Color c = GetComponentInChildren<SpriteRenderer>().material.color;
        GetComponentInChildren<SpriteRenderer>().material.color = new Color(255, 0, 0, .3f);
        yield return new WaitForSeconds(0.2f);
        GetComponentInChildren<SpriteRenderer>().material.color = c;
    }

    private void DestroyEnemy()
    {
        anim.SetTrigger("Die");
    }
}
