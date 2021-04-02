using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Graphs.Nodes;

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
