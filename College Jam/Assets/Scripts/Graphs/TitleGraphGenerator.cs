using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Graphs.Edges;
using Graphs.Nodes;

namespace Graphs {
    public class TitleGraphGenerator : MonoBehaviour {
        public Graph graph;
        public GameObject nodePrefab;
        public GameObject edgePrefab;
        protected int nodeCount;
        public float distanceThreshold = 1f;

        protected List<GameObject> nodes;
        protected Vector3 bounds;
        public float spawnInterval = 2f;
        public float displayInterval = 8f;
        public float resetInterval = 2f;
        protected float maxDistance;
        protected TitleNode player1StartNode;
        protected TitleNode player2StartNode;

        protected virtual void Start()
        {
            bounds = GetComponent<BoxCollider>().bounds.extents;
            GetComponent<BoxCollider>().enabled = false;
            nodeCount = 32;
            Generate();
        }

        protected virtual void ResetGraph()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
            graph.ResetGraph();
        }

        protected virtual Vector3 SampleRandomPoint()
        {
            return new Vector3(Random.Range(-bounds.x, bounds.x), Random.Range(-bounds.y, bounds.y), Random.Range(-bounds.z, bounds.z));
        }

        protected void Generate() {
            StartCoroutine(GenerateCR());
        }

        protected IEnumerator GenerateCR()
        {
            while (true)
            {
                nodes = new List<GameObject>();
                maxDistance = 0f;

                for (int i = 0; i < nodeCount; i++)
                {
                    yield return new WaitForSeconds(spawnInterval);
                    GameObject nodeObj = Instantiate(nodePrefab, transform);
                    nodeObj.transform.position = SampleRandomPoint();
                    TitleNode node = nodeObj.GetComponent<TitleNode>();
                    graph.AddNode(node);
                    if (i > 0)
                    {//do not add edges if first node
                        //definitely add one edge to ensure connectedness
                        //choose the closest previous node to this one
                        int closestNodeIndex = 0;
                        float closestDistance = Vector3.Distance(nodes[closestNodeIndex].transform.position, nodeObj.transform.position);
                        for (int j = 0; j < nodes.Count; j++)
                        {
                            GameObject other = nodes[j];
                            float distance = Vector3.Distance(other.transform.position, nodeObj.transform.position);
                            if (distance < closestDistance)
                            {
                                closestDistance = distance;
                                closestNodeIndex = j;
                            }
                        }

                        for (int j = 0; j < nodes.Count; j++)
                        {
                            GameObject other = nodes[j];
                            float distance = Vector3.Distance(other.transform.position, nodeObj.transform.position);
                            if (distance > maxDistance)
                            {
                                player1StartNode = node;
                                player2StartNode = other.GetComponent<TitleNode>();
                                maxDistance = distance;
                            }
                            SpringJoint joint = nodeObj.AddComponent<SpringJoint>();
                            joint.connectedBody = other.GetComponent<Rigidbody>();
                            joint.damper = 10f;
                            joint.spring = 10f;

                            if (distance < distanceThreshold || j == closestNodeIndex)
                            {
                                GameObject edge = Instantiate(edgePrefab, transform);
                                edge.GetComponent<Edge>().SetNodes(nodeObj, other);
                                graph.AddEdge(node, other.GetComponent<Node>());
                            }
                        }
                    }

                    nodes.Add(nodeObj);
                }

                ((TitleNode)player1StartNode)?.SetOwner(1);
                ((TitleNode)player2StartNode)?.SetOwner(2);

                yield return new WaitForSeconds(displayInterval);

                ResetGraph();
                yield return new WaitForSeconds(resetInterval);
            }
        }
    }
}