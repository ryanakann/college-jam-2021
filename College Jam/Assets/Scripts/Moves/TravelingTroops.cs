using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Graphs.Nodes;

public class TravelingTroops : MonoBehaviour {
    public Faction faction;
    int numPhages;
    public int NumPhages {
        get { return numPhages; }
    }

    public List<Node> path;
    //int pathStep = 0;
}
