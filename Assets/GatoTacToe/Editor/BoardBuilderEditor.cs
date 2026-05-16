using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class BoardBuilderEditor
{
    private const float BASE_UNIT = 24f;
    private static readonly float METALLIC_MEAN = 2f + Mathf.Sqrt(5f); // ≈ 4.236

    [MenuItem("Tools/Build GatoTacToe Board")]
    private static void BuildBoard()
    {
        // 1. Find BoardContainer
        GameObject boardContainer = GameObject.Find("BoardContainer");
        if (boardContainer == null)
        {
            Debug.LogError("BoardBuilder: No GameObject named 'BoardContainer' found in the scene.");
            return;
        }

        // 2. Ensure BoardContainer has a RectTransform (it should, as a child of Canvas)
        RectTransform boardRect = boardContainer.GetComponent<RectTransform>();
        if (boardRect == null)
        {
            boardRect = boardContainer.AddComponent<RectTransform>();
        }

        // 3. Compute dimensions using metallic mean
        float cellSize = BASE_UNIT * METALLIC_MEAN;       // ≈ 102
        float gap = BASE_UNIT / 4f;                      // = 6
        float boardSize = (cellSize + gap) * 3 - gap;     // ≈ 318

        // 4. Set BoardContainer's size and anchors (centered)
        boardRect.anchorMin = new Vector2(0.5f, 0.5f);
        boardRect.anchorMax = new Vector2(0.5f, 0.5f);
        boardRect.pivot = new Vector2(0.5f, 0.5f);
        boardRect.sizeDelta = new Vector2(boardSize, boardSize);
        boardRect.anchoredPosition = Vector2.zero; // centered in parent (Canvas)

        // 5. Load the Cell prefab (adjust path to match your project)
        string cellPrefabPath = "Assets/GatoTacToe/Prefabs/Cell.prefab";
        GameObject cellPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(cellPrefabPath);
        if (cellPrefab == null)
        {
            Debug.LogError($"BoardBuilder: Cell prefab not found at '{cellPrefabPath}'. Adjust the path in the script.");
            return;
        }

        // 6. Clear existing children (cells and lines)
        while (boardContainer.transform.childCount > 0)
        {
            GameObject child = boardContainer.transform.GetChild(0).gameObject;
            Undo.DestroyObjectImmediate(child);
        }

        // 7. Add or reconfigure GridLayoutGroup on BoardContainer
        GridLayoutGroup grid = boardContainer.GetComponent<GridLayoutGroup>();
        if (grid == null) grid = boardContainer.AddComponent<GridLayoutGroup>();
        grid.cellSize = new Vector2(cellSize, cellSize);
        grid.spacing = new Vector2(gap, gap);
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = 3;
        grid.childAlignment = TextAnchor.MiddleCenter;

        // 8. Instantiate the 9 cells (undo‑able)
        for (int i = 0; i < 9; i++)
        {
            GameObject cell = (GameObject)PrefabUtility.InstantiatePrefab(cellPrefab, boardContainer.transform);
            cell.name = $"Cell_{i}";
            Undo.RegisterCreatedObjectUndo(cell, "Create Cell");
        }

        // 9. Create the 6 board lines as UI images
        Color lineColor = new Color(0.96f, 0.55f, 0.13f, 0.8f); // neon orange

        System.Action<string, Vector2, Vector2, float, bool> createLine = (name, anchorMin, anchorMax, thickness, isHorizontal) =>
        {
            GameObject line = new GameObject(name, typeof(RectTransform), typeof(Image));
            line.transform.SetParent(boardContainer.transform, false);
            line.transform.SetAsFirstSibling(); // put behind cells
            RectTransform rt = line.GetComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            if (isHorizontal)
                rt.sizeDelta = new Vector2(0, thickness);
            else
                rt.sizeDelta = new Vector2(thickness, 0);
            line.GetComponent<Image>().color = lineColor;
            Undo.RegisterCreatedObjectUndo(line, "Create Line");
        };

        // Horizontal lines (between rows) – note anchors are relative to parent size
        createLine("HLine_Top",    new Vector2(0, 1f/3f), new Vector2(1, 1f/3f), gap, true);
        createLine("HLine_Bottom", new Vector2(0, 2f/3f), new Vector2(1, 2f/3f), gap, true);
        // Vertical lines
        createLine("VLine_Left",   new Vector2(1f/3f, 0), new Vector2(1f/3f, 1), gap, false);
        createLine("VLine_Right",  new Vector2(2f/3f, 0), new Vector2(2f/3f, 1), gap, false);

        Debug.Log($"BoardBuilder: Board built successfully. Size = {boardSize}x{boardSize}");
    }
}