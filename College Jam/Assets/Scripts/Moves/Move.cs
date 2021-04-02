using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Graphs.Nodes;

public class Move : ScriptableObject
{
    // can this move be executed from this node?
    public virtual bool Validate(Node node)
    {
        return true;
    }

    // execute this move from this node.
    public virtual void Execute(Node node)
    {

    }
}

[CreateAssetMenu(fileName = "Propagate", menuName = "ScriptableObjects/Move/Propagate", order = 1)]
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
        foreach (var neighbor in node.neighbors) {
            node.value--;
            // neighbor.SendPhage(1, node.faction) // Phage Count, owned faction
        }
    }
}

[CreateAssetMenu(fileName = "Split", menuName = "ScriptableObjects/Move/Split", order = 1)]
public class Split : Move
{
    public override bool Validate(Node node)
    {
        base.Validate(node);

        return node.value > 1;
    }

    public override void Execute(Node node)
    {
        base.Execute(node);

        PlayerController.instance.SetContext(new AdjacentSelectContext(node));
        ((AdjacentSelectContext)PlayerController.instance.context).OnSelect += FinalExecute;
    }

    public void FinalExecute(Node srcNode, Node tgtNode)
    {
        srcNode.value /= 2;
        // tgtNode.SendPhage((int)(srcNode.phageCount / 2), srcNode.faction)
    }
}
