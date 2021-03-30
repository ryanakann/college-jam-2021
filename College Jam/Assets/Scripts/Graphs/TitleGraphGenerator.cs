using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Graphs.Edges;
using Graphs.Nodes;

namespace Graphs {
    public class TitleGraphGenerator : GraphGenerator {

        public float spawnInterval = 2f;
        public float displayInterval = 8f;
        public float resetInterval = 2f;

        protected override void Generate() {
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
                    Node node = nodeObj.GetComponent<Node>();
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
                                player2StartNode = other.GetComponent<Node>();
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

                player1StartNode.SetOwner(1);
                player2StartNode.SetOwner(2);

                yield return new WaitForSeconds(displayInterval);

                ResetGraph();
                yield return new WaitForSeconds(resetInterval);
            }
        }
    }
}