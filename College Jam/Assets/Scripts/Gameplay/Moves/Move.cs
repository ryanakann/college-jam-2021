using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Graphs.Nodes;

[CreateAssetMenu(menuName = "ScriptableObjects/Move")]
public class Move : ScriptableObject
{
    public new string name;

    public virtual bool Validate()
    {
        return false;
    }

    public virtual void Execute(Node node)
    {

    }
}
