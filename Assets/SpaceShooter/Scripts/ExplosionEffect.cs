using UnityEngine;

/// <summary>
/// Spawns expanding, fading particles to simulate an explosion.
/// Self-destructs after the animation completes.
/// </summary>
public class ExplosionEffect : MonoBehaviour
{
    private const int ParticleCount = 10;
    private const float Lifetime = 0.5f;

    private Transform[] particles;
    private SpriteRenderer[] renderers;
    private Vector2[] velocities;
    private float timer;

    /// <summary>
    /// Call immediately after instantiation to set up the effect.
    /// </summary>
    public void Initialize(Color baseColor)
    {
        particles = new Transform[ParticleCount];
        renderers = new SpriteRenderer[ParticleCount];
        velocities = new Vector2[ParticleCount];

        Sprite dot = SpriteFactory.Star; // reuse the small star sprite

        for (int i = 0; i < ParticleCount; i++)
        {
            GameObject go = new GameObject("Particle");
            go.transform.SetParent(transform, false);
            go.transform.position = transform.position;
            go.transform.localScale = Vector3.one * Random.Range(0.4f, 0.9f);

            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = dot;
            sr.sortingOrder = 10;

            // Slight colour variation per particle
            float hueShift = Random.Range(-0.08f, 0.08f);
            Color c = new Color(
                Mathf.Clamp01(baseColor.r + hueShift),
                Mathf.Clamp01(baseColor.g + hueShift * 0.5f),
                Mathf.Clamp01(baseColor.b + hueShift * 0.3f),
                1f);
            sr.color = c;

            // Random outward velocity
            float angle = Random.Range(0f, Mathf.PI * 2f);
            float speed = Random.Range(3f, 9f);
            velocities[i] = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * speed;

            particles[i] = go.transform;
            renderers[i] = sr;
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;
        float t = timer / Lifetime; // 0 → 1

        if (t >= 1f)
        {
            Destroy(gameObject);
            return;
        }

        for (int i = 0; i < ParticleCount; i++)
        {
            if (particles[i] == null) continue;

            // Move outward
            particles[i].position += (Vector3)(velocities[i] * Time.deltaTime);

            // Fade out
            Color c = renderers[i].color;
            c.a = 1f - t;
            renderers[i].color = c;

            // Shrink
            particles[i].localScale *= (1f - 1.5f * Time.deltaTime);
        }
    }
}
