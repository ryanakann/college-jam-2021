using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wander : MonoBehaviour {
    public float speed = 10f;
    float maxSpeed = 0.5f;

    // Update is called once per frame
    void FixedUpdate() {
        Vector3 vel = GetComponent<Rigidbody>().velocity;
        vel += Random.insideUnitSphere * Time.fixedDeltaTime * speed;
        if (vel.magnitude > maxSpeed) {
            vel = vel.normalized * maxSpeed;
        }
        GetComponent<Rigidbody>().velocity = vel;
    }
}
