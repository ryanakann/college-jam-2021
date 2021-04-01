using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitTarget : MonoBehaviour {
    public GameObject target;
    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void FixedUpdate() {
        GetComponent<Rigidbody>().velocity += Random.insideUnitSphere * Time.fixedDeltaTime * 10f;
    }
}
