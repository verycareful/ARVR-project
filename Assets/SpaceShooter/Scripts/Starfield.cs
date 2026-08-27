using UnityEngine;

/// <summary>
/// Creates a scrolling star background with multiple parallax layers.
/// Stars scroll downward to give the feeling of flying through space.
/// </summary>
public class Starfield : MonoBehaviour
{
    private const int StarCount = 80;

    private Transform[] stars;
    private float[] speeds;
    private float halfH;
    private float halfW;

    private void Start()
    {
        Camera cam = Camera.main;
        halfH = cam.orthographicSize;
        halfW = halfH * cam.aspect;

        stars  = new Transform[StarCount];
        speeds = new float[StarCount];

        Sprite starSprite = SpriteFactory.Star;

        for (int i = 0; i < StarCount; i++)
        {
            GameObject go = new GameObject("Star");
            go.transform.SetParent(transform, false);
            go.transform.position = new Vector3(
                Random.Range(-halfW, halfW),
                Random.Range(-halfH, halfH),
                0f);

            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = starSprite;
            sr.sortingOrder = -10; // behind everything

            // Randomise brightness, size, and speed for depth illusion
            float brightness = Random.Range(0.15f, 0.75f);
            sr.color = new Color(
                brightness,
                brightness,
                brightness + Random.Range(0f, 0.15f), // slight blue tint
                brightness);

            float scale = Random.Range(0.2f, 0.8f);
            go.transform.localScale = Vector3.one * scale;

            stars[i]  = go.transform;
            speeds[i] = Random.Range(0.4f, 2.5f);
        }
    }

    private void Update()
    {
        for (int i = 0; i < StarCount; i++)
        {
            Vector3 pos = stars[i].position;
            pos.y -= speeds[i] * Time.deltaTime;

            // Wrap around when a star exits the bottom
            if (pos.y < -halfH - 0.5f)
            {
                pos.y = halfH + 0.5f;
                pos.x = Random.Range(-halfW, halfW);
            }

            stars[i].position = pos;
        }
    }
}
