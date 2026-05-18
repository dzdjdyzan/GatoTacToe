using UnityEngine;
using GatoTacToe.Model; // your model namespace

public class GatoTacToeController : MonoBehaviour  
{
     public BoardView boardView; // assign in Inspector
    private TicTacToeGame gameModel;
    private float gameStartTime;

    void Start()
    {
        gameModel = new TicTacToeGame(GatoTacToeConfig.gridSize);
        boardView.SetClickCallback(OnCellClicked);
        StartNewGame();
    }

    void StartNewGame()
    {
        gameModel.ResetGame();
        boardView.ResetBoard();
        gameStartTime = Time.realtimeSinceStartup;
        // Optionally update UI turn text here
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
            string result = "";
            if (newState == GameState.PLAYER1_WINS) result = "PLAYER 1 WON";
            else if (newState == GameState.PLAYER2_WINS) result = "PLAYER 2 WON";
            else result = "DRAW";

            Debug.Log(result);

            // Auto-restart after 2 seconds
            Invoke(nameof(StartNewGame), 2f);
        }
    }
}
