using TMPro;
using UnityEngine;
using GatoTacToe.Model;

public class GatoTacToeGameUIManager : MonoBehaviour
{
    [Header("Top Panel Texts")]
    public TextMeshProUGUI player1TimerText;   // "Player X\n00:00"
    public TextMeshProUGUI totalTimerText;     // "TIME PLAYED\n00:00"
    public TextMeshProUGUI player2TimerText;   // "Player O\n00:00"

    [Header("Bottom Panel Texts")]
    public TextMeshProUGUI player1TurnsText;   // "X Turns\n0"
    public TextMeshProUGUI player2TurnsText;   // "Y Turns\n0"

    private GatoTacToeGame game;
    private float nextUpdateTime;

    void Start()
    {
        Debug.Log("GameUIManager Start() called");
        var controller = FindAnyObjectByType<GatoTacToeController>();
        Debug.Log("Controller found: " + (controller != null));
        if (controller != null)
        {
            game = controller.GetGame();
            Debug.Log("Game instance obtained: " + (game != null));
        }
        else
        {
            Debug.LogError("GatoTacToeController not found!");
        }
        nextUpdateTime = Time.realtimeSinceStartup;
    }

    void Update()
    {
        RefreshDisplay();
    }

    void RefreshDisplay()
    {
        //Debug.Log("RefreshDisplay called, game is " + (game == null ? "NULL" : "not null"));
        if (game == null) return;

        float now = Time.realtimeSinceStartup;
        float ongoing = game.GetCurrentTurnElapsed(now);  // uses the provided timestamp

        float p1Elapsed = game.GetPlayer1TotalTime() + (game.IsPlayer1Turn ? ongoing : 0);
        float p2Elapsed = game.GetPlayer2TotalTime() + (!game.IsPlayer1Turn ? ongoing : 0);
        float totalElapsed = p1Elapsed + p2Elapsed;

        player1TimerText.text = $"X Time\n{FormatTime(p1Elapsed)}";
        player2TimerText.text = $"O Time\n{FormatTime(p2Elapsed)}";
        totalTimerText.text = $"{FormatTotalTime(totalElapsed)}";

        // --- Turn counts ---
        player1TurnsText.text = $"{game.GetPlayer1MoveCount()}\nX Moves";
        player2TurnsText.text = $"{game.GetPlayer2MoveCount()}\nO Moves";
    }

    string FormatTotalTime(float seconds)
    {
        int mins = Mathf.FloorToInt(seconds / 60);
        int secs = Mathf.FloorToInt(seconds % 60);
        return $"{mins:00}:{secs:00}";
    }

    string FormatTime(float seconds)
    {
        int mins = Mathf.FloorToInt(seconds / 60);
        float secs = seconds % 60;
        return $"{mins:00}:{secs:00.0}";  // e.g., "00:05.4"
    }
}