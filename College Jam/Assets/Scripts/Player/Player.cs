using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Graphs.Nodes;
using Moves;

[System.Serializable]
public class Player {
    public Faction faction;
    public string colorName;
    public Color color;
    public bool isHuman;
    public int nodeCount;

    public List<NodeState> nodeStates;
    public List<Node> nodes;
    public List<Node> actableNodes;


    public Player(Faction faction, NamedColor namedColor, bool isHuman) {
        this.faction = faction;
        this.colorName = namedColor.name;
        this.color = namedColor.color;
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

        highlightActableNodes();
        PlayerController.instance.OnSelectNode += ValidateMoves; // we're specifically subscribing to this event
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

        PlayerController.instance.OnSelectNode -= ValidateMoves;
    }

    public void highlightActableNodes() {
        foreach (Node n in Graphs.Graph.instance.nodes) {
            n.nodeSelection.SetState(NodeSelection.NodeState.Normal);
        }
        foreach (Node n in actableNodes) {
            n.nodeSelection.SetState(NodeSelection.NodeState.Highlighted);
        }
    }
}