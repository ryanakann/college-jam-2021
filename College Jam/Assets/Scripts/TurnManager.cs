using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move {
    string name;
}
public class MoveSet {
    List<Move> moves;
}
public class Faction {
    MoveSet moveSet;

    public int nodes; // temp var for win condition checking
}


public class Player
{
    public Faction faction;

    public virtual void Activate()
    {
        // tell the controller which player is going
    }

    public void EndTurn()
    {
        TurnManager.instance.NextPlayer();
    }
}


public class TurnManager : MonoBehaviour {

    int maxTurns = 20, currentTurn = 1;


    public LinkedList<Player> players;
    public LinkedListNode<Player> currentPlayer;


    public static TurnManager instance;

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


    public void StartGame()
    {
        currentPlayer = players.First;
    }

    public List<Player> CheckGraphDomination()
    {
        List<Player> results = new List<Player>();
        int maxNodes = 0;
        foreach (var player in instance.players)
        {
            if (player.faction.nodes >= maxNodes)
            {
                if (player.faction.nodes > maxNodes)
                    results = new List<Player>();
                results.Add(player);
                maxNodes = player.faction.nodes;
            }
        }
        return results;
    }

    // every move triggers a check for win conditions
    public void CheckWinConditions()
    {
        if (players.Count == 1)
        {
            // players.First.Value wins!
            return;
        }

        if (currentTurn < maxTurns)
            return;

        List<Player> winningPlayers = CheckGraphDomination();
        
        switch (winningPlayers.Count)
        {
            case 0:
                // No one wins!
                break;
            case 1:
                // winningPlayers[0] wins!
                break;
            default:
                // winningPlayers win!
                break;
        }

        // might add faction-specific victories!
    }

    public void NextPlayer()
    {
        if (players.Count <= 1)
            print("GAME OVER"); // signal that the game is over... this should be an error!


        if (currentPlayer.Next == null)
        {
            currentPlayer = players.First;
            currentTurn++;
            // update turn counters and stuff
        }
        else
            currentPlayer = currentPlayer.Next;

        currentPlayer.Value.Activate();
    }
}
