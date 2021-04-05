using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Moves;

public class Faction {
    #region STATIC
    public static List<Faction> factionList = new List<Faction>
    {
        new Faction(
            name: "Lemming",
            moveSet: new List<Move>
            {
                new Split(),
                new Propagate(),
                new Fortify(),
            }
        ),

        new Faction(
            name: "Castle",
            moveSet: new List<Move>
            {
                new Split(),
                new Propagate(),
                new Invest(),
            }
        ),

        new Faction(
            name: "River",
            moveSet: new List<Move>
            {
                new Split(),
                new Propagate(),
                new Consolidate(),
            }
        ),

        new Faction(
            name: "Garrison",
            moveSet: new List<Move>
            {
                new Split(),
                new Propagate(),
                new Consolidate(),

            }
        ),

        new Faction(
            name: "Parasite",
            moveSet: new List<Move>
            {
                new Split(),
                new Propagate(),
                new Leech(),
            }
        ),

        new Faction(
            name: "Hermit",
            moveSet: new List<Move>
            {
                new Split(),
                new Propagate(),
                new Leech(),
            }
        ),
    };

    private static Dictionary<string, int> factionMap = new Dictionary<string, int>
    {
        { factionList[0].name, 0 },
        { factionList[1].name, 1 },
        { factionList[2].name, 2 },
        { factionList[3].name, 3 },
        { factionList[4].name, 4 },
        { factionList[5].name, 5 },
    };

    public static Faction FindFactionWithName(string name) {
        if (!factionMap.ContainsKey(name)) return null;
        return factionList[factionMap[name]];
    }
    #endregion

    public string name;
    public List<Move> moveSet;

    public Faction(string name, List<Move> moveSet) {
        this.name = name;
        this.moveSet = moveSet;
    }
}
