using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Graphs {
    namespace Nodes {
        public class Node : MonoBehaviour {
            public int owner;
            public int value;
            public int fortifying;
            public bool movedThisTurn;

            public List<Node> neighbors;
            public List<NodeState> nodeStates;

            public NodeSelection nodeSelection;
            public NodePhysics nodePhysics;
            public NodeUI nodeUI;

            private void Awake() {
                neighbors = new List<Node>();
                nodeStates = new List<NodeState>();
                value = 0;
                fortifying = 0;
                movedThisTurn = false;
                owner = -1;
            }

            public virtual void ReceivePhage(int phageOwner) {
                if (owner == phageOwner) {
                    IncrementValue();
                } else {
                    DecrementValue(phageOwner);
                }
            }

            public virtual void SetOwner(int playerNum) {
                if (owner >= 0)
                {
                    Player oldOwner = GameSettings.instance.players[owner];
                    oldOwner.nodes.Remove(this);
                }
                Player newOwner = GameSettings.instance.players[playerNum];
                owner = playerNum;
                Color color = newOwner.color;
                print($"Setting player {owner} to color {color}");
                nodeSelection.mat.SetColor("_Color", color);
                newOwner.nodes.Add(this);
            }

            public virtual void SetValue(int value) {
                this.value = value;
                nodeUI.basicUI.SetText(value.ToString());
            }

            public void IncrementValue() {
                SetValue(value + 1);
            }

            public void DecrementValue(int decrementingPlayerNum) {
                SetValue(value - 1);
                if (value <= 0) {
                    SetOwner(decrementingPlayerNum);
                    SetValue(1);
                }
            }

            public void AddNeighbor(Node neighbor) {
                if (!neighbors.Contains(neighbor)) {
                    neighbors.Add(neighbor);
                }
            }

            public void AddState(NodeState nodeState) {

            }

            public void RemoveState(NodeState nodeState, bool killState = true) {
                if (killState)
                    nodeState.inactive = true;
                nodeStates.Remove(nodeState);
            }


            /*
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
                    neighbor.IncrementValue();
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
                SetValue(value + 5);
            }
            */
        }
    }
}