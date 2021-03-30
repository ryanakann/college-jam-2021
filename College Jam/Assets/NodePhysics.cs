using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodePhysics : MonoBehaviour
{
    List<NodePhysics> neighbors;
    public float targetDistance;
    public float forceMultiplier = 0.01f;

    private void Awake()
    {
        neighbors = new List<NodePhysics>(FindObjectsOfType<NodePhysics>());
    }

    private void OnTriggerStay(Collider other)
    {
        Vector3 direction = other.transform.position - transform.position;
        float distance = direction.magnitude;
        Vector3 force = direction.normalized * (distance - targetDistance) * forceMultiplier;
        GetComponent<Rigidbody>().AddForce(force);
    }
}
