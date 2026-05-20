using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GatoTacToe.Model;
using GatoTacToe.Persistence;

public class GatoTacToeController : MonoBehaviour  
{
    public GameObject startMenu;
    public GameObject gameOverPopup; 
    public GameObject statsPopup;
    
    public BoardView boardView; // assign in Inspector
    private GatoTacToeGame gameModel; 
   
   
    public TextMeshProUGUI gameOverText; // assign in Inspector
    // assign in Inspector
    
    public Button retryButton;   // assign in Inspector
    public Button playButton;
    public Button abortButton;
    public Button statsButton;
    public Button statsBackButton;

    private float gameStartTime;

    void Awake()
    {
        gameModel = new GatoTacToeGame(GatoTacToeConfig.gridSize);
    }

    void Start()
    {
        boardView.SetClickCallback(OnCellClicked);
        //StartNewGame();

        retryButton.onClick.AddListener(OnRetryClicked);
        playButton.onClick.AddListener(OnPlayClicked);
        abortButton.onClick.AddListener(OnAbortClicked);
        statsButton.onClick.AddListener(OnStatsClicked);
        statsBackButton.onClick.AddListener(OnStatsBackClicked);
        
        gameOverPopup.SetActive(false);
        startMenu.SetActive(true);

    }

    void StartNewGame()
    {
        gameOverPopup.SetActive(false);
        gameModel.ResetGame();
        boardView.ResetBoard();
        gameStartTime = Time.realtimeSinceStartup;
    }

    public void OnPlayClicked()
    {
        startMenu.SetActive(false);
        StartNewGame(); // resets model, board, timers, game over popup
    }

    public void OnStatsClicked()
    {
        statsPopup.SetActive(true);
    }

    public void OnStatsBackClicked()
    {
        statsPopup.SetActive(false);
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
        float now = Time.realtimeSinceStartup;
        float gameDuration = now - gameStartTime;
        float p1Time = gameModel.GetPlayer1TotalTime() + (gameModel.IsPlayer1Turn ? gameModel.GetCurrentTurnElapsed(now) : 0);
        float p2Time = gameModel.GetPlayer2TotalTime() + (!gameModel.IsPlayer1Turn ? gameModel.GetCurrentTurnElapsed(now) : 0);
        int p1Moves = gameModel.GetPlayer1MoveCount();
        int p2Moves = gameModel.GetPlayer2MoveCount();

        StatsManager.RecordCompletedGame(result, gameDuration, p1Time, p2Time, p1Moves, p2Moves);

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

            AudioManager.Instance.PlayStrikeWin();
        }

        // Display popup after a short delay (let animation play)
        float delay = 0.5f; // adjust to match your animation duration
        Invoke(nameof(ShowGameOverPopup), delay);
    }

    void ShowGameOverPopup()
    {
        AudioManager.Instance.PlayPopupAnim();
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

    /*void OnExitClicked()
    {
        gameOverPopup.SetActive(false);
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }*/

    public void OnAbortClicked()
    {
        AbortGame();
    }

    public void AbortGame()
    {
        // Cancel any pending Invoke (e.g., ShowGameOverPopup)
        CancelInvoke();
        
        // Reset the game model (clears board, times, turn)
        gameModel.ResetGame();
        
        // Reset the visual board (clear marks, re‑enable buttons)
        boardView.ResetBoard();
        
        // Hide game over popup if it's visible
        gameOverPopup.SetActive(false);
        
        // Show the main menu
        startMenu.SetActive(true);
    }

    public GatoTacToeGame GetGame() => gameModel;
}
