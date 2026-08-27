using UnityEngine;

/// <summary>
/// Generates all game sprites procedurally at runtime.
/// Sprites are cached after first creation — call the properties freely.
/// </summary>
public static class SpriteFactory
{
    // ── Cached sprites ───────────────────────────────────────────
    private static Sprite _player;
    private static Sprite _enemy;
    private static Sprite _playerBullet;
    private static Sprite _enemyBullet;
    private static Sprite _star;

    public static Sprite Player       => _player       ??= BuildPlayerSprite();
    public static Sprite Enemy        => _enemy        ??= BuildEnemySprite();
    public static Sprite PlayerBullet => _playerBullet ??= BuildBulletSprite(
        new Color(0.30f, 0.85f, 1.0f), new Color(0.70f, 0.95f, 1.0f));
    public static Sprite EnemyBullet  => _enemyBullet  ??= BuildBulletSprite(
        new Color(1.0f, 0.25f, 0.20f), new Color(1.0f, 0.60f, 0.30f));
    public static Sprite Star         => _star         ??= BuildStarSprite();

    // ── Player Ship (32×32, pointing UP) ─────────────────────────
    private static Sprite BuildPlayerSprite()
    {
        const int s = 32;
        Color[] px = ClearCanvas(s, s);

        Color hull     = new Color(0.25f, 0.65f, 0.95f);
        Color hullLt   = new Color(0.35f, 0.75f, 1.00f);
        Color wing     = new Color(0.15f, 0.40f, 0.70f);
        Color cockpit  = new Color(0.75f, 0.93f, 1.00f);
        Color cockpitW = new Color(0.90f, 0.97f, 1.00f);
        Color engine   = new Color(1.00f, 0.50f, 0.12f);
        Color glow     = new Color(1.00f, 0.80f, 0.30f);

        // Main hull (large triangle)
        FillTri(px, s, s, 16, 30, 5, 7, 27, 7, hull);
        // Inner hull detail (lighter)
        FillTri(px, s, s, 16, 28, 8, 9, 24, 9, hullLt);

        // Left wing
        FillTri(px, s, s, 5, 13, 0, 3, 5, 3, wing);
        // Right wing
        FillTri(px, s, s, 27, 13, 31, 3, 27, 3, wing);

        // Cockpit (layered circles for glow)
        FillCircle(px, s, s, 16, 22, 3, cockpit);
        FillCircle(px, s, s, 16, 22, 2, cockpitW);

        // Engine exhausts — left
        FillRect(px, s, s, 9, 0, 4, 6, engine);
        FillRect(px, s, s, 10, 0, 2, 4, glow);
        // Engine exhausts — right
        FillRect(px, s, s, 19, 0, 4, 6, engine);
        FillRect(px, s, s, 20, 0, 2, 4, glow);

        return BakeSprite(px, s, s, 32f);
    }

    // ── Enemy Ship (32×32, pointing DOWN) ────────────────────────
    private static Sprite BuildEnemySprite()
    {
        const int s = 32;
        Color[] px = ClearCanvas(s, s);

        Color hull   = new Color(0.85f, 0.15f, 0.15f);
        Color hullDk = new Color(0.65f, 0.10f, 0.10f);
        Color wing   = new Color(0.55f, 0.08f, 0.08f);
        Color eye    = new Color(1.00f, 0.90f, 0.20f);
        Color eyeIn  = new Color(1.00f, 1.00f, 0.60f);
        Color core   = new Color(1.00f, 0.45f, 0.10f);

        // Hull (inverted — wide top, pointed bottom)
        FillTri(px, s, s, 16, 2, 3, 28, 29, 28, hull);
        FillTri(px, s, s, 16, 5, 7, 25, 25, 25, hullDk);

        // Wings
        FillTri(px, s, s, 3, 24, 0, 30, 3, 30, wing);
        FillTri(px, s, s, 29, 24, 31, 30, 29, 30, wing);

        // Eyes (layered for bright inner pupil)
        FillCircle(px, s, s, 11, 20, 3, eye);
        FillCircle(px, s, s, 11, 20, 2, eyeIn);
        FillCircle(px, s, s, 21, 20, 3, eye);
        FillCircle(px, s, s, 21, 20, 2, eyeIn);

        // Central core
        FillCircle(px, s, s, 16, 11, 2, core);

        return BakeSprite(px, s, s, 32f);
    }

