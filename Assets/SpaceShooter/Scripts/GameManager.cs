using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/// <summary>
/// Singleton managing overall game state: score, lives, waves, and UI.
/// Created and wired by GameBootstrapper.
/// </summary>
public class GameManager : MonoBehaviour
{
    // ── Singleton ────────────────────────────────────────────────
    public static GameManager Instance { get; private set; }

    // ── Game state ───────────────────────────────────────────────
    public enum GameState { Title, Playing, GameOver }
    public GameState CurrentState { get; private set; } = GameState.Title;

    public int Score { get; private set; }
    public int Lives { get; private set; }
    public int Wave  { get; private set; }

    // ── UI references (assigned by GameBootstrapper) ─────────────
    [HideInInspector] public Text scoreText;
    [HideInInspector] public Text waveText;
    [HideInInspector] public Text livesText;
    [HideInInspector] public Text finalScoreText;
    [HideInInspector] public Text finalWaveText;
    [HideInInspector] public GameObject titlePanel;
    [HideInInspector] public GameObject hudPanel;
    [HideInInspector] public GameObject gameOverPanel;

    // ── Game-object references (assigned by GameBootstrapper) ────
    [HideInInspector] public PlayerShip player;
    [HideInInspector] public EnemySpawner spawner;

    // ═════════════════════════════════════════════════════════════

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        // Only listen for input on non-playing screens
        if (CurrentState == GameState.Playing) return;

        Keyboard kb = Keyboard.current;
        if (kb != null && kb.spaceKey.wasPressedThisFrame)
        {
            StartGame();
        }
    }

    // ── Public API ───────────────────────────────────────────────

    public void StartGame()
    {
        CleanupEntities();

        Score = 0;
        Lives = 3;
        Wave  = 1;
        CurrentState = GameState.Playing;

        titlePanel.SetActive(false);
        gameOverPanel.SetActive(false);
        hudPanel.SetActive(true);
        RefreshHUD();

        player.Activate();
        spawner.BeginWave(Wave);
    }

    public void AddScore(int points)
    {
        Score += points;
        RefreshHUD();
    }

    public void LoseLife()
    {
        Lives--;
        RefreshHUD();

        if (Lives <= 0)
        {
            TriggerGameOver();
        }
    }

    /// <summary>
    /// Called by EnemySpawner when all enemies in the current wave are destroyed.
    /// </summary>
    public void OnWaveCleared()
    {
        Wave++;
        RefreshHUD();
        spawner.BeginWave(Wave);
    }

    // ── Internals ────────────────────────────────────────────────

    private void TriggerGameOver()
    {
        CurrentState = GameState.GameOver;

        player.Deactivate();
        spawner.StopSpawning();
        CleanupEntities();

        finalScoreText.text = "SCORE: " + Score;
        finalWaveText.text  = "WAVE: " + Wave;
        hudPanel.SetActive(false);
        gameOverPanel.SetActive(true);
    }

    private void RefreshHUD()
    {
        if (scoreText != null) scoreText.text = "SCORE: " + Score;
        if (waveText != null)  waveText.text  = "WAVE " + Wave;
        if (livesText != null) livesText.text = "LIVES: " + Lives;
    }

    private void CleanupEntities()
    {
        // Destroy all active enemies
        foreach (EnemyShip e in FindObjectsByType<EnemyShip>(FindObjectsSortMode.None))
            Destroy(e.gameObject);

        // Destroy all active projectiles
        foreach (Projectile p in FindObjectsByType<Projectile>(FindObjectsSortMode.None))
            Destroy(p.gameObject);

        // Destroy any lingering explosions
        foreach (ExplosionEffect fx in FindObjectsByType<ExplosionEffect>(FindObjectsSortMode.None))
            Destroy(fx.gameObject);
    }
}
