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

        public virtual void Finish(Node node)
        {
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
            description = "Transfer 1 phage from the source node to each edge.";
        }

        public override void Execute(Node node) {
            base.Execute(node);
            foreach (var neighbor in node.neighbors) {
                Graph.instance.SendPhages(node, neighbor, 1);
            }
        }
    }

    public class Split : Move {

        public Split() : base() {
            name = "Split";
            validationChecks.Add(new AtLeast_MVC(2));
            description = "Send half of the source node's phages to a target neighboring node.";
        }

        public override void Execute(Node node) {
            PlayerController.instance.SetContext(new AdjacentSelectContext(node));
            ((AdjacentSelectContext)PlayerController.instance.context).OnSelect += FinalExecute;
        }

        public void FinalExecute(Node srcNode, Node tgtNode) {
            base.Execute(srcNode);
            int amountToSend = srcNode.value / 2;
            Graph.instance.SendPhages(srcNode, tgtNode, amountToSend);
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

}