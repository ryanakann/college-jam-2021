using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Graphs
{
    namespace Nodes
    {
        public class NodeState
        {
            public bool inactive, blocking;
            public string name;
            public Node node;

            public NodeState(Node node, string name = "Idle")
            {
                this.node = node;
            }

            public virtual void PreActivate()
            {

            }

            public virtual void PostActivate()
            {

            }
        }

        public class FortifyState : NodeState
        {
            Faction faction;
            int turns, amount;

            public FortifyState(Node node, Faction faction, int turns, int amount, string name="Fortifying") : base(node, name)
            {
                this.faction = faction;
                this.turns = turns;
                this.amount = amount;
                blocking = true;
            }

            public override void PreActivate()
            {
                base.PreActivate();
                if (--turns <= 0)
                {
                    node.SetValue(node.value + amount);
                    node.RemoveState(this);
                }
            }
        }
    }
}