using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Graphs.Edges;
using Graphs.Nodes;

namespace Graphs
{
    public class GraphGenerator : MonoBehaviour
    {
        public Graph graph;
        public GameObject nodePrefab;
        public GameObject edgePrefab;
        public int nodeCount;
        public float distanceThreshold = 1f;

        private List<GameObject> nodes;
        private Vector3 bounds;

        private Node player1Node;
        private Node player2Node;
        private float maxDistance;

        void Start()
        {
            bounds = GetComponent<BoxCollider>().bounds.extents;
            GetComponent<BoxCollider>().enabled = false;
            Generate();
        }

        void Generate()
        {
            nodes = new List<GameObject>();
            maxDistance = 0f;

            for (int i = 0; i < nodeCount; i++)
            {
                GameObject nodeObj = Instantiate(nodePrefab, transform);
                nodeObj.transform.position = SampleRandomPoint();
                Node node = nodeObj.GetComponent<Node>();
                graph.AddNode(node);

                foreach (var other in nodes)
                {
                    float distance = Vector3.Distance(other.transform.position, nodeObj.transform.position);
                    if (distance > maxDistance)
                    {
                        player1Node = node;
                        player2Node = other.GetComponent<Node>();
                        maxDistance = distance;
                    }
                    SpringJoint joint = nodeObj.AddComponent<SpringJoint>();
                    joint.connectedBody = other.GetComponent<Rigidbody>();
                    //joint.damper = 10f;
                    joint.spring = 1 / distance;

                    if (distance < distanceThreshold)
                    {
                        GameObject edge = Instantiate(edgePrefab, transform);
                        edge.GetComponent<Edge>().SetNodes(nodeObj, other);
                        graph.AddNeighbor(node, other.GetComponent<Node>());
                    }
                }
                nodes.Add(nodeObj);
            }

            player1Node.SetOwner(1);
            player2Node.SetOwner(2);
        }

        void ResetGraph()
        {
            //UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
            Generate();
        }

        Vector3 SampleRandomPoint()
        {
            return new Vector3(Random.Range(-bounds.x, bounds.x), Random.Range(-bounds.y, bounds.y), Random.Range(-bounds.z, bounds.z));
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                ResetGraph();
            }
        }
    }
}