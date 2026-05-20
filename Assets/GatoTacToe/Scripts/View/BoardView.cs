using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class BoardView : MonoBehaviour
{
    [Header("Mark Prefabs (assign in Inspector)")]
    public GameObject markXPrefab;
    public GameObject markOPrefab;
    public Transform cellsContainer;

    private Button[] cells;
    private System.Action<int> onCellClickCallback;

    void Start()
    {
        // Find all cells under CellsContainer
        Transform cellsContainer = transform.Find("CellsContainer");
        if (cellsContainer == null) {
            Debug.LogError("BoardView: No CellsContainer found under BoardContainer.");
            return;
        }

        int childCount = cellsContainer.childCount;
        cells = new Button[childCount];
        for (int i = 0; i < childCount; i++) {
            Button btn = cellsContainer.GetChild(i).GetComponent<Button>();
            if (btn == null) {
                Debug.LogError("Something went wrong with getting buttons.");
                return;
            }
            int index = i; // capture for closure
            btn.onClick.AddListener(() => onCellClickCallback?.Invoke(index));
            cells[i] = btn;
        }
    }

    public void SetClickCallback(System.Action<int> callback) {
        onCellClickCallback = callback;
    }

    public void PlaceMark(int cellIndex, int player)
    {
        Transform cell = cells[cellIndex].transform;
        GameObject markPrefab = (player == 1) ? markXPrefab : markOPrefab;
        GameObject mark = Instantiate(markPrefab, cell);

        // Scale font size if needed (optional)
        TextMeshProUGUI tmp = mark.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp != null) {
            // Compute desired font size from metallic mean proportions
            float cellSize = GatoTacToeConfig.baseUnit * GatoTacToeConfig.metallicRatio;
            float targetFontSize = cellSize * GatoTacToeConfig.markFontScale;
            tmp.fontSize = Mathf.RoundToInt(targetFontSize);
        }

        // Animate
        mark.transform.localScale = Vector3.zero;
        mark.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);

        // Disable the button so it cannot be clicked again
        cells[cellIndex].interactable = false;
    }

    public void AnimateCell(int cellIndex)
    {
        Transform cell = cellsContainer.transform.GetChild(cellIndex);
        // DOTween example
        cell.DOScale(1.2f, 0.2f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            cell.DOScale(1f, 0.2f);
        });

    }

    public void ResetBoard()
    {
        // Remove all marks and re-enable all buttons
        Transform cellsContainer = transform.Find("CellsContainer");
        for (int i = 0; i < cellsContainer.childCount; i++)
        {
            Transform cell = cellsContainer.GetChild(i);
            // Remove mark children
            foreach (Transform child in cell)
            {
                if (child.CompareTag("Mark"))
                    Destroy(child.gameObject);
            }
            // Re-enable button
            Button btn = cell.GetComponent<Button>();
            if (btn != null) btn.interactable = true;
        }
    }
}