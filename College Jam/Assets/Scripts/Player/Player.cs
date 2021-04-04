using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Graphs.Nodes;
using Moves;

[System.Serializable]
public class Player {
    public Faction faction;
    public Color color;
    public bool isHuman;
    public int nodeCount;

    public List<NodeState> nodeStates;
    public List<Node> nodes;
    public List<Node> actableNodes;


    public Player(Faction faction, Color color, bool isHuman) {
        this.faction = faction;
        this.color = color;
        this.isHuman = isHuman;
        nodeCount = 0;

        nodeStates = new List<NodeState>();
        nodes = new List<Node>();
        actableNodes = new List<Node>();
    }

    public virtual void Activate() {
        actableNodes = new List<Node>(nodes); // shallow copy

        for (int i = nodeStates.Count - 1; i >= 0; i--) {
            if (nodeStates[i].inactive) {
                nodeStates.RemoveAt(i);
                continue;
            }
            nodeStates[i].PreActivate();
        }
        foreach (Node n in nodes) {
            n.nodeSelection.OnSelect += ValidateMoves;
        }

        //if (isHuman)
        //    PlayerController.instance.OnSelectNode += ValidateMoves;
        //else
        //{
        //    // do AI stuff...
        //}
    }

    public virtual void Deactivate() {
        foreach (var node in actableNodes) {
            node.nodeSelection.OnSelect -= ValidateMoves;
        }
    }

    public void ValidateMoves(Node node) {
        List<(Move, bool, string)> validMoves = new List<(Move, bool, string)>();
        foreach (Move move in faction.moveSet) {
            (bool allowed, string description) = move.Validate(node);
            validMoves.Add((move, allowed, description));
        }

        node.nodeUI.PopulateUI(validMoves);
    }

    public void EndTurn() {
        for (int i = nodeStates.Count - 1; i >= 0; i--) {
            if (nodeStates[i].inactive) {
                nodeStates.RemoveAt(i);
                continue;
            }
            nodeStates[i].PostActivate();
        }
    }
}