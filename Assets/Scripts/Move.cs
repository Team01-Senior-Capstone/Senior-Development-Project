using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class Move
{
    public Gamecore.Tile fromTile;
    public Gamecore.Tile toTile;

    public Gamecore.MoveAction action;

    
    public Gamecore.Worker worker;

    public Move() { }

    public Move(Gamecore.Tile fromT, Gamecore.Tile toT, Gamecore.MoveAction a, Gamecore.Worker w)
    {
        fromTile = fromT;
        toTile = toT;
        action = a;
        worker = w;
    }

}
