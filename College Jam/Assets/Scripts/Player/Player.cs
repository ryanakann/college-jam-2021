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
    public HashSet<Node> nodes;
    public List<Node> actableNodes;


    public Player(Faction faction, NamedColor namedColor, bool isHuman) {
        this.faction = faction;
        this.colorName = namedColor.name;
        this.color = namedColor.color;
        this.isHuman = isHuman;
        nodeCount = 0;

        nodeStates = new List<NodeState>();
        nodes = new HashSet<Node>();
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

        PlayerController.instance.UpdateToolTip(null);

        highlightActableNodes();
        PlayerController.instance.OnSelectNode += ValidateMovesVoid; // we're specifically subscribing to this event

        if (actableNodes.Count > 0)
            CameraPivot.instance.SetTarget(actableNodes[Random.Range(0, actableNodes.Count)].transform);

        if (!isHuman)
        {
            TurnManager.instance.AIMovement();
        }
    }

    public void ValidateMovesVoid(Node node)
    {
        ValidateMoves(node);
    }

    public List<(Move, bool, string)> ValidateMoves(Node node) {
        List<(Move, bool, string)> validMoves = new List<(Move, bool, string)>();
        foreach (Move move in faction.moveSet) {
            (bool allowed, string description) = move.Validate(node);
            validMoves.Add((move, allowed, description));
        }

        node.nodeUI.PopulateUI(validMoves);
        return validMoves;
    }

    public void EndTurn() {
        for (int i = nodeStates.Count - 1; i >= 0; i--) {
            if (nodeStates[i].inactive) {
                nodeStates.RemoveAt(i);
                continue;
            }
            nodeStates[i].PostActivate();
        }

        foreach (var node in nodes)
        {
            node.IncrementValue();
        }

        PlayerController.instance.OnSelectNode -= ValidateMovesVoid;
    }

    public void highlightActableNodes() {

        foreach (Node n in Graphs.Graph.instance.nodes) {
            n.nodeUI.SetDetailVisibility(false);
            n.nodeSelection.SetState(NodeSelection.NodeState.Normal);
        }
        foreach (Node n in actableNodes) {
            bool highlight = true;
            foreach (NodeState state in n.nodeStates)
            {
                if (state.blocking)
                    highlight = false;
            }
            if (highlight)
                n.nodeSelection.SetState(NodeSelection.NodeState.Highlighted);
        }
    }
}