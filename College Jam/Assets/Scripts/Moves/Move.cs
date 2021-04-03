using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Graphs.Nodes;

namespace Moves
{
    [System.Serializable]
    public class Move
    {
        public List<MoveValidationCheck> validationChecks;
        public string description;

        public Move()
        {
            validationChecks = new List<MoveValidationCheck>() { new Blocking_MCV() };
        }

        // can this move be executed from this node?
        public virtual (bool, string) Validate(Node node)
        {
            string msg = "";
            bool result = true;
            foreach (var check in validationChecks)
            {
                (bool subResult, string subMsg) = check.Validate(node);
                if (!result)
                {
                    result = false;
                    msg += $"{subMsg}\n";
                }
            }
            return (result) ? (result, description) : (result, msg);
        }

        // execute this move from this node.
        public virtual void Execute(Node node)
        {

        }
    }

    public class Propagate : Move
    {

        public Propagate() : base()
        {
            validationChecks.Add(new GreaterThanDegree_MCV());
            description = "Transfer 1 phage from the source node to each edge.";
        }

        public override void Execute(Node node)
        {
            base.Execute(node);
            foreach (var neighbor in node.neighbors)
            {
                node.value--;
                // neighbor.SendPhage(1, node.faction) // Phage Count, owned faction
            }
        }
    }

    public class Split : Move
    {

        public Split() : base()
        {
            validationChecks.Add(new AtLeast_MCV(2));
            description = "Send half of the source node's phages to a target neighboring node.";
        }

        public override void Execute(Node node)
        {
            base.Execute(node);

            PlayerController.instance.SetContext(new AdjacentSelectContext(node));
            ((AdjacentSelectContext)PlayerController.instance.context).OnSelect += FinalExecute;
        }

        public void FinalExecute(Node srcNode, Node tgtNode)
        {
            srcNode.value /= 2;
            // tgtNode.SendPhage((int)(srcNode.phageCount / 2), srcNode.faction)
        }
    }

    public class Fortify : Move
    {
        public Faction faction;
        public int totalTurns, amount;

        public Fortify(Faction faction, int totalTurns = 2, int amount = 4)
        {
            this.faction = faction;
            this.totalTurns = totalTurns;
            this.amount = amount;
            description = "Node stays inactive for 2 turns, then gains 4 phages.";
        }

        public override void Execute(Node node)
        {
            base.Execute(node);
            // 
            node.AddState(new FortifyState(node, faction, totalTurns, amount));
        }
    }

    public class Invest : Fortify
    {
        public int investment;

        public Invest(Faction faction, int totalTurns=3, int amount=6, int investment=3) : base(faction, totalTurns, amount)
        {
            this.investment = investment;
            description = "Spend 3 phages, then node remains inactive for 3 turns, then gain 6 phages.";
        }

        public override void Execute(Node node)
        {
            base.Execute(node);
            // 
            node.AddPhages(-investment);
            node.AddState(new FortifyState(node, faction, totalTurns, amount, name:"Investing"));
        }
    }


    public class MoveValidationCheck
    {
        public virtual (bool, string) Validate(Node node)
        {
            return (true, "");
        }
    }

    public class Blocking_MCV : MoveValidationCheck
    {
        public override (bool, string) Validate(Node node)
        {
            foreach (var nodeState in node.nodeStates)
            {
                if (!nodeState.inactive && nodeState.blocking)
                {
                    return (false, $"Node is blocked by action: {nodeState.name}");
                }
            }
            return (true, "");
        }
    }

    public class GreaterThanDegree_MCV : MoveValidationCheck
    {
        public override (bool, string) Validate(Node node)
        {
            return (node.value > node.neighbors.Count) ? (true, "") : (false, "This node must have more phages than edges.");
        }
    }

    public class AtLeast_MCV : MoveValidationCheck
    {
        int amount;
        public AtLeast_MCV(int amount)
        {
            this.amount = amount;
        }

        public override (bool, string) Validate(Node node)
        {
            return (node.value >= amount) ? (true, "") : (false, $"This node must have at least {amount} phages.");
        }
    }

}