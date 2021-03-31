using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Graphs
{
    namespace Nodes
    {
        public class NodeUI : MonoBehaviour
        {
            public NodeSelection nodeSelection;
            public TMP_Text basicUI;
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