using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Graphs.Edges;
using Graphs.Nodes;

namespace Graphs
{
    public class Graph : MonoBehaviour
    {
        List<Node> nodes;
        Dictionary<Node, List<Node>> neighborMap;

        void Awake()
        {
            nodes = new List<Node>();
            neighborMap = new Dictionary<Node, List<Node>>();
        }
        
        public void AddNode(Node node)
        {
            if (!nodes.Contains(node))
            {
                nodes.Add(node);
                node.nodeSelection.OnSelect?.AddListener(SetNodeActive);
            }
        }

        public void AddNeighbor(Node node, Node neighbor)
        {
            if (neighborMap.ContainsKey(node))
            {
                if (!neighborMap[node].Contains(neighbor))
                {
                    neighborMap[node].Add(neighbor);
                }
            }

            if (neighborMap.ContainsKey(neighbor))
            {
                if (!neighborMap[neighbor].Contains(node))
                {
                    neighborMap[neighbor].Add(node);
                }
            }
        }

        public void SetNodeActive(Node node)
        {
            CameraPivot.instance.SetTarget(node.transform);
        }
    }
}