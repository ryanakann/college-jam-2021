using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Graphs.Nodes;

public class CollideWithTarget : MonoBehaviour {
    public int owner;
    [HideInInspector]
    public Node target;

    private void OnTriggerEnter(Collider other) {
        if (other.isTrigger) {
            return;
        }
        Node n = other.gameObject.GetComponent<Node>();
        if (n && n == target) {
            target.ReceivePhage(owner);
            //destroy animation target
            Destroy(GetComponent<OrbitTarget>().target.gameObject);
            //destroy self
            TurnManager.instance.phageCounter--;
            Destroy(gameObject);
        }
    }
}
