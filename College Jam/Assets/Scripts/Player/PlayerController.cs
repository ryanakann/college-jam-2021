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
    }

    public void Initialize()
    {
        // get all the nodes n subscribe n stuff
    }

    public void SetContext(MoveContext context)
    {
        this.context.Clear();
        this.context = context;
        tooltip.SetText(context.GetTooltip());
    }
}
