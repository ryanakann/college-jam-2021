using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphGenerator : MonoBehaviour
{
    public GameObject nodePrefab;
    public GameObject edgePrefab;
    public int nodeCount;
    public float distanceThreshold = 1f;
    private float distanceThresholdSqrd;

    private List<GameObject> nodes;
    private Vector3 bounds;

    void Start()
    {
        nodes = new List<GameObject>();
        bounds = GetComponent<BoxCollider>().bounds.extents;
        distanceThresholdSqrd = distanceThreshold * distanceThreshold;
        GetComponent<BoxCollider>().enabled = false;

        for (int i = 0; i < nodeCount; i++)
        {
            Vector3 pos = SampleRandomPoint();
            GameObject node = Instantiate(nodePrefab, pos, Quaternion.identity);
            foreach (var other in nodes)
            {
                float distance = Vector3.Distance(other.transform.position, node.transform.position);
                SpringJoint joint = node.AddComponent<SpringJoint>();
                joint.connectedBody = other.GetComponent<Rigidbody>();
                //joint.damper = 10f;
                joint.spring = 1 / distance;

                if (distance  < distanceThreshold)
                {
                    GameObject edge = Instantiate(edgePrefab);
                    edge.GetComponent<Edge>().SetNodes(node, other);
                }
            }
            nodes.Add(node);
        }

    }

    Vector3 SampleRandomPoint()
    {
        return new Vector3(Random.Range(-bounds.x, bounds.x), Random.Range(-bounds.y, bounds.y), Random.Range(-bounds.z, bounds.z));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
