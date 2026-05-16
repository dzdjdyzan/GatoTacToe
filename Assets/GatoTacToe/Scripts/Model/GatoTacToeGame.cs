using UnityEngine;
using System.Collections.Generic;

namespace GatoTacToe.Model
{
    public enum GameState
    {
        ONGOING,
        PLAYER1_WINS,
        PLAYER2_WINS,
        DRAW
    }

    public class TicTacToeGame
    {
        // --- Data as per pseudocode ---
        private List<int> moveQueue = new List<int>();
        private bool isPlayer1Turn = true;
        private int[] grid = new int[9];   // 0 empty, 1 = Player1, 2 = Player2

        // Timing (using simple float timestamps, no Stopwatch needed)
        private float turnStartTime;
        private float totalTimePlayer1 = 0f;
        private float totalTimePlayer2 = 0f;

        public GameState CurrentState { get; private set; }

        public TicTacToeGame()
        {
            ResetGame();
        }

        public void ResetGame()
        {
            // Clear grid
            for (int i = 0; i < 9; i++) grid[i] = 0;
            moveQueue.Clear();
            isPlayer1Turn = true;
            totalTimePlayer1 = 0f;
            totalTimePlayer2 = 0f;
            CurrentState = GameState.ONGOING;

            // Start the timer for the first player's turn
            turnStartTime = Time.realtimeSinceStartup;
        }

        /// <summary>
        /// Play a turn at given coordinate (0-8). Assumes coordinate is valid and cell empty.
        /// Returns the game state after the move (ONGOING, or terminal state).
        /// </summary>
        public GameState PlayTurn(int coordinate)
        {
            // 1. Stop timer and add elapsed time to current player
            float elapsed = Time.realtimeSinceStartup - turnStartTime;
            if (isPlayer1Turn)
                totalTimePlayer1 += elapsed;
            else
                totalTimePlayer2 += elapsed;

            // 2. Place mark
            grid[coordinate] = isPlayer1Turn ? 1 : 2;
            moveQueue.Add(coordinate);

            // 3. Check game over
            CurrentState = CheckGameOver();

            // 4. If game continues, switch turn and restart timer
            if (CurrentState == GameState.ONGOING)
            {
                // Switch player
                isPlayer1Turn = !isPlayer1Turn;
                // Reset and start timer for next player
                turnStartTime = Time.realtimeSinceStartup;
            }

            // 5. Return the current state (as required)
            return CurrentState;
        }

        // Exactly your pseudocode's check_game_over (with fix for diagonal)
        private GameState CheckGameOver()
        {
           
            if ( (grid[0] != 0 && grid[0] == grid[1] && grid[1] == grid[2]) || 
                 (grid[3] != 0 && grid[3] == grid[4] && grid[4] == grid[5]) || 
                 (grid[6] != 0 && grid[6] == grid[7] && grid[7] == grid[8]) || 
                 (grid[0] != 0 && grid[0] == grid[3] && grid[3] == grid[6]) || 
                 (grid[1] != 0 && grid[1] == grid[4] && grid[4] == grid[7]) || 
                 (grid[2] != 0 && grid[2] == grid[5] && grid[5] == grid[8]) || 
                 (grid[0] != 0 && grid[0] == grid[4] && grid[4] == grid[8]) || 
                 (grid[2] != 0 && grid[2] == grid[4] && grid[4] == grid[6]) )
            {
                return isPlayer1Turn ? GameState.PLAYER1_WINS : GameState.PLAYER2_WINS;
            }

            // Draw check – any zero left?
            bool isFull = true;
            for (int i = 0; i < 9; i++)
            {
                if (grid[i] == 0)
                {
                    isFull = false;
                    break;
                }
            }
            if (isFull) return GameState.DRAW;

            return GameState.ONGOING;
        }

        // ----- Statistics getters (for persistence) -----
        public float GetPlayer1TotalTime() => totalTimePlayer1;
        public float GetPlayer2TotalTime() => totalTimePlayer2;

        public int GetPlayer1MoveCount()
        {
            if (moveQueue.Count == 0) return 0;
            return (moveQueue.Count + 1) / 2;
        }

        public int GetPlayer2MoveCount()
        {
            return moveQueue.Count / 2;
        }
    }

}