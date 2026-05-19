using UnityEngine;

public static class Theme
{
    // Background
    public static readonly Color backgroundColor = new Color(6f/255f, 22f/255f, 56f/255f, 1f);

    // Cell button feedback
    public static readonly Color cellHighlighted = new Color(41f/255f, 125f/255f, 240f/255f, 152f/255f);
    public static readonly Color cellPressed     = new Color(41f/255f, 125f/255f, 240f/255f, 152f/255f);

    // X mark colours (TextMeshPro)
    public static readonly Color markXVertex = new Color(255f/255f, 220f/255f, 0f/255f, 1f);
    public static readonly Color markXOutline = new Color(252f/255f, 220f/255f, 0f/255f, 1f);
    public static readonly Color markXGlow   = new Color(252f/255f, 220f/255f, 0f/255f, 128f/255f);

    // O mark colours (TextMeshPro)
    public static readonly Color markOVertex = new Color(248f/255f, 60f/255f, 25f/255f, 1f);
    public static readonly Color markOOutline = new Color(248f/255f, 60f/255f, 70f/255f, 1f);
    public static readonly Color markOGlow   = new Color(248f/255f, 60f/255f, 25f/255f, 128f/255f);

    // Grid lines (base colour – used when no glow material is assigned)
    public static readonly Color lineColor = new Color(255f/255f, 129f/255f, 0f/255f, 1f);
    // Glow material colour (applied to the GlowLine_material instance)
    public static readonly Color glowLineMaterialColor = new Color(255f/255f, 129f/255f, 0f/255f, 1f);
}