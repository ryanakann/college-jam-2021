using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Graphs.Edges;
using Graphs.Nodes;

namespace Graphs {
    public class Graph : MonoBehaviour {
        public static Graph instance;

        public Node activeNode;
        public List<Node> nodes;
        public Dictionary<Node, List<Node>> edges;

        public GameObject PhagePrefab;
        public GameObject PhageTarget;

        void Awake() {
            instance = this;
            nodes = new List<Node>();
            edges = new Dictionary<Node, List<Node>>();
        }

        public void AddNode(Node node) {
            if (!nodes.Contains(node)) {
                nodes.Add(node);
                if (node.nodeSelection)
                    node.nodeSelection.OnSelect += SetNodeActive;
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
            activeNode?.nodeUI.SetDetailVisibility(false);
            activeNode = node;
            if (activeNode.owner > -1 && TurnManager.instance.currentPlayer.Value.actableNodes.Contains(node) && !TurnManager.instance.locked) {
                activeNode.nodeSelection.SetState(NodeSelection.NodeState.Selected);
                activeNode.nodeUI.SetDetailVisibility(true);
            }
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

        //unconditionally play animation for sending phages from start to finish
        public void SendPhages(Node start, Node finish, int numPhages) {
            SendPhages(start, finish, numPhages, start.owner);
        }

        public void SendPhages(Node start, Node finish, int numPhages, int phageOwner) {
            for (int i = 0; i < numPhages; i++) {
                StartCoroutine(SendPhage(start, finish, i, phageOwner));
            }
        }

        public IEnumerator SendPhage(Node start, Node finish, int id, int phageOwner) {
            yield return new WaitForSeconds(UnityEngine.Random.Range(0f, 1.5f));

            start.SetValue(start.value - 1);

            //give phage the right info
            GameObject phageObj = Instantiate(PhagePrefab, start.transform.position, Quaternion.identity, transform);
            phageObj.name = $"Phage {id}";
            phageObj.GetComponent<CollideWithTarget>().owner = phageOwner;
            phageObj.GetComponent<CollideWithTarget>().target = finish;

            //start animation
            GameObject phageAnimTarget = Instantiate(PhageTarget, start.transform.position, Quaternion.identity, transform);
            phageAnimTarget.name = $"Phage Target {id}";
            phageAnimTarget.GetComponent<TravelBetween>().from = start.transform;
            phageAnimTarget.GetComponent<TravelBetween>().to = finish.transform;

            ParticleSystem.MainModule mainModule = phageObj.GetComponent<ParticleSystem>().main;
            mainModule.startColor = GameSettings.instance.players[phageOwner].color;
            phageObj.transform.position = start.transform.position + UnityEngine.Random.onUnitSphere * 0.2f;
            phageObj.GetComponent<OrbitTarget>().target = phageAnimTarget.transform;
        }

        public void ResetGraph() {
            activeNode = null;
            nodes.Clear();
            edges.Clear();
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
