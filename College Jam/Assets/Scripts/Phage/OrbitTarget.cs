using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitTarget : MonoBehaviour {
    public Transform target;
    float maxSpeed = 2f;

    // Update is called once per frame
    void FixedUpdate() {
        Vector3 towardTarget = target.position - transform.position;
        float targetRadius = target.lossyScale.magnitude / 3f;
        float myRadius = towardTarget.magnitude;
        Vector3 radiusCorrection = (myRadius - targetRadius) * towardTarget;

        Vector3 vel = Vector3.ProjectOnPlane(GetComponent<Rigidbody>().velocity, towardTarget);
        Vector3 perp = Vector3.Cross(vel, towardTarget).normalized;
        perp *= Random.Range(-0.1f, 0.1f);
        vel += radiusCorrection + perp + Random.insideUnitSphere * 0.01f;
        if (vel.magnitude > maxSpeed) {
            vel = vel.normalized * maxSpeed;
        }

        GetComponent<Rigidbody>().velocity = vel;
    }
}
