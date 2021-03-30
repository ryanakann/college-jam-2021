using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Graphs.Edges;
using Graphs.Nodes;

namespace Graphs {
    public class Graph : MonoBehaviour {
        public Node activeNode;
        public List<Node> nodes;
        public Dictionary<Node, List<Node>> edges;

        void Awake() {
            nodes = new List<Node>();
            edges = new Dictionary<Node, List<Node>>();
        }

        public void AddNode(Node node) {
            if (!nodes.Contains(node)) {
                nodes.Add(node);
                node.nodeSelection.OnSelect?.AddListener(SetNodeActive);
            }
        }

        public void AddEdge(Node a, Node b) {
            if (edges.ContainsKey(a)) {
                if (!edges[a].Contains(b)) {
                    edges[a].Add(b);
                    a.AddNeighbor(b);
                }
            }

            if (edges.ContainsKey(b)) {
                if (!edges[b].Contains(a)) {
                    edges[b].Add(a);
                    b.AddNeighbor(a);
                }
            }
        }

        public void SetNodeActive(Node node) {
            activeNode?.nodeSelection.SetState(NodeSelection.MouseState.Normal);
            activeNode = node;

            if (activeNode.owner == 1)
            {
                activeNode.nodeUI.SetDetailVisibility(true);
            }
            CameraPivot.instance.SetTarget(node.transform);
        }

        public static List<Node> GetImmediateNeighbors(Node start) {
            return start.neighbors;
        }

        public List<Node> BFSOrder(Node start, Func<Node, List<Node>> getNeighbors) {
            List<Node> results = new List<Node>();
            //TODO: BFS using getNeighbors
            return results;
        }

        public static List<Node> ShortestPath(Node start, Node finish, Func<Node, List<Node>> getNeighbors) {
            List<Node> path = new List<Node>();
            //TODO: get result of BFSOrder(start, getNeighbors)
            //if finish not in BFSOrder, return empty
            //else, construct path from BFSOrder
            return path;
        }
    }
}
