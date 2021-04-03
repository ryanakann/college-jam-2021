using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Graphs.Nodes;
using Moves;

[System.Serializable]
public class Player
{
    public Faction faction;
    public Color color;
    public bool isHuman;
    public int nodeCount;

    public List<NodeState> nodeStates;
    public List<Node> nodes;
    public List<Node> actableNodes; 


    public Player(Faction faction, Color color, bool isHuman)
    {
        this.faction = faction;
        this.color = color;
        this.isHuman = isHuman;
        nodeCount = 0;
    }

    public virtual void Activate()
    {
        actableNodes = new List<Node>(nodes); // shallow copy

        for (int i = nodeStates.Count - 1; i >= 0; i--)
        {
            if (nodeStates[i].inactive)
            {
                nodeStates.RemoveAt(i);
                continue;
            }
            nodeStates[i].PreActivate();
        }

        if (isHuman)
            PlayerController.instance.OnSelectNode += ValidateMoves;
        else
        {
            // do AI stuff...
        }
    }

    public void ValidateMoves(Node node)
    {
        Dictionary<Move, bool> validMoves = new Dictionary<Move, bool>();
        foreach (Move move in faction.moveSet)
            validMoves[move] = move.Validate(node);

        // populate Node selection ui with validated (and invalidated) moves
    }

    public void EndTurn()
    {
        for (int i = nodeStates.Count - 1; i >= 0; i--)
        {
            if (nodeStates[i].inactive)
            {
                nodeStates.RemoveAt(i);
                continue;
            }
            nodeStates[i].PostActivate();
        }
    }
}