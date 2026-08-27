using UnityEngine;

/// <summary>
/// Spawns enemy ships in waves. Each wave has more (and faster) enemies.
/// Reports wave completion to GameManager.
/// </summary>
public class EnemySpawner : MonoBehaviour
{
    private int currentWave;
    private int enemiesToSpawn;   // how many still need to appear
    private int enemiesAlive;     // how many are alive on screen
    private float spawnInterval;
    private float spawnTimer;
    private bool isSpawning;

    /// <summary>Begin a new wave of enemies.</summary>
    public void BeginWave(int wave)
    {
        currentWave   = wave;
        enemiesToSpawn = 4 + 2 * wave;             // wave 1 → 6, wave 2 → 8, …
        enemiesAlive   = enemiesToSpawn;
        spawnInterval  = Mathf.Max(0.35f, 1.1f - wave * 0.08f);
        spawnTimer     = 0.6f; // brief initial delay
        isSpawning     = true;
    }

    /// <summary>Stop all spawning (game over / restart).</summary>
    public void StopSpawning()
    {
        isSpawning     = false;
        enemiesToSpawn = 0;
    }

    /// <summary>Called by EnemyShip when it is destroyed or leaves screen.</summary>
    public void OnEnemyDestroyed()
    {
        enemiesAlive--;

        // If all enemies from this wave are gone, advance
        if (enemiesAlive <= 0 && enemiesToSpawn <= 0 && isSpawning)
        {
            isSpawning = false;
            if (GameManager.Instance != null)
                GameManager.Instance.OnWaveCleared();
        }
    }

    private void Update()
    {
        if (!isSpawning || enemiesToSpawn <= 0) return;

        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            SpawnEnemy();
            enemiesToSpawn--;
            spawnTimer = spawnInterval;
        }
    }

    private void SpawnEnemy()
    {
        Camera cam = Camera.main;
        float halfW = cam.orthographicSize * cam.aspect;

        float x = Random.Range(-halfW + 1f, halfW - 1f);
        float y = cam.orthographicSize + 1.5f;

        GameObject go = new GameObject("Enemy");
        go.transform.position = new Vector3(x, y, 0f);

        // Sprite
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteFactory.Enemy;
        sr.sortingOrder = 2;

        // Physics
        Rigidbody2D rb = go.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.useFullKinematicContacts = true;

        CircleCollider2D col = go.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = 0.4f;

        // Enemy behaviour
        EnemyShip enemy = go.AddComponent<EnemyShip>();
        enemy.speed      = 1.2f + currentWave * 0.25f;
        enemy.health     = currentWave >= 5 ? 3 : currentWave >= 3 ? 2 : 1;
        enemy.scoreValue = 100 * enemy.health;
        enemy.spawner    = this;
    }
}
