using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Graphs.Nodes;

public class MoveContext
{
    public virtual void Clear()
    {

    }

    public virtual string GetTooltip()
    {
        return "Click on a node to see move options.";
    }
}

// this context involves forcing the controller to click on a node adjacent to the provided node
public class AdjacentSelectContext : MoveContext
{
    Node srcNode;
    public TargetNodeEvent OnSelect;

    public AdjacentSelectContext(Node node)
    {
        srcNode = node;
        PlayerController.instance.OnSelectNode += ValidateSelection;
        PlayerController.instance.OnCancel += Clear;
        // highlight valid nodes
    }

    public override void Clear()
    {
        base.Clear();
        PlayerController.instance.OnSelectNode -= ValidateSelection;
        PlayerController.instance.OnCancel -= Clear;
        // clear highlights
    }

    void ValidateSelection(Node node)
    {
        if (srcNode.neighbors.Contains(node))
        {
            OnSelect?.Invoke(srcNode, node);
        }
        else
        {
            Clear();
        }
    }

    public override string GetTooltip()
    {
        return "Click on an adjacent node to confirm your target. Press escape to cancel.";
    }
}