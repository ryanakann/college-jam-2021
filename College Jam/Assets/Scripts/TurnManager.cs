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
    public float playerTurnElapsedTime;

    public bool locked { get { return phageCounter > 0; } }

    public void Start() {
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

    private void Update()
    {
        playerTurnElapsedTime += Time.deltaTime;
    }


    public void StartGame() {
        SyncPlayers();
        graphGenerator.Generate();
        playerTurnElapsedTime = 0f;
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
            EndGame(players.First.Value);
            return;
        }

        if (currentTurn < maxTurns)
            EndGame(null);
        return;

        List<Player> winningPlayers = CheckGraphDomination();
        print("Winners:");
        foreach (var item in winningPlayers)
        {
            print(item.colorName);
        }
        if (winningPlayers.Count == 1) {
            EndGame(winningPlayers[0]);
        }
        // might add faction-specific victories!
    }

    public void NextPlayer(bool start = false) {

        if (locked)
            return;

        if (players.Count <= 1)
            EndGame(players.First.Value);

        if (!start) {
            if (playerTurnElapsedTime < NotificationMenu.instance.tweenDuration) return;
            currentPlayer.Value.EndTurn();

            if (currentPlayer.Next == null) {
                currentPlayer = players.First;
                currentTurn++;
                // update turn counters and stuff
                playerTurnElapsedTime = -NotificationMenu.instance.tweenDuration;
                NotificationMenu.instance.AddToQueue($"Turn {currentTurn}");
            } else {
                currentPlayer = currentPlayer.Next;
                playerTurnElapsedTime = 0f;
            }
        } 
        else
        {
            playerTurnElapsedTime = NotificationMenu.instance.tweenDuration;
            NotificationMenu.instance.AddToQueue($"Turn {currentTurn}");
        }
        NotificationMenu.instance.AddToQueue($"{currentPlayer.Value.colorName}'s turn");
        turnText.SetText($"{currentPlayer.Value.colorName}'s turn ({currentPlayer.Value.faction.name})");
        currentPlayer.Value.Activate();
    }

    public void EndGame(Player winner) {
        WinMenu.instance.gameObject.SetActive(true);
        if (winner == null)
        {
            WinMenu.instance.Win("Nobody"); // Draw
        }
        else
        {
            WinMenu.instance.Win(winner.colorName);
            StartCoroutine(EndGameCR(winner));
        }
    }

    private IEnumerator EndGameCR(Player winner)
    {
        CameraPivot.instance.EndGame();

        int winnerIndex = GameSettings.instance.players.IndexOf(winner);
        List<Node> loserNodes = new List<Node>();
        foreach (var node in Graph.instance.nodes)
        {
            if (node.owner != winnerIndex)
            {
                loserNodes.Add(node);
            }
        }

        for (int i = 0; i < loserNodes.Count; i++)
        {
            yield return new WaitForSeconds(0.4f);
            loserNodes[i].SetOwner(winnerIndex);
        }
    }

    private void OnGUI() {
        if (GUI.Button(new Rect(0f, 0f, 120f, 30f), "Current player win")) {
            EndGame(currentPlayer.Value);
        }
    }
}
