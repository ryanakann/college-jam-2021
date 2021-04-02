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


