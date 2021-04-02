using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Faction")]
public class MoveSet : ScriptableObject
{
    public List<Move> moves;
}