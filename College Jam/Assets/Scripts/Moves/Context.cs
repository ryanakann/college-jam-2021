using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Graphs.Nodes;
using Moves;

public class MoveContext {

    public bool clearing;

    public virtual void Clear() {

    }

    public virtual void Initialize() {

    }

    public virtual string GetTooltip() {
        return "Click on a node to see move options.";
    }
}

// this context involves forcing the controller to click on a node adjacent to the provided node
public class AdjacentSelectContext : MoveContext {
    Node srcNode;
    Move move;
    public TargetNodeEvent OnSelect;
    List<Node> highlightedNodes;

    public AdjacentSelectContext(Node node, Move move) {
        srcNode = node;
        this.move = move;
        PlayerController.instance.OnClickNode += ValidateSelection;
        PlayerController.instance.OnCancel += Clear;
        clearing = true;
    }

    public override void Initialize() {
        base.Initialize();
        foreach (Node n in srcNode.neighbors)
            n.nodeSelection.SetState(NodeSelection.NodeState.Highlighted);
        // highlight valid nodes
    }

    public override void Clear() {
        base.Clear();
        PlayerController.instance.OnClickNode -= ValidateSelection;
        PlayerController.instance.OnCancel -= Clear;
        // clear highlights
        TurnManager.instance.currentPlayer.Value.highlightActableNodes();
    }

    void ValidateSelection(Node node) {
        if (srcNode.neighbors.Contains(node)) {
            foreach (var check in move.validationChecks) {
                (bool valid, string msg) = check.Validate(srcNode, node);
                if (!valid) {
                    Debug.Log(msg);//TODO: where do I put this?
                    PlayerController.instance.Clear();
                    return;
                }
            }
            //execute the move
            OnSelect?.Invoke(srcNode, node);
        } else {
            PlayerController.instance.Clear();
        }
    }

    public override string GetTooltip() {
        return "Click on an adjacent node to confirm your target. Press escape to cancel.";
    }
}