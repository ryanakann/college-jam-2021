using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Graphs.Nodes;

public class MoveContext
{

    public bool clearing;

    public virtual void Clear()
    {

    }

    public virtual void Initialize()
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
    List<Node> highlightedNodes;

    public AdjacentSelectContext(Node node)
    {
        srcNode = node;
        PlayerController.instance.OnClickNode += ValidateSelection;
        PlayerController.instance.OnCancel += Clear;
        clearing = true;
    }

    public override void Initialize()
    {
        base.Initialize();
        foreach (Node n in srcNode.neighbors)
            n.nodeSelection.SetState(NodeSelection.NodeState.Highlighted);
        // highlight valid nodes
    }

    public override void Clear()
    {
        base.Clear();
        PlayerController.instance.OnClickNode -= ValidateSelection;
        PlayerController.instance.OnCancel -= Clear;
        // clear highlights
        foreach (Node n in srcNode.neighbors)
            n.nodeSelection.SetState(NodeSelection.NodeState.Normal);
    }

    void ValidateSelection(Node node)
    {
        if (srcNode.neighbors.Contains(node))
        {
            OnSelect?.Invoke(srcNode, node);
        }
        //if (PlayerController.instance.context == this)
          //  PlayerController.instance.Clear();
    }

    public override string GetTooltip()
    {
        return "Click on an adjacent node to confirm your target. Press escape to cancel.";
    }
}