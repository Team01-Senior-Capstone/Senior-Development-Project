using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move
{
    public Gamecore.Tile fromTile;
    public Gamecore.Tile toTile;

    public Gamecore.Action action;

    public Move(Gamecore.Tile fromT, Gamecore.Tile toT, Gamecore.Action a)
    {
        fromTile = fromT;
        toTile = toT;
        action = a;
    }

    string worker;
}
