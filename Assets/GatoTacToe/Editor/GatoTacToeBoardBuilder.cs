using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class GatoTacToeBoardBuilder
{
    [MenuItem("Tools/Build GatoTacToe Board")]
    private static void BuildBoard()
    {
        Canvas canvas = GameObject.Find("GameCanvas")?.GetComponent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("No Canvas in scene. Create a Canvas first.");
            return;
        }

        // Adjust these paths to match your prefab locations
        GameObject cellPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/GatoTacToe/Prefabs/Cell.prefab");
        GameObject markXPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/GatoTacToe/Prefabs/Mark_X.prefab");
        GameObject markOPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/GatoTacToe/Prefabs/Mark_O.prefab");

        if (cellPrefab == null || markXPrefab == null || markOPrefab == null)
        {
            Debug.LogError("One or more prefabs not found. Check paths.");
            return;
        }

        int n = GatoTacToeConfig.gridSize;
        float U = GatoTacToeConfig.baseUnit;
        float phi = GatoTacToeConfig.metallicRatio;
        float cellSize = U * phi;
        float gap = U / 4f;
        float boardSize = (cellSize + gap) * n - gap;

        // BoardContainer
        GameObject boardContainer = new GameObject("BoardContainer", typeof(RectTransform));
        boardContainer.transform.SetParent(canvas.transform, false);
        RectTransform boardRect = boardContainer.GetComponent<RectTransform>();
        boardRect.anchorMin = boardRect.anchorMax = new Vector2(0.5f, 0.5f);
        boardRect.pivot = new Vector2(0.5f, 0.5f);
        boardRect.sizeDelta = new Vector2(boardSize, boardSize);
        boardRect.anchoredPosition = Vector2.zero;

        // CellsContainer (with GridLayoutGroup)
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
        grid.constraintCount = n;
        grid.childAlignment = TextAnchor.MiddleCenter;

        for (int i = 0; i < n * n; i++)
        {
            GameObject cell = (GameObject)PrefabUtility.InstantiatePrefab(cellPrefab, cellsContainer.transform);
            cell.name = $"Cell_{i}";

            Image img = cell.GetComponent<Image>();
            if (img) img.color = GatoTacToeConfig.cellNormalColor;

            Button btn = cell.GetComponent<Button>();
            if (btn)
            {
                ColorBlock colors = btn.colors;
                colors.normalColor = GatoTacToeConfig.cellNormalColor;
                colors.highlightedColor = GatoTacToeConfig.cellHighlightColor;
                colors.pressedColor = GatoTacToeConfig.cellPressedColor;
                btn.colors = colors;
            }
        }

        // LinesContainer
        GameObject linesContainer = new GameObject("LinesContainer", typeof(RectTransform));
        linesContainer.transform.SetParent(boardContainer.transform, false);
        RectTransform linesRect = linesContainer.GetComponent<RectTransform>();
        linesRect.anchorMin = Vector2.zero;
        linesRect.anchorMax = Vector2.one;
        linesRect.offsetMin = linesRect.offsetMax = Vector2.zero;

        for (int i = 1; i < n; i++)
        {
            float t = i / (float)n;

            // Horizontal line
            GameObject hLine = new GameObject($"HLine_{i}", typeof(RectTransform), typeof(Image));
            hLine.transform.SetParent(linesContainer.transform, false);
            RectTransform hRect = hLine.GetComponent<RectTransform>();
            hRect.anchorMin = new Vector2(0, t);
            hRect.anchorMax = new Vector2(1, t);
            hRect.offsetMin = new Vector2(0, -gap/2);
            hRect.offsetMax = new Vector2(0, gap/2);
            hLine.GetComponent<Image>().color = GatoTacToeConfig.lineColor;

            // Vertical line
            GameObject vLine = new GameObject($"VLine_{i}", typeof(RectTransform), typeof(Image));
            vLine.transform.SetParent(linesContainer.transform, false);
            RectTransform vRect = vLine.GetComponent<RectTransform>();
            vRect.anchorMin = new Vector2(t, 0);
            vRect.anchorMax = new Vector2(t, 1);
            vRect.offsetMin = new Vector2(-gap/2, 0);
            vRect.offsetMax = new Vector2(gap/2, 0);
            vLine.GetComponent<Image>().color = GatoTacToeConfig.lineColor;
        }

        Debug.Log($"Board built: {n}x{n}, cell size = {cellSize}, board size = {boardSize}");
    }
}