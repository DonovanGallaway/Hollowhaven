using UnityEngine;

[System.Serializable]
public class GridSquare
{
    public ElementType elementType;
    public PlayerN playerOwnership = PlayerN.None;
    public int defenseValue = 0;
    public GameObject visualRepresentation;
    public ManaState manaState = ManaState.Uncaptured;
    public GameObject ownershipBorder;
}

public enum ElementType { Fire, Water, Earth, Air }

public enum ManaState { Ashed, Kindling, Ignited, Uncaptured}