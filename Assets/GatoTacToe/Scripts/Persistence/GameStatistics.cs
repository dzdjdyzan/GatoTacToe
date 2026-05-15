using UnityEngine;
using GatoTacToe.Model;

namespace GatoTacToe.Persistence
{
    [System.Serializable]
    public class GameStatistics
    {
        public int totalGamesPlayed = 0;
        public int player1Wins = 0;
        public int player2Wins = 0;
        public int draws = 0;
        public float totalGameTimeSeconds = 0f;     // sum of all game durations
        public float totalPlayer1TimeSeconds = 0f;
        public float totalPlayer2TimeSeconds = 0f;
        public int totalPlayer1Moves = 0;
        public int totalPlayer2Moves = 0;

        // Computed averages (read-only)
        public float AverageGameTime => totalGamesPlayed > 0 ? totalGameTimeSeconds / totalGamesPlayed : 0f;
        public float AveragePlayer1TimePerGame => totalGamesPlayed > 0 ? totalPlayer1TimeSeconds / totalGamesPlayed : 0f;
        public float AveragePlayer2TimePerGame => totalGamesPlayed > 0 ? totalPlayer2TimeSeconds / totalGamesPlayed : 0f;
        public float AveragePlayer1MovesPerGame => totalGamesPlayed > 0 ? (float)totalPlayer1Moves / totalGamesPlayed : 0f;
        public float AveragePlayer2MovesPerGame => totalGamesPlayed > 0 ? (float)totalPlayer2Moves / totalGamesPlayed : 0f;
    }

    public static class StatsManager
    {
        private const string STATS_KEY = "GatoTacToeStats";
        public static GameStatistics Stats { get; private set; }

        [RuntimeInitializeOnLoadMethod]
        private static void LoadStats()
        {
            string json = PlayerPrefs.GetString(STATS_KEY, "");
            if (!string.IsNullOrEmpty(json))
            {
                Stats = JsonUtility.FromJson<GameStatistics>(json);
            }
            else
            {
                Stats = new GameStatistics();
                SaveStats();
            }
        }

        public static void SaveStats()
        {
            string json = JsonUtility.ToJson(Stats);
            PlayerPrefs.SetString(STATS_KEY, json);
            PlayerPrefs.Save();  // Immediately writes to disk (important for WebGL)
        }

        /// <summary>
        /// Call this after a game finishes.
        /// </summary>
        public static void RecordCompletedGame(GameState result, float gameDurationSeconds,
                                            float player1Time, float player2Time,
                                            int player1Moves, int player2Moves)
        {
            Stats.totalGamesPlayed++;
            Stats.totalGameTimeSeconds += gameDurationSeconds;
            Stats.totalPlayer1TimeSeconds += player1Time;
            Stats.totalPlayer2TimeSeconds += player2Time;
            Stats.totalPlayer1Moves += player1Moves;
            Stats.totalPlayer2Moves += player2Moves;

            switch (result)
            {
                case GameState.PLAYER1_WINS: Stats.player1Wins++; break;
                case GameState.PLAYER2_WINS: Stats.player2Wins++; break;
                case GameState.DRAW: Stats.draws++; break;
            }

            SaveStats();
        }
    }


}


