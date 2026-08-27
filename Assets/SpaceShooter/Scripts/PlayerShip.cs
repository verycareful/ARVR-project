using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Player spaceship controller.
/// Moves left/right with arrow keys and fires bullets with Space.
/// </summary>
public class PlayerShip : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;

    [Header("Shooting")]
    public float fireRate = 0.15f; // seconds between shots

    private SpriteRenderer sr;
    private bool isActive;
    private float fireCooldown;
    private float invincibilityTimer;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    /// <summary>Reposition to bottom-center and enable controls.</summary>
    public void Activate()
    {
        isActive = true;
        invincibilityTimer = 0f;
        fireCooldown = 0f;

        Camera cam = Camera.main;
        float bottomY = -cam.orthographicSize + 1.5f;
        transform.position = new Vector3(0f, bottomY, 0f);

        if (sr != null) sr.color = Color.white;
        gameObject.SetActive(true);
    }

    /// <summary>Disable controls (game over).</summary>
    public void Deactivate()
    {
        isActive = false;
    }

    private void Update()
    {
        if (!isActive) return;

        Keyboard kb = Keyboard.current;
        if (kb == null) return;

        HandleMovement(kb);
        HandleShooting(kb);
        HandleInvincibility();
    }

    // ── Movement ─────────────────────────────────────────────────

    private void HandleMovement(Keyboard kb)
    {
        float h = 0f;
        if (kb.leftArrowKey.isPressed)  h -= 1f;
        if (kb.rightArrowKey.isPressed) h += 1f;

        if (h != 0f)
        {
            transform.Translate(Vector3.right * (h * moveSpeed * Time.deltaTime), Space.World);
        }

        // Clamp to screen edges
        Camera cam = Camera.main;
        float halfW = cam.orthographicSize * cam.aspect - 0.5f;
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, -halfW, halfW);
        transform.position = pos;
    }

    // ── Shooting ─────────────────────────────────────────────────

    private void HandleShooting(Keyboard kb)
    {
        fireCooldown -= Time.deltaTime;

        if (kb.spaceKey.isPressed && fireCooldown <= 0f)
        {
            FireBullet();
            fireCooldown = fireRate;
        }
    }

    private void FireBullet()
    {
        GameObject bullet = new GameObject("PlayerBullet");
        bullet.transform.position = transform.position + Vector3.up * 0.6f;

        SpriteRenderer bsr = bullet.AddComponent<SpriteRenderer>();
        bsr.sprite = SpriteFactory.PlayerBullet;
        bsr.sortingOrder = 3;

        Rigidbody2D rb = bullet.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.useFullKinematicContacts = true;

        BoxCollider2D col = bullet.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size = new Vector2(0.12f, 0.35f);

        Projectile proj = bullet.AddComponent<Projectile>();
        proj.speed = 14f;
        proj.direction = Vector2.up;
        proj.isPlayerBullet = true;
    }

    // ── Damage / Invincibility ───────────────────────────────────

    /// <summary>Called when the player is hit by an enemy or enemy bullet.</summary>
    public void TakeHit()
    {
        if (invincibilityTimer > 0f) return; // still invincible

        invincibilityTimer = 1.5f;
        GameManager.Instance.LoseLife();
    }

    private void HandleInvincibility()
    {
        if (invincibilityTimer <= 0f) return;

        invincibilityTimer -= Time.deltaTime;

        // Flash effect
        if (sr != null)
        {
            float alpha = Mathf.PingPong(Time.time * 10f, 1f);
            sr.color = new Color(1f, 1f, 1f, 0.3f + alpha * 0.7f);
        }

        if (invincibilityTimer <= 0f && sr != null)
        {
            sr.color = Color.white;
        }
    }
}
