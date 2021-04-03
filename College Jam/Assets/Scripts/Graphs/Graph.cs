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

        private void AddDirectedEdge(Node a, Node b) {
            a.AddNeighbor(b);
            if (edges.ContainsKey(a)) {
                if (!edges[a].Contains(b)) {
                    edges[a].Add(b);
                }
            } else {
                edges.Add(a, new List<Node>() { b });
            }


        }

        public void AddEdge(Node a, Node b) {
            AddDirectedEdge(a, b);
            AddDirectedEdge(b, a);
        }

        public void SetNodeActive(Node node) {
            activeNode?.nodeSelection.SetState(NodeSelection.NodeState.Normal);
            activeNode = node;

            if (activeNode.owner == 1) {
                activeNode.nodeUI.SetDetailVisibility(true);
            }
            CameraPivot.instance?.SetTarget(node.transform);
        }

        public static List<Node> GetImmediateNeighbors(Node start) {
            return start.neighbors;
        }
        public List<Node> BFSOrder(Node start) {
            return BFSOrderGeneral(start, GetImmediateNeighbors);
        }

        public List<Node> BFSOrderGeneral(Node start, Func<Node, List<Node>> getNeighbors) {
            List<Node> results = new List<Node>();
            Queue<Node> q = new Queue<Node>();
            q.Enqueue(start);
            while (q.Count > 0) {
                Node current = q.Dequeue();
                foreach (Node n in getNeighbors(current)) {
                    if (!results.Contains(n)) {
                        q.Enqueue(n);
                        if (n != start) {
                            results.Add(n);
                        }
                    }
                }
            }
            return results;
        }

        public List<Node> ShortestPath(Node start, Node finish) {
            return ShortestPathGeneral(start, finish, GetImmediateNeighbors);
        }

        public List<Node> ShortestPathGeneral(Node start, Node finish, Func<Node, List<Node>> getNeighbors) {
            bool reachable = false;
            Dictionary<Node, Node> edgesTaken = new Dictionary<Node, Node>();
            Queue<Node> q = new Queue<Node>();
            q.Enqueue(start);
            while (q.Count > 0) {
                Node current = q.Dequeue();
                foreach (Node n in getNeighbors(current)) {
                    if (!edgesTaken.ContainsKey(n)) {
                        q.Enqueue(n);
                        if (n != start) {
                            edgesTaken.Add(n, current);
                        }
                        if (n == finish) {
                            reachable = true;
                            break;
                        }
                    }
                }
            }
            List<Node> path = new List<Node>();
            if (!reachable) {
                return path;
            }
            Node currentNode = finish;
            path.Add(finish);
            while (currentNode != start) {
                currentNode = edgesTaken[currentNode];
                path.Add(currentNode);
            }
            path.Reverse();
            return path;
        }

        public void ResetGraph() {
            activeNode = null;
            nodes.Clear();
            edges.Clear();
        }

        public void Iterate() {
            foreach (var node in nodes) {
                node.NextTurn();
            }
        }

        public Vector3 Center() {
            Vector3 center = Vector3.zero;
            foreach (Node node in nodes) {
                center += node.transform.position;
            }
            center /= nodes.Count;
            return center;
        }
    }
}
