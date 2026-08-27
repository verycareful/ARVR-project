using UnityEngine;

/// <summary>
/// Enemy ship that drifts downward. Takes damage from player bullets,
/// awards score on death, and costs the player a life if it reaches the bottom.
/// </summary>
public class EnemyShip : MonoBehaviour
{
    [HideInInspector] public float speed = 1.5f;
    [HideInInspector] public int health = 1;
    [HideInInspector] public int scoreValue = 100;
    [HideInInspector] public EnemySpawner spawner;

    private SpriteRenderer sr;
    private bool isDead;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (isDead) return;

        // Drift downward
        transform.Translate(Vector2.down * (speed * Time.deltaTime), Space.World);

        // If past the bottom of the screen → player loses a life
        Camera cam = Camera.main;
        if (cam != null && transform.position.y < -cam.orthographicSize - 1.5f)
        {
            // Notify manager (costs a life)
            if (GameManager.Instance != null &&
                GameManager.Instance.CurrentState == GameManager.GameState.Playing)
            {
                GameManager.Instance.LoseLife();
            }
            DestroyQuietly();
        }
    }

    // ── Collision ────────────────────────────────────────────────

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead) return;

        // Colliding directly with the player ship
        PlayerShip player = other.GetComponent<PlayerShip>();
        if (player != null)
        {
            player.TakeHit();
            Die();
        }
    }

    // ── Damage ───────────────────────────────────────────────────

    /// <summary>Called by Projectile when hit by a player bullet.</summary>
    public void TakeDamage(int amount)
    {
        if (isDead) return;

        health -= amount;

        // Flash white briefly
        if (sr != null) sr.color = Color.white;
        Invoke(nameof(ResetColor), 0.05f);

        if (health <= 0)
        {
            Die();
        }
    }

    private void ResetColor()
    {
        if (sr != null) sr.color = Color.white;
    }

    // ── Death ────────────────────────────────────────────────────

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        // Award score
        if (GameManager.Instance != null)
            GameManager.Instance.AddScore(scoreValue);

        // Spawn explosion
        SpawnExplosion();

        // Notify spawner for wave tracking
        if (spawner != null)
            spawner.OnEnemyDestroyed();

        Destroy(gameObject);
    }

    /// <summary>Destroyed without awarding score (e.g. cleanup on restart).</summary>
    private void DestroyQuietly()
    {
        if (isDead) return;
        isDead = true;

        if (spawner != null)
            spawner.OnEnemyDestroyed();

        Destroy(gameObject);
    }

    private void SpawnExplosion()
    {
        GameObject fxGo = new GameObject("Explosion");
        fxGo.transform.position = transform.position;
        ExplosionEffect fx = fxGo.AddComponent<ExplosionEffect>();
        fx.Initialize(new Color(1f, 0.5f, 0.15f)); // orange explosion
    }
}
