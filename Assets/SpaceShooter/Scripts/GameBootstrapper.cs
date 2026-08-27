using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// THE ONE SCRIPT you add to the scene.
/// On Awake it builds the entire space-shooter game:
/// camera, starfield, player, spawner, UI, and GameManager.
///
/// Setup:
///   1. Open/create a scene with only the Main Camera.
///   2. Create an empty GameObject, name it "Game".
///   3. Attach this component.
///   4. Press Play.
/// </summary>
public class GameBootstrapper : MonoBehaviour
{
    private void Awake()
    {
        // 1. Camera
        SetupCamera();

        // 2. Starfield background
        CreateStarfield();

        // 3. Player
        PlayerShip player = CreatePlayer();

        // 4. Enemy spawner
        EnemySpawner spawner = CreateSpawner();

        // 5. Game Manager + UI
        CreateManagerAndUI(player, spawner);
    }

    // ═════════════════════════════════════════════════════════════
    //  Camera
    // ═════════════════════════════════════════════════════════════

    private void SetupCamera()
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            GameObject camGo = new GameObject("Main Camera");
            camGo.tag = "MainCamera";
            cam = camGo.AddComponent<Camera>();
        }

        cam.orthographic      = true;
        cam.orthographicSize  = 8f;
        cam.clearFlags        = CameraClearFlags.SolidColor;
        cam.backgroundColor   = new Color(0.02f, 0.02f, 0.06f);
        cam.transform.position = new Vector3(0f, 0f, -10f);
    }

    // ═════════════════════════════════════════════════════════════
    //  Starfield
    // ═════════════════════════════════════════════════════════════

    private void CreateStarfield()
    {
        GameObject go = new GameObject("Starfield");
        go.AddComponent<Starfield>();
    }

    // ═════════════════════════════════════════════════════════════
    //  Player
    // ═════════════════════════════════════════════════════════════

    private PlayerShip CreatePlayer()
    {
        GameObject go = new GameObject("Player");

        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteFactory.Player;
        sr.sortingOrder = 5;

        Rigidbody2D rb = go.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.useFullKinematicContacts = true;

        CircleCollider2D col = go.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius = 0.35f;

        PlayerShip ship = go.AddComponent<PlayerShip>();

        // Start hidden until GameManager activates it
        Camera cam = Camera.main;
        go.transform.position = new Vector3(0f, -cam.orthographicSize + 1.5f, 0f);

        return ship;
    }

    // ═════════════════════════════════════════════════════════════
    //  Enemy Spawner
    // ═════════════════════════════════════════════════════════════

    private EnemySpawner CreateSpawner()
    {
        GameObject go = new GameObject("EnemySpawner");
        return go.AddComponent<EnemySpawner>();
    }

    // ═════════════════════════════════════════════════════════════
    //  Game Manager + UI
    // ═════════════════════════════════════════════════════════════

    private void CreateManagerAndUI(PlayerShip player, EnemySpawner spawner)
    {
        // ── Manager ──────────────────────────────────────────────
        GameObject mgrGo = new GameObject("GameManager");
        GameManager mgr  = mgrGo.AddComponent<GameManager>();
        mgr.player  = player;
        mgr.spawner = spawner;

        // ── Canvas ───────────────────────────────────────────────
        GameObject canvasGo = new GameObject("UICanvas");
        Canvas canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode  = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        CanvasScaler scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight  = 0.5f;

        canvasGo.AddComponent<GraphicRaycaster>();

        Font font = GetFont();

        // ── TITLE PANEL ──────────────────────────────────────────
        GameObject titlePanel = CreateFullPanel(canvasGo.transform, "TitlePanel",
            new Color(0.02f, 0.02f, 0.08f, 0.92f));

        CreateText(titlePanel.transform, "TitleText",
            "STELLAR\nBLITZ", font, 100, new Color(0.7f, 0.85f, 1f),
            TextAnchor.MiddleCenter,
            new Vector2(0.1f, 0.50f), new Vector2(0.9f, 0.78f));

        CreateText(titlePanel.transform, "SubtitleText",
            "SPACE SHOOTER", font, 32, new Color(0.5f, 0.6f, 0.8f, 0.6f),
            TextAnchor.UpperCenter,
            new Vector2(0.1f, 0.44f), new Vector2(0.9f, 0.52f));

        CreateText(titlePanel.transform, "StartPrompt",
            "PRESS  SPACE  TO  START", font, 36, new Color(0.6f, 0.8f, 1f, 0.8f),
            TextAnchor.MiddleCenter,
            new Vector2(0.1f, 0.30f), new Vector2(0.9f, 0.40f));

        CreateText(titlePanel.transform, "ControlsHint",
            "\u2190 \u2192   MOVE        SPACE   FIRE", font, 24,
            new Color(0.45f, 0.55f, 0.7f, 0.5f),
            TextAnchor.MiddleCenter,
            new Vector2(0.05f, 0.18f), new Vector2(0.95f, 0.28f));

        // ── HUD PANEL ────────────────────────────────────────────
        GameObject hudPanel = CreatePanel(canvasGo.transform, "HUDPanel");
        hudPanel.SetActive(false);

        Text scoreText = CreateText(hudPanel.transform, "ScoreText",
            "SCORE: 0", font, 36, new Color(1f, 0.85f, 0.3f),
            TextAnchor.UpperLeft,
            new Vector2(0.03f, 0.93f), new Vector2(0.45f, 0.99f));

        Text waveText = CreateText(hudPanel.transform, "WaveText",
            "WAVE 1", font, 30, new Color(0.7f, 0.8f, 1f, 0.7f),
            TextAnchor.UpperCenter,
            new Vector2(0.3f, 0.93f), new Vector2(0.7f, 0.99f));

        Text livesText = CreateText(hudPanel.transform, "LivesText",
            "LIVES: 3", font, 36, new Color(0.4f, 0.9f, 0.5f),
            TextAnchor.UpperRight,
            new Vector2(0.55f, 0.93f), new Vector2(0.97f, 0.99f));

        // ── GAME OVER PANEL ──────────────────────────────────────
        GameObject gameOverPanel = CreateFullPanel(canvasGo.transform, "GameOverPanel",
            new Color(0.05f, 0.02f, 0.02f, 0.92f));
        gameOverPanel.SetActive(false);

        CreateText(gameOverPanel.transform, "GameOverTitle",
            "MISSION\nFAILED", font, 90, new Color(1f, 0.35f, 0.35f),
            TextAnchor.MiddleCenter,
            new Vector2(0.1f, 0.55f), new Vector2(0.9f, 0.78f));

        Text finalScore = CreateText(gameOverPanel.transform, "FinalScore",
            "SCORE: 0", font, 48, new Color(1f, 0.85f, 0.3f),
            TextAnchor.MiddleCenter,
            new Vector2(0.1f, 0.42f), new Vector2(0.9f, 0.53f));

        Text finalWave = CreateText(gameOverPanel.transform, "FinalWave",
            "WAVE: 1", font, 32, new Color(0.6f, 0.7f, 0.85f, 0.6f),
            TextAnchor.MiddleCenter,
            new Vector2(0.1f, 0.36f), new Vector2(0.9f, 0.43f));

        CreateText(gameOverPanel.transform, "RestartPrompt",
            "PRESS  SPACE  TO  RETRY", font, 36, new Color(0.6f, 0.8f, 1f, 0.8f),
            TextAnchor.MiddleCenter,
            new Vector2(0.1f, 0.22f), new Vector2(0.9f, 0.32f));

        // ── Wire references ──────────────────────────────────────
        mgr.titlePanel    = titlePanel;
        mgr.hudPanel      = hudPanel;
        mgr.gameOverPanel = gameOverPanel;
        mgr.scoreText     = scoreText;
        mgr.waveText      = waveText;
        mgr.livesText     = livesText;
        mgr.finalScoreText = finalScore;
        mgr.finalWaveText  = finalWave;
    }

    // ═════════════════════════════════════════════════════════════
    //  UI Helpers
    // ═════════════════════════════════════════════════════════════

    /// <summary>Full-screen panel with a background image (overlay).</summary>
    private GameObject CreateFullPanel(Transform parent, string name, Color bgColor)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);

        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        Image img = go.AddComponent<Image>();
        img.color = bgColor;

        return go;
    }

    /// <summary>Full-screen transparent panel (no background).</summary>
    private GameObject CreatePanel(Transform parent, string name)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);

        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        return go;
    }

    /// <summary>Create a UI Text element with anchor-based positioning.</summary>
    private Text CreateText(Transform parent, string name,
        string content, Font font, int fontSize, Color color,
        TextAnchor alignment,
        Vector2 anchorMin, Vector2 anchorMax)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);

        RectTransform rt = go.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        Text text  = go.AddComponent<Text>();
        text.text      = content;
        text.font      = font;
        text.fontSize  = fontSize;
        text.color     = color;
        text.alignment = alignment;
        text.horizontalOverflow = HorizontalWrapMode.Overflow;
        text.verticalOverflow   = VerticalWrapMode.Overflow;

        // Optional: add shadow for readability
        Shadow shadow = go.AddComponent<Shadow>();
        shadow.effectColor    = new Color(0, 0, 0, 0.6f);
        shadow.effectDistance = new Vector2(2, -2);

        return text;
    }

    /// <summary>Get a built-in font that works across Unity versions.</summary>
    private Font GetFont()
    {
        // Unity 6 uses LegacyRuntime.ttf; older versions use Arial.ttf
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (font == null)
            font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        return font;
    }
}
