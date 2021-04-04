using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Moves;

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
            public GameObject actionButtonPrefab;

            private void Start()
            {
                SetDetailVisibility(false);
            }

            public void SetDetailVisibility(bool visibile)
            {
                //if (visibile == true)
                //{
                //    Faction faction = TurnManager.instance.currentPlayer.Value.faction;
                //    action1.transform.GetChild(0).GetComponent<TMP_Text>().SetText(faction.moveSet[0].name);
                //    action1.onClick.RemoveAllListeners();

                //    action2.transform.GetChild(0).GetComponent<TMP_Text>().SetText(faction.moveSet[1].name);
                //    action2.onClick.RemoveAllListeners();

                //    action3.transform.GetChild(0).GetComponent<TMP_Text>().SetText(faction.moveSet[2].name);
                //    action3.onClick.RemoveAllListeners();
                //}
                detailedUI.SetActive(visibile);
            }

            public void PopulateUI(List<(Move, bool, string)> moves)
            {
                foreach (Transform child in detailedUI.transform.GetChild(0).GetChild(0))
                {
                    Destroy(child.gameObject);
                }

                moves.ForEach(move =>
                    {
                        GameObject buttonObj = Instantiate(actionButtonPrefab, detailedUI.transform.GetChild(0).GetChild(0));
                        ActionButton actionButton = buttonObj.GetComponent<ActionButton>();
                        actionButton.SetActionName(move.Item1.name);
                        if (move.Item2)
                        {
                            actionButton.ActivateButton();
                            actionButton.button.onClick.AddListener(() =>
                                {
                                    move.Item1.Execute(GetComponent<Node>());
                                    PlayerController.instance.HandleMoveNode(GetComponent<Node>());
                                }
                            );
                        }
                        else
                        {
                            actionButton.DeactivateButton();
                        }
                        actionButton.SetErrorDescription(move.Item3);
                    }
                );
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
                SetDetailVisibility(!detailedUI.activeSelf);
            }
        }
    }
}