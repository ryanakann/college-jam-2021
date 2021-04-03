using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Player
{
    public Faction faction;
    public Color color;
    public bool isHuman;
    public int nodes;

    public Player(Faction faction, Color color, bool isHuman)
    {
        this.faction = faction;
        this.color = color;
        this.isHuman = isHuman;
        nodes = 0;
    }

    public virtual void Activate()
    {
        // tell the controller which player is going
    }
}