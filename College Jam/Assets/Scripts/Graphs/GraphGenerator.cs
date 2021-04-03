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
        protected int nodeCount;
        public float distanceThreshold = 1f;

        protected List<GameObject> nodes;
        protected Vector3 bounds;

        protected int playerCount;
        protected List<Vector3> startPoints;
        protected List<Node> startNodes;

        protected virtual void Start() {
            bounds = GetComponent<BoxCollider>().bounds.extents;
            GetComponent<BoxCollider>().enabled = false;
            nodeCount = GameSettings.instance.mapSize.nodeCount;
            playerCount = GameSettings.instance.playerCount;
            startPoints = new List<Vector3>();
            startNodes = new List<Node>();
            Generate();
        }

        // Get (1-3)-simplex equidistant points
        protected virtual List<Vector3> GetStartPoints()
        {
            List<Vector3> points = new List<Vector3>();
            if (playerCount < 2 || playerCount > 4) return points;
            
            // Randomly rotate the points for variety
            Vector3 axis = Random.onUnitSphere;
            float rotation = Random.value * 360f;

            float distanceFromOrigin = 100f;

            // Line
            if (playerCount == 2)
            {
                Vector3 p1 = Vector3.left;
                Vector3 p2 = Vector3.right;
                points.Add(p1);
                points.Add(p2);
            }
            //Triangle
            else if (playerCount == 3)
            {
                Vector3 p1 = new Vector3(1f, -1 / Mathf.Sqrt(3), 0f);
                Vector3 p2 = new Vector3(-1f, -1 / Mathf.Sqrt(3), 0f);
                Vector3 p3 = new Vector3(0f, 2 / Mathf.Sqrt(3), 0f);
            }
            // Tetrahedron
            else if (playerCount == 4)
            {
                Vector3 p1 = new Vector3(1f, -1 / Mathf.Sqrt(3), -1 / Mathf.Sqrt(6));
                Vector3 p2 = new Vector3(-1f, -1 / Mathf.Sqrt(3), -1 / Mathf.Sqrt(6));
                Vector3 p3 = new Vector3(0f, 2 / Mathf.Sqrt(3), -1 / Mathf.Sqrt(6));
                Vector3 p4 = new Vector3(0f, 0f, 3 / Mathf.Sqrt(3));
                points.Add(p1);
                points.Add(p2);
                points.Add(p3);
                points.Add(p4);
            }

            Transform reference = new GameObject("Reference").transform;
            for (int i = 0; i < points.Count; i++)
            {
                reference.position = points[i];
                reference.RotateAround(Vector3.zero, axis, rotation);
                points[i] = reference.position;
                points[i] = points[i].normalized * distanceFromOrigin;
            }
            Destroy(reference.gameObject);
            return points;
        }

        protected virtual void Generate() {
            nodes = new List<GameObject>();
            startPoints = GetStartPoints();

            // Init start nodes to playerCount nulls
            startNodes = new List<Node>();
            for (int i = 0; i < playerCount; i++)
            {
                startNodes.Add(null);
            }

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

                    // Add edges and physics between nodes
                    for (int j = 0; j < nodes.Count; j++) {
                        GameObject other = nodes[j];
                        float distance = Vector3.Distance(other.transform.position, nodeObj.transform.position);
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

                    // Update start points
                    for (int j = 0; j < playerCount; j++)
                    {
                        // Initial points are always closest
                        if (startNodes[j] == null)
                        {
                            startNodes[j] = node;
                        }
                        // If new point is closer than existing point, update
                        else if ((startPoints[j] - startNodes[j].transform.position).sqrMagnitude >
                            (startPoints[j] - node.transform.position).sqrMagnitude)
                        {
                            startNodes[j] = node;
                        }
                    }
                }

                nodes.Add(nodeObj);
            }

            for (int i = 0; i < playerCount; i++)
            {
                startNodes[i].SetOwner(i);
                startNodes[i].SetValue(1);
            }
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
    }
}