using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StayInSphere : MonoBehaviour {
    public Graphs.Graph graph;
    Vector3 center;
    public float radius = 1f;
    float strength = 0.01f;
    // Start is called before the first frame update
    void Start() {
        graph = GameObject.Find("Graph").GetComponent<Graphs.Graph>();
        center = Vector3.zero;
        transform.position = Random.insideUnitSphere * radius * 0.7f + center;
    }

    // Update is called once per frame
    void FixedUpdate() {
        center = graph.Center();
        if ((transform.position - center).magnitude > radius) {
            GetComponent<Rigidbody>().velocity = (center - transform.position) * strength;
        }
    }
}
