using UnityEngine;
using System.Collections.Generic;

namespace GatoTacToe.Model
{
    public enum GameState { ONGOING, PLAYER1_WINS, PLAYER2_WINS, DRAW }

    public class GatoTacToeGame
    {
        private readonly int gridSize;
        private int[] grid;
        private List<int> moveQueue = new List<int>();
        private bool isPlayer1Turn = true;
        private float turnStartTime;
        private float totalTimePlayer1, totalTimePlayer2;
        public GameState CurrentState { get; private set; }

        public GatoTacToeGame(int size)
        {
            gridSize = size;
            grid = new int[size * size];
            ResetGame();
        }

        public void ResetGame()
        {
            for (int i = 0; i < grid.Length; i++) grid[i] = 0;
            moveQueue.Clear();
            isPlayer1Turn = true;
            totalTimePlayer1 = totalTimePlayer2 = 0f;
            CurrentState = GameState.ONGOING;
            turnStartTime = Time.realtimeSinceStartup;
        }

        public GameState PlayTurn(int coordinate)
        {
            float elapsed = Time.realtimeSinceStartup - turnStartTime;
            if (isPlayer1Turn) totalTimePlayer1 += elapsed;
            else totalTimePlayer2 += elapsed;

            grid[coordinate] = isPlayer1Turn ? 1 : 2;
            moveQueue.Add(coordinate);

            CurrentState = CheckGameOver();

            if (CurrentState == GameState.ONGOING)
            {
                isPlayer1Turn = !isPlayer1Turn;
                turnStartTime = Time.realtimeSinceStartup;
            }
            return CurrentState;
        }

        private GameState Winner()
        {
            return isPlayer1Turn ? GameState.PLAYER1_WINS : GameState.PLAYER2_WINS;
        }

        private GameState CheckGameOver()
        {
            int n = gridSize;

            // Rows
            for (int row = 0; row < n; row++)
            {
                int first = grid[row * n];
                if (first != 0)
                {
                    int col = 1;
                    while (col < n && grid[row * n + col] == first) col++;
                    if (col == n) return Winner();
                }
            }

            // Columns
            for (int col = 0; col < n; col++)
            {
                int first = grid[col];
                if (first != 0)
                {
                    int row = 1;
                    while (row < n && grid[row * n + col] == first) row++;
                    if (row == n) return Winner();
                }
            }

            // Main diagonal (top-left to bottom-right)
            int mainFirst = grid[0];
            if (mainFirst != 0)
            {
                int i = 1;
                while (i < n && grid[i * n + i] == mainFirst) i++;
                if (i == n) return Winner();
            }

            // Anti-diagonal (top-right to bottom-left)
            int antiFirst = grid[n - 1];
            if (antiFirst != 0)
            {
                int i = 1;
                while (i < n && grid[i * n + (n - 1 - i)] == antiFirst) i++;
                if (i == n) return Winner();
            }

            // Draw check
            int idx = 0;
            while (idx < grid.Length && grid[idx] != 0) idx++;
            if (idx == grid.Length) return GameState.DRAW;

            return GameState.ONGOING;
        }

        public float GetCurrentTurnElapsed(float now)
        {
            if (CurrentState != GameState.ONGOING) return 0f;
            return now - turnStartTime;
        }
        
        public float GetPlayer1TotalTime() => totalTimePlayer1;
        public float GetPlayer2TotalTime() => totalTimePlayer2;
        public int GetPlayer1MoveCount() => moveQueue.Count == 0 ? 0 : (moveQueue.Count + 1) / 2;
        public int GetPlayer2MoveCount() => moveQueue.Count / 2;
        public bool IsPlayer1Turn => isPlayer1Turn;
    }
}