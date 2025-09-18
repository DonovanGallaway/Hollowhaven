using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Managers")]
    public GridManager gridManager;
    public TurnManager turnManager;
    public Button endTurnButton;
    public TMP_Dropdown actionList;
    public TMP_Dropdown secondaryList;
    // Start is called before the first frame update
    void Start()
    {
        endTurnButton.onClick.AddListener(turnManager.EndTurn);
        actionList.ClearOptions();
        secondaryList.gameObject.SetActive(false);
        List<TMP_Dropdown.OptionData> dropdownOptions = new List<TMP_Dropdown.OptionData>();
        foreach (TurnManager.PlayerAction action in System.Enum.GetValues(typeof(TurnManager.PlayerAction)))
        {
            dropdownOptions.Add(new TMP_Dropdown.OptionData(action.ToString()));
        }
        actionList.AddOptions(dropdownOptions);

        // Add listener for when an action is selected
        actionList.onValueChanged.AddListener(OnActionSelected);
    }

    private void OnActionSelected(int index)
    {
        TurnManager.PlayerAction selectedAction = (TurnManager.PlayerAction)index;
        turnManager.selectedAction = selectedAction; 
        PopulateSecondaryDropdown(selectedAction);
    }

    private void PopulateSecondaryDropdown(TurnManager.PlayerAction selectedAction)
    {
        secondaryList.ClearOptions();
        List<TMP_Dropdown.OptionData> secondaryOptions = new List<TMP_Dropdown.OptionData>();

        switch (selectedAction)
        {
            case TurnManager.PlayerAction.Transmute:
                secondaryOptions.Add(new TMP_Dropdown.OptionData("--Select--"));
                foreach (ElementType element in System.Enum.GetValues(typeof(ElementType)))
                {
                    secondaryOptions.Add(new TMP_Dropdown.OptionData(element.ToString()));
                }
                secondaryList.gameObject.SetActive(true);
                secondaryList.AddOptions(secondaryOptions);
                secondaryList.onValueChanged.AddListener(MapTransmute);
                break;
            default:
                secondaryList.gameObject.SetActive(false);
                return;
        }

    }

    private void MapTransmute(int index)
    {
        if (index > 0)
        {
            turnManager.transmuteElement = (ElementType)index - 1;
            turnManager.transmuteSelected = true;
        }
        else
            turnManager.transmuteSelected = false;
    }


}
