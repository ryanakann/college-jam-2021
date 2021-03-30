using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Graphs
{
    namespace Nodes
    {
        public class Node : MonoBehaviour
        {
            public List<Node> neighbors;

            public NodeSelection nodeSelection;
            public NodePhysics nodePhysics;
            public NodeUI nodeUI;

            private void Awake()
            {
                neighbors = new List<Node>();
            }

            public void SetOwner(int playerNum)
            {
                if (playerNum == 0)
                {
                    nodeSelection.mat.SetColor("_Color", new Color(177f / 255f, 255f / 255f, 125f / 255f));
                }
                else if (playerNum == 1)
                {
                    nodeSelection.mat.SetColor("_Color", new Color(219f / 255f, 88f / 255f, 224f / 255f));
                }
            }

            public void AddNeighbor (Node neighbor)
            {
                if (!neighbors.Contains(neighbor))
                {
                    neighbors.Add(neighbor);
                }
            }
        }
    }
}