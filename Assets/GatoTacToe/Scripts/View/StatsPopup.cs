using TMPro;
using UnityEngine;
using GatoTacToe.Persistence; // adjust namespace

public class StatsPopup : MonoBehaviour
{
    public TextMeshProUGUI statsText;

    private void OnEnable()
    {
        UpdateStats();
        // Play popup SFX later
    }

    void UpdateStats()
    {
        var s = StatsManager.Stats;
        statsText.text = $"Total games played: {s.totalGamesPlayed}\n\n" +
                         $"Player X wins: {s.player1Wins}\n\n" +
                         $"Player O wins: {s.player2Wins}\n\n" +
                         $"Draws: {s.draws}\n\n" +
                         $"Average game duration: {s.AverageGameTime:F1} seconds\n\n" +
                         $"Average time per player X: {s.AveragePlayer1TimePerGame:F1} sec\n\n" +
                         $"Average time per player O: {s.AveragePlayer2TimePerGame:F1} sec";
    }
}