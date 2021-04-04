using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Graphs.Nodes;
using TMPro;


public delegate void TargetNodeEvent(Node srcNode, Node tgtNode);
public delegate void GameEvent();

public class PlayerController : MonoBehaviour
{
    public MoveContext context;

    public NodeEvent OnSelectNode; // clicking on a node which you own while you aren't in a context that expects you to click on a node
    public NodeEvent OnClickNode; // clicking on any old node, even if you don't own it
    public GameEvent OnCancel; // press escape, backspace

    public TMP_Text tooltip;

    string defaultToolTip = "Click on a highlighted node to select a move.", toolTipSelectText = "Select a move from the list.";

    public static PlayerController instance;
    public void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        tooltip.text = defaultToolTip;
    }

    public void Clear()
    {
        if (context != null)
            context.Clear();
        context = null;
        tooltip.SetText(defaultToolTip);
    }

    public void SetContext(MoveContext context)
    {
        if (this.context != null)
            this.context.Clear();
        this.context = context;
        this.context.Initialize();
        tooltip.SetText(context.GetTooltip());
    }

    public void HandleClickNode(Node node)
    {
        print("GOO GOOO");
        OnClickNode?.Invoke(node);
        if (context == null)
        {
            tooltip.text = toolTipSelectText;
            if (TurnManager.instance.currentPlayer.Value.actableNodes.Contains(node))
                OnSelectNode?.Invoke(node);
            node.nodeSelection.OnSelect?.Invoke(node);
        }
    }
}
