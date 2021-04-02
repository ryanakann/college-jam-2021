using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Graphs.Edges;
using Graphs.Nodes;

namespace Graphs {
    public class GraphGenerator : MonoBehaviour {
        public Graph graph;
        public GameObject nodePrefab;
        public GameObject edgePrefab;
        public int nodeCount;
        public float distanceThreshold = 1f;

        protected List<GameObject> nodes;
        protected Vector3 bounds;

        protected Node player1StartNode;
        protected Node player2StartNode;
        protected float maxDistance;

        protected virtual void Start() {
            bounds = GetComponent<BoxCollider>().bounds.extents;
            GetComponent<BoxCollider>().enabled = false;
            Generate();
        }

        protected virtual void Generate() {
            nodes = new List<GameObject>();
            maxDistance = 0f;

            for (int i = 0; i < nodeCount; i++) {
                GameObject nodeObj = Instantiate(nodePrefab, transform);
                nodeObj.transform.position = SampleRandomPoint();
                nodeObj.name = $"Node {i}";
                Node node = nodeObj.GetComponent<Node>();
                graph.AddNode(node);
                if (i > 0) {//do not add edges if first node
                    //definitely add one edge to ensure connectedness
                    //choose the closest previous node to this one
                    int closestNodeIndex = 0;
                    float closestDistance = Vector3.Distance(nodes[closestNodeIndex].transform.position, nodeObj.transform.position);
                    for (int j = 0; j < nodes.Count; j++) {
                        GameObject other = nodes[j];
                        float distance = Vector3.Distance(other.transform.position, nodeObj.transform.position);
                        if (distance < closestDistance) {
                            closestDistance = distance;
                            closestNodeIndex = j;
                        }
                    }

                    for (int j = 0; j < nodes.Count; j++) {
                        GameObject other = nodes[j];
                        float distance = Vector3.Distance(other.transform.position, nodeObj.transform.position);
                        if (distance > maxDistance) {
                            player1StartNode = node;
                            player2StartNode = other.GetComponent<Node>();
                            maxDistance = distance;
                        }
                        SpringJoint joint = nodeObj.AddComponent<SpringJoint>();
                        joint.connectedBody = other.GetComponent<Rigidbody>();
                        //joint.damper = 10f;
                        joint.spring = 1 / distance;

                        if (distance < distanceThreshold || j == closestNodeIndex) {
                            GameObject edge = Instantiate(edgePrefab, transform);
                            edge.GetComponent<Edge>().SetNodes(nodeObj, other);
                            edge.name = $"Edge {i} {j}";
                            graph.AddEdge(node, other.GetComponent<Node>());
                        }
                    }
                }

                nodes.Add(nodeObj);
            }

            /*
            player1StartNode.SetOwner(1);
            player1StartNode.SetValue(1);
            player2StartNode.SetOwner(2);
            player2StartNode.SetValue(1);
            */

        }

        protected virtual void ResetGraph() {
            //UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            foreach (Transform child in transform) {
                Destroy(child.gameObject);
            }
            graph.ResetGraph();
        }

        protected virtual Vector3 SampleRandomPoint() {
            return new Vector3(Random.Range(-bounds.x, bounds.x), Random.Range(-bounds.y, bounds.y), Random.Range(-bounds.z, bounds.z));
        }

        protected virtual void Update() {
            //TODO: remove debug
            if (Input.GetKeyDown(KeyCode.R)) {
                ResetGraph();
                Generate();
            }
            if (Input.GetKeyDown(KeyCode.Space)) {
                graph.Iterate();
            }
            if (Input.GetKeyDown(KeyCode.P)) {
                foreach (var node in graph.nodes) {
                    node.Propagate();
                }
            }
            //TODO: remove debug
        }
    }
}