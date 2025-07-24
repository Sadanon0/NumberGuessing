using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Gameplay : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI atttempsLeft;
    public TextMeshProUGUI currentPlayer;
    public TextMeshProUGUI gameState;
    public TextMeshProUGUI gameLog;

    [Header("Input")]
    public TMP_InputField guessInputField;
    public Button submitButton;
    public Button newgameButton;

    [Header("Game Settings")]
    public int minNumber = 1;
    public int maxNumber = 100;
    public int maxAttemps = 12;

    private int targetNumber;
    private int currentAttemps;
    private bool isPlayerTurn;
    private bool gameActive;

    private int computerMinguess;
    private int computerMaxguess;
    private int lastComputerGuess;

    void InitializeUI()
    {
        submitButton.onClick.AddListener(SubmitGuess);
        newgameButton.onClick.AddListener(StartNewGame);
        guessInputField.onSubmit.AddListener(delegate { SubmitGuess(); });
    }

    void SubmitGuess()
    {
        if (!gameActive || !isPlayerTurn) return;

        string input = guessInputField.text.Trim();
        if (string.IsNullOrEmpty(input)) return;

        if (!int.TryParse(input, out int guess))
        {
            gameState.text += "Please enter a valid number.\n";
            return;
        }

        if (guess < minNumber || guess > maxNumber)
        {
            gameState.text += $"<sprite=6> Please enter a number between {minNumber} - {maxNumber}\n";
            return;
        }

        guessInputField.text = "";
        ProcessGuess(guess, true);
    }

    void ProcessGuess(int guess, bool isPlayer)
    {
        currentAttemps++;
        string playerName = isPlayer ? "Player" : "Computer";

        gameLog.text += $"{playerName} guessed: {guess}\n";

        if (guess == targetNumber)
        {
            gameLog.text += $"<sprite=\"Symbols\" index=23> {playerName} got it right!\n";
            EndGame();
        }
        else if (currentAttemps >= maxAttemps)
        {
            gameLog.text += $"<sprite=6> Game Over! The correct number was {targetNumber}\n";
            EndGame();
        }
        else
        {
            string hint = guess < targetNumber ? "Too Low" : "Too High";
            gameLog.text += $"<sprite=\"Symbols\" index=24> {hint}\n";

            // Update range for computer
            if (!isPlayer)
            {
                if (guess < targetNumber)
                    computerMinguess = guess + 1;
                else
                    computerMaxguess = guess - 1;
            }

            // Switch turn
            isPlayerTurn = !isPlayer;
            currentPlayer.text = isPlayerTurn ? "Player's Turn" : "Computer's Turn";
            atttempsLeft.text = $"Attempts left: {maxAttemps - currentAttemps}";

            if (!isPlayerTurn)
            {
                guessInputField.interactable = false;
                submitButton.interactable = false;
                StartCoroutine(ComputerTurn());
            }
            else
            {
                guessInputField.interactable = true;
                submitButton.interactable = true;
                guessInputField.Select();
                guessInputField.ActivateInputField();
            }
        }
    }

    IEnumerator ComputerTurn()
    {
        yield return new WaitForSeconds(1.5f);

        if (!gameActive) yield break;

        lastComputerGuess = (computerMinguess + computerMaxguess) / 2;
        ProcessGuess(lastComputerGuess, false);
    }

    void EndGame()
    {
        gameActive = false;
        guessInputField.interactable = false;
        submitButton.interactable = false;
        currentPlayer.text = "";
        gameState.text = "Game Over! Click 'New Game' to start again.";
        Canvas.ForceUpdateCanvases();
    }

    void StartNewGame()
    {
        targetNumber = Random.Range(minNumber, maxNumber + 1);
        currentAttemps = 0;
        isPlayerTurn = true;
        gameActive = true;

        currentPlayer.text = "Player's Turn";
        atttempsLeft.text = $"Attempts left: {maxAttemps}";
        gameLog.text = "=== Game Log ===\n";
        gameState.text = "Game In Progress";

        guessInputField.text = "";
        guessInputField.interactable = true;
        submitButton.interactable = true;
        guessInputField.Select();
        guessInputField.ActivateInputField();

        computerMinguess = minNumber;
        computerMaxguess = maxNumber;
        lastComputerGuess = 0;
    }

    void Start()
    {
        InitializeUI();
        StartNewGame();
    }
}
