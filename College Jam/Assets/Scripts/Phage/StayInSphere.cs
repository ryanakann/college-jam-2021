using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StayInSphere : MonoBehaviour {
    public Graphs.Graph graph;
    public float radius = 1f;
    float strength = 0.01f;
    // Start is called before the first frame update
    void Start() {
        graph = GameObject.Find("Graph").GetComponent<Graphs.Graph>();
        transform.position = Random.insideUnitSphere * radius * 0.7f;
    }

    // Update is called once per frame
    void FixedUpdate() {
        print(graph.Center());
        Vector3 diff = (graph.Center() - transform.position);
        if (diff.magnitude > radius) {
            GetComponent<Rigidbody>().velocity = diff * strength;
        }
    }
}
