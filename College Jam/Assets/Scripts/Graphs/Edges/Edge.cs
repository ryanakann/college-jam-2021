using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Graphs {
    namespace Edges {
        public class Edge : MonoBehaviour {
            public float edgeWidth = 0.2f;

            protected GameObject node1;
            protected GameObject node2;

            public (Vector3, Vector3) GetEndpoints() { return (node1.transform.position, node2.transform.position); }

            public void SetNodes(GameObject node1, GameObject node2) {
                this.node1 = node1;
                this.node2 = node2;
                SetPosition();
            }

            void SetPosition() {
                transform.position = (node1.transform.position + node2.transform.position) / 2f;
                transform.up = (node1.transform.position - node2.transform.position).normalized;
                transform.localScale = new Vector3(edgeWidth, (node1.transform.position - node2.transform.position).magnitude / 2f, edgeWidth);
            }

            // Update is called once per frame
            void Update() {
                SetPosition();
            }
        }
    }
}