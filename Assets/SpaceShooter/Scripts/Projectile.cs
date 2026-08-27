using UnityEngine;

/// <summary>
/// Projectile (bullet) that travels in a direction and handles collision.
/// Created dynamically by PlayerShip or EnemyShip.
/// </summary>
public class Projectile : MonoBehaviour
{
    [HideInInspector] public float speed = 12f;
    [HideInInspector] public Vector2 direction = Vector2.up;
    [HideInInspector] public bool isPlayerBullet = true;

    private float screenLimit;

    private void Start()
    {
        // Cache screen bounds for off-screen check
        Camera cam = Camera.main;
        if (cam != null)
            screenLimit = cam.orthographicSize + 2f;
        else
            screenLimit = 12f;
    }

    private void Update()
    {
        // Move in assigned direction
        transform.Translate(direction * (speed * Time.deltaTime), Space.World);

        // Destroy when off-screen (above or below)
        if (Mathf.Abs(transform.position.y) > screenLimit)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isPlayerBullet)
        {
            // Player bullets damage enemies
            EnemyShip enemy = other.GetComponent<EnemyShip>();
            if (enemy != null)
            {
                enemy.TakeDamage(1);
                Destroy(gameObject);
            }
        }
        else
        {
            // Enemy bullets damage the player
            PlayerShip player = other.GetComponent<PlayerShip>();
            if (player != null)
            {
                player.TakeHit();
                Destroy(gameObject);
            }
        }
    }
}
