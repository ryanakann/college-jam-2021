using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Graphs.Nodes;
using TMPro;


public delegate void TargetNodeEvent(Node srcNode, Node tgtNode);
public delegate void GameEvent();

public class PlayerController : MonoBehaviour {
    public MoveContext context;

    public NodeEvent OnSelectNode; // clicking on a node which you own while you aren't in a context that expects you to click on a node
    public NodeEvent OnClickNode; // clicking on any old node, even if you don't own it
    public GameEvent OnCancel; // press escape, backspace

    public TMP_Text tooltip;

    [HideInInspector]
    public string defaultToolTip = "Click on a highlighted node to select a move.";

    [HideInInspector]
    public string toolTipSelectText = "Select a move from the list.";

    public static PlayerController instance;
    public void Awake() {
        if (!instance) {
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
            return;
        }

        tooltip.text = defaultToolTip;
    }

    public void Clear() {
        if (context != null)
            context.Clear();
        context = null;
        UpdateToolTip(null);
    }

    public void SetContext(MoveContext context) {
        if (this.context != null)
            this.context.Clear();
        this.context = context;
        this.context.Initialize();
        tooltip.SetText(context.GetTooltip());
    }

    public void HandleClickNode(Node node) {
        OnClickNode?.Invoke(node);
        if (context == null) {
            CameraPivot.instance?.SetTarget(node.transform);
            node.nodeSelection.OnSelect?.Invoke(node);
            OnSelectNode?.Invoke(node);
            UpdateToolTip(node);

        } else if (context.clearing) {
            Clear();
        }
    }

    public void UpdateToolTip(Node node)
    {
        int count = 0; // sorry for this
        foreach (Node n in TurnManager.instance.currentPlayer.Value.actableNodes) {
            bool blocking = false;
            foreach (NodeState state in n.nodeStates) {
                if (state.blocking) {
                    blocking = true;
                    continue;
                }
            }
            count += (blocking) ? 0 : 1;
        }
        if (count == 0)
        {
            instance.tooltip.text = "No active nodes remain.";
        }
        else if (node != null && TurnManager.instance.currentPlayer.Value.actableNodes.Contains(node))
        {
            tooltip.text = toolTipSelectText;
        }
        else
        {
            tooltip.text = defaultToolTip;
        }
    }

    public void HandleMoveNode(Node node) {
        TurnManager.instance.currentPlayer.Value.actableNodes.Remove(node);
        UpdateToolTip(null);
    }
}
