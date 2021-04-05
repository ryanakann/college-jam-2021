using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Graphs.Nodes;
using Graphs;

namespace Moves {
    [System.Serializable]
    public class Move {
        public List<MoveValidationCheck> validationChecks;
        public string name;
        public string description;

        public Move() {
            validationChecks = new List<MoveValidationCheck>() { new Blocking_MVC() };
        }

        // can this move be executed from this node?
        public virtual (bool, string) Validate(Node node) {
            string msg = "";
            bool result = true;
            foreach (var check in validationChecks) {
                (bool subResult, string subMsg) = check.Validate(node);
                if (!subResult) {
                    result = false;
                    msg += $"{subMsg}\n";
                }
            }
            return (result) ? (result, description) : (result, msg);
        }

        public virtual void Finish(Node node) {
            Player currentPlayer = TurnManager.instance.currentPlayer.Value;
            //remove node from player's actableNodes
            currentPlayer.actableNodes.Remove(node);
            //hide move selection UI
            node.nodeUI.SetDetailVisibility(false);
            //update node highlights
            currentPlayer.highlightActableNodes();

            PlayerController.instance.UpdateToolTip(null);
        }

        // execute this move from this node.
        public virtual void Execute(Node node) {
            Finish(node);
        }
    }

    public class Propagate : Move {

        public Propagate() : base() {
            name = "Propagate";
            validationChecks.Add(new GreaterThanDegree_MVC());
            description = "Transfer up to 3 phages from the source node to each edge.";
        }

        public override void Execute(Node node) {
            base.Execute(node);
            //TODO: send phages in rounds
            foreach (var neighbor in node.neighbors) {
                Graph.instance.SendPhages(node, neighbor, 1);
            }
        }
    }

    public class Consolidate : Move {
        public Consolidate() : base() {
            name = "Consolidate";
            validationChecks.Add(new Consolidate_MVC());
            description = "Gather up to 3 phages from each neighboring node that you own.";
        }
        public override void Execute(Node node) {
            base.Execute(node);
            foreach (var neighbor in node.neighbors) {
                if (neighbor.owner == node.owner) {
                    Graph.instance.SendPhages(neighbor, node, Mathf.Min(3, neighbor.value - 1));
                }
            }
        }
    }

    public class TargetedMove : Move {
        public TargetedMove() : base() {
        }
        public override void Execute(Node node) {
            PlayerController.instance.SetContext(new AdjacentSelectContext(node, this));
            ((AdjacentSelectContext)PlayerController.instance.context).OnSelect += FinalExecute;
        }
        public virtual void FinalExecute(Node srcNode, Node tgtNode) {
            base.Execute(srcNode);
        }
    }

    public class Split : TargetedMove {

        public Split() : base() {
            name = "Split";
            validationChecks.Add(new AtLeast_MVC(2));
            description = "Send half of the node's phages to a target neighboring node.";
        }

        public override void FinalExecute(Node srcNode, Node tgtNode) {
            base.FinalExecute(srcNode, tgtNode);
            int amountToSend = srcNode.value / 2;
            if (srcNode.value % 2 == 1) {//if odd, go above half
                amountToSend++;
            }
            Graph.instance.SendPhages(srcNode, tgtNode, amountToSend);
        }
    }

    public class Leech : TargetedMove {
        public Leech() : base() {
            name = "Leech";
            validationChecks.Add(new Leech_MVC());
            description = "Steal one phage from a target neighboring node.";
        }
        public override void FinalExecute(Node srcNode, Node tgtNode) {
            base.FinalExecute(srcNode, tgtNode);
            Graph.instance.SendPhages(tgtNode, srcNode, 1, srcNode.owner);
        }
    }

    public class Fortify : Move {
        public Faction faction;
        public int totalTurns, amount;

        public Fortify(int totalTurns = 2, int amount = 4) : base() {
            name = "Fortify";
            this.totalTurns = totalTurns;
            this.amount = amount;
            description = $"Node stays inactive for {totalTurns} turns, then gains {amount} phages.";
        }

        public override void Execute(Node node) {
            base.Execute(node);
            faction = TurnManager.instance.currentPlayer.Value.faction;
            var state = new FortifyState(node, faction, totalTurns, amount);
            TurnManager.instance.currentPlayer.Value.nodeStates.Add(state);
            node.AddState(state);
        }
    }

    public class Invest : Fortify {
        public int investment;

        public Invest(int totalTurns = 3, int amount = 6, int investment = 3) : base(totalTurns, amount) {
            name = "Invest";
            this.investment = investment;
            description = $"Spend {investment} phages. Node remains inactive for {totalTurns} turns, then gain {amount} phages.";
            validationChecks.Add(new AtLeast_MVC(3));
        }

        public override void Execute(Node node) {
            Finish(node);
            node.SetValue(node.value - investment);
            node.AddState(new FortifyState(node, faction, totalTurns, amount, name: "Investing"));
        }
    }


    public class MoveValidationCheck {
        public virtual (bool, string) Validate(Node node) {
            return (true, "");
        }

        //for targeted moves
        public virtual (bool, string) Validate(Node srcNode, Node tgtNode) {
            return (true, "");
        }
    }

    public class Blocking_MVC : MoveValidationCheck {
        public override (bool, string) Validate(Node node) {
            foreach (var nodeState in node.nodeStates) {
                if (!nodeState.inactive && nodeState.blocking) {
                    return (false, $"Node is blocked by state: {nodeState.name}");
                }
            }
            return (true, "");
        }
    }

    public class GreaterThanDegree_MVC : MoveValidationCheck {
        public override (bool, string) Validate(Node node) {
            return (node.value > node.neighbors.Count) ? (true, "") : (false, "This node must have more phages than edges.");
        }
    }

    public class AtLeast_MVC : MoveValidationCheck {
        int amount;
        public AtLeast_MVC(int amount) {
            this.amount = amount;
        }

        public override (bool, string) Validate(Node node) {
            return (node.value >= amount) ? (true, "") : (false, $"This node must have at least {amount} phages.");
        }
    }

    public class Leech_MVC : MoveValidationCheck {
        public override (bool, string) Validate(Node node) {
            foreach (Node neighbor in node.neighbors) {
                if (neighbor.owner != node.owner && neighbor.value > 0) {
                    return (true, "");
                }
            }
            return (false, "Target node must have at least one phage to steal.");
        }
        public override (bool, string) Validate(Node srcNode, Node tgtNode) {
            return (tgtNode.value > 0) ? (true, "") : (false, "Target node must have at least one phage to steal.");
        }
    }

    public class Consolidate_MVC : MoveValidationCheck {
        public override (bool, string) Validate(Node node) {
            bool hasFriendlyNeighbor = false;
            foreach (Node neighbor in node.neighbors) {
                if (neighbor.owner == node.owner) {
                    hasFriendlyNeighbor = true;
                    if (neighbor.value > 1) {
                        return (true, "");
                    }
                }
            }
            if (hasFriendlyNeighbor) {
                return (false, "There must be at least one extra phage to gather from your neighbors.");
            } else {
                return (false, "There must be at least one neighbor that you own.");
            }
        }
    }

    public class Propagate_MVC : MoveValidationCheck {
        public override (bool, string) Validate(Node node) {
            return (true, "");
        }
    }


}