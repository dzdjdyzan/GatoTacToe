using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public class GatoTacToeBoardBuilder
{
    // Scaling factors
    private static int gridSize = 3;
    private static float metallicRatio = 3.303f;
    private static float baseUnit = 96f;
    private static float markFontScale = 0.6f;

    [MenuItem("Tools/Build GatoTacToe Board")]
    private static void BuildBoard()
    {
        Canvas canvas = GameObject.Find("GameCanvas")?.GetComponent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("No Canvas named 'GameCanvas' found.");
            return;
        }

        // ----- 1. Background -----
        Transform bgTransform = canvas.transform.Find("Background");
        if (bgTransform != null)
            bgTransform.GetComponent<Image>().color = Theme.backgroundColor;
        else
        {
            GameObject bg = new GameObject("Background", typeof(RectTransform), typeof(Image));
            bg.transform.SetParent(canvas.transform, false);
            RectTransform bgRect = bg.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = bgRect.offsetMax = Vector2.zero;
            bg.GetComponent<Image>().color = Theme.backgroundColor;
        }

        // ----- 2. Load all prefabs and material -----
        GameObject cellPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/GatoTacToe/Prefabs/Cell.prefab");
        GameObject horizontalLinePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/GatoTacToe/Prefabs/HorizontalLine.prefab");
        GameObject verticalLinePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/GatoTacToe/Prefabs/VerticalLine.prefab");
        GameObject markXPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/GatoTacToe/Prefabs/Mark_X.prefab");
        GameObject markOPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/GatoTacToe/Prefabs/Mark_O.prefab");
        Material glowMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/GatoTacToe/UI/Materials/GlowLine_Material.mat");

        if (cellPrefab == null || horizontalLinePrefab == null || verticalLinePrefab == null ||
            markXPrefab == null || markOPrefab == null || glowMaterial == null)
        {
            Debug.LogError("One or more prefabs or material not found. Check paths.");
            return;
        }

        // ----- 3. Apply theme to mark prefabs (so new instances have correct colours) -----
        ApplyThemeToMarkPrefab(markXPrefab, 
            Theme.markXVertex, 
            Theme.markXOutline, 
            Theme.markXGlow);
        ApplyThemeToMarkPrefab(markOPrefab, 
            Theme.markOVertex, 
            Theme.markOOutline, 
            Theme.markOGlow);

        // ----- 4. Board dimensions -----
        float U = baseUnit;
        float phi = metallicRatio;
        float cellSize = U * phi;
        float gap = U / 4f;
        float boardSize = (cellSize + gap) * gridSize - gap;

        // ----- 5. BoardContainer -----
        GameObject boardContainer = new GameObject("BoardContainer", typeof(RectTransform));
        boardContainer.transform.SetParent(canvas.transform, false);
        RectTransform boardRect = boardContainer.GetComponent<RectTransform>();
        boardRect.anchorMin = boardRect.anchorMax = new Vector2(0.5f, 0.5f);
        boardRect.pivot = new Vector2(0.5f, 0.5f);
        boardRect.sizeDelta = new Vector2(boardSize, boardSize);
        boardRect.anchoredPosition = Vector2.zero;

        // ----- 6. CellsContainer with GridLayoutGroup -----
        GameObject cellsContainer = new GameObject("CellsContainer", typeof(RectTransform));
        cellsContainer.transform.SetParent(boardContainer.transform, false);
        RectTransform cellsRect = cellsContainer.GetComponent<RectTransform>();
        cellsRect.anchorMin = Vector2.zero;
        cellsRect.anchorMax = Vector2.one;
        cellsRect.offsetMin = cellsRect.offsetMax = Vector2.zero;

        GridLayoutGroup grid = cellsContainer.AddComponent<GridLayoutGroup>();
        grid.cellSize = new Vector2(cellSize, cellSize);
        grid.spacing = new Vector2(gap, gap);
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = gridSize;
        grid.childAlignment = TextAnchor.MiddleCenter;

        for (int i = 0; i < gridSize * gridSize; i++)
        {
            GameObject cell = (GameObject)PrefabUtility.InstantiatePrefab(cellPrefab, cellsContainer.transform);
            cell.name = $"Cell_{i}";
            Button btn = cell.GetComponent<Button>();
            if (btn != null)
            {
                ColorBlock colors = btn.colors;
                colors.highlightedColor = Theme.cellHighlighted;
                colors.pressedColor = Theme.cellPressed;
                btn.colors = colors;
            }
        }

        // ----- 7. LinesContainer (using line prefabs with glow material AND image colour) -----
        GameObject linesContainer = new GameObject("LinesContainer", typeof(RectTransform));
        linesContainer.transform.SetParent(boardContainer.transform, false);
        RectTransform linesRect = linesContainer.GetComponent<RectTransform>();
        linesRect.anchorMin = Vector2.zero;
        linesRect.anchorMax = Vector2.one;
        linesRect.offsetMin = linesRect.offsetMax = Vector2.zero;

        // IMPORTANT: Change "_Color" to match your material's HDR colour property name.
        // Select GlowLine_Material in Project view and check the label next to the colour picker.
        string colorPropertyName = "_Color"; // e.g., "_GlowColor", "_EmissionColor"

        for (int i = 1; i < gridSize; i++)
        {
            float t = i / (float)gridSize;

            // Horizontal line
            GameObject hLine = (GameObject)PrefabUtility.InstantiatePrefab(horizontalLinePrefab, linesContainer.transform);
            hLine.name = $"HLine_{i}";
            RectTransform hRect = hLine.GetComponent<RectTransform>();
            hRect.anchorMin = new Vector2(0, t);
            hRect.anchorMax = new Vector2(1, t);
            hRect.offsetMin = new Vector2(0, -gap/2);
            hRect.offsetMax = new Vector2(0, gap/2);
            
            Image hImg = hLine.GetComponent<Image>();
            if (hImg != null)
            {
                // Assign a unique material instance so we can change its colour without affecting other lines
                Material mat = new Material(glowMaterial);
                hImg.material = mat;
                mat.SetColor(colorPropertyName, Theme.glowLineMaterialColor);
                // Also set the image colour (this gives the exact combined look you want)
                hImg.color = Theme.lineColor;
            }

            // Vertical line
            GameObject vLine = (GameObject)PrefabUtility.InstantiatePrefab(verticalLinePrefab, linesContainer.transform);
            vLine.name = $"VLine_{i}";
            RectTransform vRect = vLine.GetComponent<RectTransform>();
            vRect.anchorMin = new Vector2(t, 0);
            vRect.anchorMax = new Vector2(t, 1);
            vRect.offsetMin = new Vector2(-gap/2, 0);
            vRect.offsetMax = new Vector2(gap/2, 0);
            
            Image vImg = vLine.GetComponent<Image>();
            if (vImg != null)
            {
                Material mat = new Material(glowMaterial);
                vImg.material = mat;
                mat.SetColor(colorPropertyName, Theme.glowLineMaterialColor);
                vImg.color = Theme.lineColor;
            }
        }

        // Save changes to the mark prefabs
        AssetDatabase.SaveAssets();
        Debug.Log("Board built and theme applied to mark prefabs.");
    }

    // Helper: apply theme colours to a mark prefab (TextMeshPro, Outline, and glow image)
    private static void ApplyThemeToMarkPrefab(GameObject prefab, Color vertexColor, Color outlineColor, Color glowColor)
    {
        // TextMeshPro component (on root or child)
        TextMeshProUGUI tmp = prefab.GetComponent<TextMeshProUGUI>();
        if (tmp == null) tmp = prefab.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp != null) tmp.color = vertexColor;
        else Debug.LogWarning($"No TextMeshProUGUI found on {prefab.name}");

        // Outline component (if any)
        Outline outline = prefab.GetComponent<Outline>();
        if (outline == null) outline = prefab.GetComponentInChildren<Outline>();
        if (outline != null) outline.effectColor = outlineColor;

        // Glow image (optional – if the prefab has an Image with a glow material)
        Image glowImage = prefab.GetComponent<Image>();
        if (glowImage == null) glowImage = prefab.GetComponentInChildren<Image>();
        if (glowImage != null && glowImage.material != null)
        {
            // The property name for the glow image's colour might be "_Color" – change if different
            glowImage.material.SetColor("_Color", glowColor);
        }

        EditorUtility.SetDirty(prefab);
    }
}