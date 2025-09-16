using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum PlayerN { None, Player1, Player2 }
public class ManaValues
{
    public int fire = 0;
    public int earth = 0;
    public int water = 0;
    public int air = 0;
}

public class Player
{
    public PlayerN identity;
    public ManaValues manaValues;
    public int Health;

    public Player(PlayerN name)
    {
        identity = name;
        manaValues = new ManaValues();
    }


}

