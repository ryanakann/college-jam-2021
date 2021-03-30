using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Graphs
{
    namespace Nodes
    {
        public class NodeUI : MonoBehaviour
        {
            public NodeSelection nodeSelection;
            public GameObject basicUI;
            public GameObject detailedUI;

            private void Start()
            {
                SetDetailVisibility(false);
            }

            public void SetDetailVisibility(bool visibile)
            {
                detailedUI.SetActive(visibile);
            }
        }
    }
}