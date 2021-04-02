using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Graphs.Nodes;

public class PlayerSelectManager : MonoBehaviour
{

    List<NodePlayerSelection> players = new List<NodePlayerSelection>();

    public static PlayerSelectManager instance;

    public void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    public void RegisterPlayer(NodePlayerSelection selection)
    {
        players.Add(selection);
    }

    public void DeregisterPlayer(NodePlayerSelection selection)
    {
        players.Remove(selection);
    }

    public void FinalizePlayers()
    {
        if (players.Count <= 1) 
        {
            print("Need at least 2 players!"); // needs to be a pop-up
            return;
        }

        // feed information to TurnManager
        // change scene
        // clear players? depends on whether DontDestroyOnLoad
    }
}
