using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Graphs.Nodes;

public delegate void NodeEvent(Node node);
public delegate void TargetNodeEvent(Node srcNode, Node tgtNode);
public delegate void GameEvent();

public class PlayerController : MonoBehaviour
{
    public MoveContext context;

    public NodeEvent OnSelectNode;
    public GameEvent OnCancel; // press escape, backspace

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
    }
}
