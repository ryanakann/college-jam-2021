using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Graphs.Nodes;
//public class Faction {
//    List<ScriptableObject> moves; //things this faction can do in a turn
//    //TODO: List<Move> moveSet;

//    public List<Node> nodes; //all nodes controlled by this faction

//    public int numNodes; // temp var for win condition checking
//}


//public class Player {
//    public Faction faction;

//    public virtual void Activate() {
//        //TODO: this goes in the Controller class
//        foreach (Node n in faction.nodes) {
//            //if node has stuff to do at the beginning of the turn, do it
//            //fortify check goes here
//        }

//        //actableNodes = new List<Node>(faction.nodes); //this is separate because taking over a node should not give you more moves
//        //while(Player wants to do stuff){
//        //   click on a node
//        //   node figures out what moves can do
//        //   you click one of them
//        //   execute action RIGHT NOW
//        //   if(action results in losing a node){
//        //     remove that node from actableNodes
//        //   }
//        //   remove node from actableNodes
//        //}

//        foreach (Node n in faction.nodes) {
//            //if node has stuff to do at the end of the turn, do it
//            //troop movement goes here
//        }
//    }
//}

public class TurnManager : MonoBehaviour {

    [Range(10, 100)]
    public int maxTurns = 20;
    private int currentTurn = 1;

    public LinkedList<Player> players;
    public LinkedListNode<Player> currentPlayer;

    public static TurnManager instance;

    public void Awake() {
        if (!instance) {
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
            return;
        }
    }

    public void SyncPlayers()
    {
        players = new LinkedList<Player>(GameSettings.instance.players);
    }


    public void StartGame() {
        SyncPlayers();
        currentPlayer = players.First;
    }

    public List<Player> CheckGraphDomination() {
        List<Player> results = new List<Player>();
        int maxNodes = 0;
        foreach (var player in instance.players)
        {
            if (player.nodes >= maxNodes)
            {
                if (player.nodes > maxNodes)
                    results.Clear();
                results.Add(player);
                maxNodes = player.nodes;
            }
        }
        return results;
    }

    // every move triggers a check for win conditions
    public void CheckWinConditions() {
        if (players.Count == 1) {
            // players.First.Value wins!
            return;
        }

        if (currentTurn < maxTurns)
            return;

        List<Player> winningPlayers = CheckGraphDomination();

        switch (winningPlayers.Count) {
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

    public void NextPlayer() {
        if (players.Count <= 1)
            print("GAME OVER"); // signal that the game is over... this should be an error!


        if (currentPlayer.Next == null) {
            currentPlayer = players.First;
            currentTurn++;
            // update turn counters and stuff
        } else
            currentPlayer = currentPlayer.Next;

        currentPlayer.Value.Activate();
    }
}
