using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Graphs
{
    namespace Nodes
    {
        public class Node : MonoBehaviour
        {
            public int owner;
            public int value;
            public int fortifying;
            public bool movedThisTurn;

            public List<Node> neighbors;

            public NodeSelection nodeSelection;
            public NodePhysics nodePhysics;
            public NodeUI nodeUI;

            private void Awake()
            {
                neighbors = new List<Node>();
                value = 0;
                fortifying = 0;
                movedThisTurn = false;
            }

            public void NextTurn()
            {
                if (fortifying == 1)
                {
                    fortifying--;
                    FortifyPayout();
                    movedThisTurn = false;
                }
                else if (fortifying > 1)
                {
                    fortifying--;
                    movedThisTurn = true;
                }
                else
                {
                    if (owner > 0)
                    {
                        IncrementValue();
                    }
                    movedThisTurn = false;
                }


            }

            public void SetOwner(int playerNum)
            {
                owner = playerNum;
                if (playerNum == 1)
                {
                    nodeSelection.mat.SetColor("_Color", new Color(177f / 255f, 255f / 255f, 125f / 255f));
                }
                else if (playerNum == 2)
                {
                    nodeSelection.mat.SetColor("_Color", new Color(219f / 255f, 88f / 255f, 224f / 255f));
                }
            }

            public void SetValue(int value)
            {
                this.value = value;
                nodeUI.basicUI.SetText(value.ToString());
            }

            public void IncrementValue()
            {
                SetValue(value + 1);
            }

            public void AddNeighbor (Node neighbor)
            {
                if (!neighbors.Contains(neighbor))
                {
                    neighbors.Add(neighbor);
                }
            }

            public bool Propagate()
            {
                if (movedThisTurn) return false;
                print("PP");
                int neighborCount = 0;
                List<Node> ownedNeighbors = new List<Node>();
                foreach (var neighbor in neighbors)
                {
                    if (neighbor.owner == owner || neighbor.owner == 0)
                    {
                        neighborCount++;
                        ownedNeighbors.Add(neighbor);
                    }
                }
                print(neighbors.Count);
                if (value <= neighborCount) return false;
                foreach (var neighbor in ownedNeighbors)
                {
                    neighbor.value++;
                    SetValue(value - 1);
                }
                return true;
            }

            public bool Divide(Node node)
            {
                if (movedThisTurn) return false;
                if (!neighbors.Contains(node)) return false;
                int index = neighbors.IndexOf(node);

                if (index < 0 || index >= neighbors.Count) return false;
                if (value < 2) return false;
                int numToSend = value / 2;
                if (neighbors[index].owner == owner)
                {
                    neighbors[index].value += numToSend;
                    SetValue(value - numToSend);
                }
                else
                {
                    if (neighbors[index].value > numToSend) return false;
                    if (neighbors[index].value == numToSend)
                    {
                        neighbors[index].owner = 0;
                    }
                    else
                    {
                        neighbors[index].owner = owner;
                    }
                    neighbors[index].SetValue(Mathf.Abs(neighbors[index].value - numToSend));

                    SetValue(value - numToSend);
                    fortifying = 0;
                }
                return true;
            }

            public bool Fortify()
            {
                if (movedThisTurn) return false;
                if (fortifying > 0) return false;

                fortifying = 2;
                return true;
            }

            private void FortifyPayout()
            {
                value += 5;
            }
        }
    }
}