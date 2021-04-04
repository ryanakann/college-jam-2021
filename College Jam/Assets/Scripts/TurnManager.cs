using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Graphs.Nodes;
using Graphs;
using TMPro;

public class TurnManager : MonoBehaviour {

    [Range(10, 100)]
    public int maxTurns = 20;
    private int currentTurn = 1;

    public LinkedList<Player> players;
    public LinkedListNode<Player> currentPlayer;

    public GraphGenerator graphGenerator;

    public static TurnManager instance;

    public TMP_Text turnText;

    public int phageCounter = 0;

    public bool locked { get { return phageCounter > 0; } }

    public void Awake() {
        if (!instance) {
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
            return;
        }

        StartGame();
    }

    public void SyncPlayers() {
        players = new LinkedList<Player>(GameSettings.instance.players);
    }


    public void StartGame() {
        SyncPlayers();
        graphGenerator.Generate();
        currentPlayer = players.First;
        NextPlayer(start: true);
    }

    public List<Player> CheckGraphDomination() {
        List<Player> results = new List<Player>();
        int maxNodes = 0;
        foreach (var player in instance.players) {
            if (player.nodeCount >= maxNodes) {
                if (player.nodeCount > maxNodes)
                    results.Clear();
                results.Add(player);
                maxNodes = player.nodeCount;
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

        if (winningPlayers.Count == 1) {
            EndGame(winningPlayers[0]);
        }
        // might add faction-specific victories!
    }

    public void NextPlayer(bool start = false) {

        if (locked)
            return;

        if (players.Count <= 1)
            print("GAME OVER"); // signal that the game is over... this should be an error!

        if (!start) {
            currentPlayer.Value.EndTurn();

            if (currentPlayer.Next == null) {
                currentPlayer = players.First;
                currentTurn++;
                // update turn counters and stuff
            } else {
                currentPlayer = currentPlayer.Next;
            }
        }

        turnText.SetText($"{currentPlayer.Value.colorName}'s turn");
        currentPlayer.Value.Activate();
    }

    public void EndGame(Player winner) {
        WinMenu.instance.gameObject.SetActive(true);
        WinMenu.instance.Win(winner.colorName);
    }

    private void OnGUI() {
        if (GUI.Button(new Rect(0f, 0f, 120f, 30f), "Current player win")) {
            EndGame(currentPlayer.Value);
        }
    }
}
