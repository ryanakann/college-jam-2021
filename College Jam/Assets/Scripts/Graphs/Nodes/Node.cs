using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Graphs
{
    namespace Nodes
    {
        public class Node : MonoBehaviour
        {
            public List<Node> neighbors;

            public NodeSelection nodeSelection;
            public NodePhysics nodePhysics;
            public NodeUI nodeUI;

            private void Awake()
            {
                neighbors = new List<Node>();
            }
        }
    }
}