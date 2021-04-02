using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Graphs.Nodes;

public class Propagate : Move
{

    public override bool Validate(Node node)
    {
        base.Validate(node);

        return node.value > node.neighbors.Count;
    }

    public override void Execute(Node node)
    {
        base.Execute(node);
        foreach (var neighbor in node.neighbors)
        {
            node.value--;
            // neighbor.SendPhage(1, node.faction) // Phage Count, owned faction
        }
    }
}