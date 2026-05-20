using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GatoTacToe.Model;

public class GatoTacToeController : MonoBehaviour  
{
    public BoardView boardView; // assign in Inspector
    public GameObject gameOverPopup;          // assign in Inspector
    public TextMeshProUGUI gameOverText;
    private GatoTacToeGame gameModel;
    public Button retryButton;   // add this
    public Button exitButton;    // add this
    private float gameStartTime;

    void Awake()
    {
        gameModel = new GatoTacToeGame(GatoTacToeConfig.gridSize);
    }

    void Start()
    {
        boardView.SetClickCallback(OnCellClicked);
        StartNewGame();

        retryButton.onClick.AddListener(OnRetryClicked);
        exitButton.onClick.AddListener(OnExitClicked);

        gameOverPopup.SetActive(false);
    }

    void StartNewGame()
    {
        gameOverPopup.SetActive(false);
        gameModel.ResetGame();
        boardView.ResetBoard();
        gameStartTime = Time.realtimeSinceStartup;
    }

    void OnCellClicked(int cellIndex)
    {
        if (gameModel.CurrentState != GameState.ONGOING) return;

        int currentPlayer = gameModel.IsPlayer1Turn ? 1 : 2;
        
        GameState newState = gameModel.PlayTurn(cellIndex);

        boardView.PlaceMark(cellIndex, currentPlayer);

        if (newState != GameState.ONGOING)
        {
            // Game ended
            /*string result = "";
            if (newState == GameState.PLAYER1_WINS) result = "PLAYER 1 WON";
            else if (newState == GameState.PLAYER2_WINS) result = "PLAYER 2 WON";
            else result = "DRAW";

            Debug.Log(result);

            // Auto-restart after 2 seconds
            Invoke(nameof(StartNewGame), 2f);*/
            HandleGameEnd(newState);
        }
    }

    void HandleGameEnd(GameState result)
    {
        // Show winning animation on the line if applicable
        if (result == GameState.PLAYER1_WINS || result == GameState.PLAYER2_WINS)
        {
            int[] winningLine = gameModel.GetWinningLine();
            if (winningLine != null)
            {
                // Animate each winning cell
                foreach (int index in winningLine)
                {
                    boardView.AnimateCell(index);
                }
            }
        }

        // Display popup after a short delay (let animation play)
        float delay = 0.5f; // adjust to match your animation duration
        Invoke(nameof(ShowGameOverPopup), delay);
    }

    void ShowGameOverPopup()
    {
        string winnerText;
        if (gameModel.CurrentState == GameState.PLAYER1_WINS)
            winnerText = "PLAYER X WON";
        else if (gameModel.CurrentState == GameState.PLAYER2_WINS)
            winnerText = "PLAYER O WON";
        else
            winnerText = "DRAW";

        gameOverText.text = winnerText;
        gameOverPopup.SetActive(true);
    }

    void OnRetryClicked()
    {
        StartNewGame();
    }

    void OnExitClicked()
    {
        gameOverPopup.SetActive(false);
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    public GatoTacToeGame GetGame() => gameModel;
}
