using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TurnManager : MonoBehaviour
{
    [Header("Managers")]
    public GridManager gridManager;
    public UIManager uiManager;
    [Header("Game State")]
    public Player player1;
    public Player player2;
    public Player currentPlayer;
    public ElementType transmuteElement;
    public bool transmuteSelected = false;

    [Header("UI References")]
    public Button endTurnButton;
    public TextMeshProUGUI currentPlayerText;
    public TextMeshProUGUI manaCountText;

    public enum TurnState { Ignite, SelectAction, End };
    public enum PlayerAction { Capture, Transmute, Attack, Ward, Extinguish, BigCapture, EditQualia, CastSpell, Select, AdminDebug };
    public TurnState currentState = TurnState.SelectAction;
    public PlayerAction? selectedAction = PlayerAction.Capture;

    public void Cancel()
    {
        transmuteSelected = false;
        selectedAction = null;
        currentState = TurnState.SelectAction;
    }
    public void EndTurn()
    {
        currentState = TurnState.End;
        // Mana cleanup
        currentPlayer.manaValues = new ManaValues();
        // change player identity
        currentPlayer = currentPlayer.identity == PlayerN.Player1 ? player2 : player1;
        if (currentPlayerText != null)
            currentPlayerText.text = $"Active Player: {currentPlayer.identity}";
        Ignite();
    }

    public void Ignite()
    {
        currentState = TurnState.Ignite;
        // Update Mana
        if (gridManager != null)
            gridManager.GridMana();
        UpdateManaText();
    }

    public void SelectAction()
    {
        currentState = TurnState.SelectAction;
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
        
        // Initialize UI
        UpdateManaText();
        if (currentPlayerText != null)
            currentPlayerText.text = $"Active Player: {currentPlayer.identity}";
    }

    public void CancelAction()
    {
        currentState = TurnState.SelectAction;
        selectedAction = null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
