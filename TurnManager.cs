using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TurnManager : MonoBehaviour
{
    [Header("Managers")]
    public GridManager gridManager;
    [Header("Game State")]
    public Player player1;
    public Player player2;
    public Player currentPlayer;

    [Header("UI References")]
    public Button endTurnButton;
    public TextMeshProUGUI currentPlayerText;
    public TextMeshProUGUI manaCountText;

    public enum TurnState { Ignite, SelectAction, TargetingSquare, TargetingMultiple, ConfirmingAction, ExecutingAction };
    public enum PlayerAction { Capture, Transmute, Attack, Ward, Extinguish, EditQualia, CastSpell };
    public TurnState currentState = TurnState.SelectAction;
    public PlayerAction? selectedAction = null;
    public void SwitchPlayer() // Ends turn
    {
        currentPlayer.manaValues = new ManaValues();
        currentPlayer = currentPlayer.identity == PlayerN.Player1 ? player2 : player1;

        if (currentPlayerText != null)
            currentPlayerText.text = $"Active Player: {currentPlayer.identity}";

        UpdateManaText();

        if (gridManager != null)
            gridManager.GridMana();
    }
    
    public void UpdateManaText()
    {
        if (currentPlayer != null && currentPlayer.manaValues != null && manaCountText != null)
        {
            ManaValues manaValues = currentPlayer.manaValues;
            manaCountText.text = $"Fire: {manaValues.fire}\nWater: {manaValues.water}\nAir: {manaValues.air}\nEarth: {manaValues.earth}";
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        player1 = new Player(PlayerN.Player1);
        player2 = new Player(PlayerN.Player2);
        currentPlayer = player1;
        endTurnButton.onClick.AddListener(SwitchPlayer);
        
        // Initialize UI
        UpdateManaText();
        if (currentPlayerText != null)
            currentPlayerText.text = $"Active Player: {currentPlayer.identity}";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
