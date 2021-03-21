using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngrySkull : MonoBehaviour
{
    public float spawnEnemyDelay = 10f;
    private float _spawnDelay;
    public List<GameObject> enemyPrefabs = new List<GameObject>();
    [SerializeField] private float pushForce = 10f;
    private bool dead = false;

    void Start()
    {
        _spawnDelay = spawnEnemyDelay;
    }

    void Update()
    {
        if (dead)
            return;

        if(_spawnDelay > 0f)
        {
            _spawnDelay -= Time.deltaTime;
        }
        else
        {
            SpawnEnemies();
            _spawnDelay = spawnEnemyDelay;
        }
    }

    private void SpawnEnemies()
    {
        Vector2 leftSpawnPoint = new Vector2(transform.position.x - 0.3f, transform.position.y);
        Vector2 rightSpawnPoint = new Vector2(transform.position.x + 0.3f, transform.position.y);

        GameObject leftEnemy = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Count)], leftSpawnPoint, Quaternion.identity);
        leftEnemy.GetComponent<Rigidbody2D>().AddForce(-transform.right * pushForce, ForceMode2D.Impulse);

        GameObject rightEnemy = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Count)], rightSpawnPoint, Quaternion.identity);
        rightEnemy.GetComponent<Rigidbody2D>().AddForce(transform.right * pushForce, ForceMode2D.Impulse);

        GameObject centerEnemy = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Count)], transform.position, Quaternion.identity);
        centerEnemy.GetComponent<Rigidbody2D>().AddForce(transform.up * pushForce, ForceMode2D.Impulse);

        leftEnemy = RandomDirection(leftEnemy);
        centerEnemy = RandomDirection(centerEnemy);
        rightEnemy = RandomDirection(rightEnemy);
    }

    private GameObject RandomDirection(GameObject enemy)
    {
        int rand = UnityEngine.Random.Range(1, 3);
        Enemy enemyScript = enemy.GetComponent<Enemy>();
        int direction = enemyScript.Direction;
        if (rand == 1)
            direction = -1;
        else
            direction = 1;

        enemyScript.Direction = direction;
        return enemy;
    }
}
