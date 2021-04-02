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
            public GameObject phagePrefab;

            private void Start()
            {
                SetDetailVisibility(false);
            }

            public void SetDetailVisibility(bool visibile)
            {
                detailedUI.SetActive(visibile);
            }

            public void SendPhages(Node node, int quantity)
            {
                StartCoroutine(SendPhageCR(node, quantity));
            }

            IEnumerator SendPhageCR(Node node, int quantity)
            {
                for (int i = 0; i < quantity; i++)
                {
                    Instantiate(phagePrefab);
                    yield return new WaitForSeconds(0.3f);
                }
            }
            
            public void ToggleVisibility()
            {
                detailedUI.SetActive(!detailedUI.activeSelf);
            }
        }
    }
}