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

    public int this[int index]
    {
        get
        {
            return index switch
            {
                0 => fire,
                1 => water, 
                2 => earth,
                3 => air,
                _ => 0
            };
        }
        set
        {
            switch (index)
            {
                case 0: fire = value; break;
                case 1: water = value; break;
                case 2: earth = value; break;
                case 3: air = value; break;
            }
        }
    }
    
    public int this[ElementType element]
    {
        get => this[(int)element];
        set => this[(int)element] = value;
    }
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

