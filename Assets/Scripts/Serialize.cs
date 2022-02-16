using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Serialize
{

    public static Tuple<string, string> serialize(Tuple<Move, Move> moves)
    {
        string s1 = MoveToString(moves.Item1);
        string s2 = MoveToString(moves.Item2);

        return new Tuple<string, string>(s1, s2);
    }

    public static Tuple<Move, Move> deserialize(string s1, string s2)
    {
        Move m1 = stringToMove(s1);
        Move m2 = stringToMove(s2);

        return new Tuple<Move, Move>(m1, m2);
    }

    static string MoveToString(Move m)
    {
        string s = "";
        //The first two moves do not have from tiles
        if (m.fromTile != null)
        {
            s += m.fromTile.getRow();
            s += m.fromTile.getCol();
        }
        else
        {
            s += "00";
        }
        s += m.toTile.getRow();
        s += m.toTile.getCol();
        if(m.action == Gamecore.MoveAction.Build)
        {
            s += 'B';
        }
        else
        {
            s += 'M';
        }

        return s;
    }

    static Move stringToMove(string s)
    {
        Game g = GameObject.Find("Game").GetComponent<Game>();
        int fromRow, fromCol, toRow, toCol;
        Gamecore.MoveAction a;

        fromRow = int.Parse(s[0].ToString());
        fromCol = int.Parse(s[1].ToString());
        toRow = int.Parse(s[2].ToString());
        toCol = int.Parse(s[3].ToString());

        Gamecore.Tile from = g.game.getGameboard()[fromRow, fromCol];
        Gamecore.Tile to = g.game.getGameboard()[toRow, toCol];


        if (s[4] == 'B')
        {
            a = Gamecore.MoveAction.Build;
        }
        else
        {
            a = Gamecore.MoveAction.Move;
        }

        Gamecore.Worker w = from.getWorker();

        return new Move(from, to, a, w);
    }

}
