using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Graphs.Nodes;
using Graphs;
using TMPro;
using Moves;

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
        {
            EndGame(null);
            return;
        }

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

    public void AIMovement()
    {
        StartCoroutine(AIMovementCR());
    }

    IEnumerator AIMovementCR()
    {
        // Get a list of nodes the current player can perform actions on and randomly permute it
        List<Node> actableNodes = currentPlayer.Value.actableNodes;
        print("Actable node count: " + actableNodes.Count);
        System.Random rng = new System.Random();
        int n = actableNodes.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            var value = actableNodes[k];
            actableNodes[k] = actableNodes[n];
            actableNodes[n] = value;
        }

        // For each node you can perform an action on, execute a random action
        //foreach (var node in actableNodes)
        for (int i = 0; i < actableNodes.Count; i++)
        {
            Node node = actableNodes[i];
            // Wait for the graph to be unlocked before continuing
            while (locked)
            {
                yield return new WaitForEndOfFrame();
            }

            // Get a list of possible actions to take for this node
            List<(Move, bool, string)> validMoves = currentPlayer.Value.ValidateMoves(node);
            List<Move> options = new List<Move>();
            foreach (var item in validMoves)
            {
                Move itemMove = item.Item1;
                bool itemValid = item.Item2;
                if (itemValid)
                {
                    options.Add(itemMove);
                }
            }

            if (options.Count <= 0) continue;

            // Choose a random move out of the options available
            Move move = options[Random.Range(0, options.Count)];
            // Execute the move in accordance with its type
            if (move.GetType().IsEquivalentTo(typeof(Split)))
            {
                print("Splitting!");
                Split splitMove = (Split)move;
                Node targetNode = node.neighbors[Random.Range(0, node.neighbors.Count)];
                splitMove.Execute(node);
                splitMove.FinalExecute(node, targetNode);
            }
            else if (move.GetType().IsEquivalentTo(typeof(Propagate)))
            {
                print("Propagating!");
                move.Execute(node);
            }
            else if (move.GetType().IsEquivalentTo(typeof(Fortify)))
            {
                print("Fortifying!");
                move.Execute(node);
            }
        }
        
        // Wait for the last move to finish executing before continuing
        while (locked)
        {
            yield return new WaitForEndOfFrame();
        }
        currentPlayer.Value.EndTurn();
        NextPlayer();

        //List<(Node, Move)> moves = new List<(Node, Move)>();
        //foreach (var node in currentPlayer.Value.actableNodes)
        //{
        //    List<(Move, bool, string)> validMoves = currentPlayer.Value.ValidateMoves(node);
        //    List<Move> options = new List<Move>();
        //    foreach (var item in validMoves)
        //    {
        //        Move move = item.Item1;
        //        bool valid = item.Item2;
        //        if (valid)
        //        {
        //            options.Add(move);
        //        }
        //    }
        //    moves.Add((node, options[Random.Range(0, options.Count)]));
        //}

        //// Randomly permute list
        //System.Random rng = new System.Random();
        //int n = nodeMoves.Count;
        //while (n > 1)
        //{
        //    n--;
        //    int k = rng.Next(n + 1);
        //    var value = nodeMoves[k];
        //    nodeMoves[k] = nodeMoves[n];
        //    nodeMoves[n] = value;
        //}

        //foreach (var nodeMove in nodeMoves)
        //{
        //    Node node = nodeMove.Item1;
        //    Move move = nodeMove.Item2;

        //    // Ignore nodes that are currently fortifying. Probably unneeded check
        //    if (node.fortifying > 0) continue;

        //    if (move.GetType().IsEquivalentTo(typeof(Split)))
        //    {
        //        Split splitMove = (Split)move;
        //        Node targetNode = node.neighbors[Random.Range(0, node.neighbors.Count)];
        //        splitMove.FinalExecute(node, targetNode);
        //    }
        //    else if (move.GetType().IsEquivalentTo(typeof(Propagate)))
        //    {
        //        move.Execute(node);
        //    }
        //    else if (move.GetType().IsEquivalentTo(typeof(Fortify)))
        //    {
        //        move.Execute(node);
        //    }
        //    yield return new WaitForSeconds(3f);
        //}

        //NextPlayer();
    }
}