    // ── Bullet (6×14) ────────────────────────────────────────────
    private static Sprite BuildBulletSprite(Color outer, Color inner)
    {
        const int w = 6, h = 14;
        Color[] px = ClearCanvas(w, h);

        // Outer body
        FillRect(px, w, h, 1, 2, 4, 10, outer);
        // Bright inner core
        FillRect(px, w, h, 2, 3, 2, 8, inner);
        // Top cap
        FillRect(px, w, h, 2, 12, 2, 2, outer);
        FillRect(px, w, h, 2, 13, 2, 1, inner);
        // Bottom cap
        FillRect(px, w, h, 2, 0, 2, 2, outer);

        return BakeSprite(px, w, h, 32f);
    }

    // ── Star (4×4) ───────────────────────────────────────────────
    private static Sprite BuildStarSprite()
    {
        const int s = 4;
        Color[] px = ClearCanvas(s, s);

        Color full = Color.white;
        Color dim  = new Color(1f, 1f, 1f, 0.5f);

        // Soft cross pattern
        FillRect(px, s, s, 1, 0, 2, 4, dim);
        FillRect(px, s, s, 0, 1, 4, 2, dim);
        FillRect(px, s, s, 1, 1, 2, 2, full);

        return BakeSprite(px, s, s, 16f);
    }

    // ═════════════════════════════════════════════════════════════
    //  Drawing primitives
    // ═════════════════════════════════════════════════════════════

    private static Color[] ClearCanvas(int w, int h)
    {
        Color[] px = new Color[w * h];
        Color clear = new Color(0, 0, 0, 0);
        for (int i = 0; i < px.Length; i++) px[i] = clear;
        return px;
    }

    private static Sprite BakeSprite(Color[] px, int w, int h, float ppu)
    {
        Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false)
        {
            filterMode = FilterMode.Bilinear,
            wrapMode   = TextureWrapMode.Clamp
        };
        tex.SetPixels(px);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), ppu);
    }

    private static void SetPx(Color[] px, int w, int h, int x, int y, Color c)
    {
        if (x >= 0 && x < w && y >= 0 && y < h)
            px[y * w + x] = c;
    }

    private static void FillRect(Color[] px, int w, int h,
        int rx, int ry, int rw, int rh, Color c)
    {
        for (int dy = 0; dy < rh; dy++)
            for (int dx = 0; dx < rw; dx++)
                SetPx(px, w, h, rx + dx, ry + dy, c);
    }

    private static void FillCircle(Color[] px, int w, int h,
        int cx, int cy, int r, Color c)
    {
        int r2 = r * r;
        for (int dy = -r; dy <= r; dy++)
            for (int dx = -r; dx <= r; dx++)
                if (dx * dx + dy * dy <= r2)
                    SetPx(px, w, h, cx + dx, cy + dy, c);
    }

    private static void FillTri(Color[] px, int w, int h,
        int x0, int y0, int x1, int y1, int x2, int y2, Color c)
    {
        int minX = Mathf.Max(0, Mathf.Min(x0, Mathf.Min(x1, x2)));
        int maxX = Mathf.Min(w - 1, Mathf.Max(x0, Mathf.Max(x1, x2)));
        int minY = Mathf.Max(0, Mathf.Min(y0, Mathf.Min(y1, y2)));
        int maxY = Mathf.Min(h - 1, Mathf.Max(y0, Mathf.Max(y1, y2)));

        for (int py = minY; py <= maxY; py++)
            for (int ppx = minX; ppx <= maxX; ppx++)
                if (InsideTri(ppx, py, x0, y0, x1, y1, x2, y2))
                    px[py * w + ppx] = c;
    }

    private static bool InsideTri(int px, int py,
        int x0, int y0, int x1, int y1, int x2, int y2)
    {
        float d1 = EdgeCross(px, py, x0, y0, x1, y1);
        float d2 = EdgeCross(px, py, x1, y1, x2, y2);
        float d3 = EdgeCross(px, py, x2, y2, x0, y0);

        bool hasNeg = (d1 < 0) || (d2 < 0) || (d3 < 0);
        bool hasPos = (d1 > 0) || (d2 > 0) || (d3 > 0);
        return !(hasNeg && hasPos);
    }

    private static float EdgeCross(int px, int py, int ax, int ay, int bx, int by)
    {
        return (float)((px - bx) * (ay - by) - (ax - bx) * (py - by));
    }
}
