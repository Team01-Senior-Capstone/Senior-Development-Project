using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public abstract class Opponent
{
    GameObject worker1, worker2;

    public GameObject getWorker1()
    {
        return worker1;
    }

    public GameObject getWorker2()
    {
        return worker2;
    }

    public void changeWorker1Pos(Vector3 pos)
    {
        worker1.transform.position = pos;
    }
    public void changeWorker2Pos(Vector3 pos)
    {
        worker2.transform.position = pos;
    }

    public void setWorker1(GameObject w)
    {
        worker1 = w;
    }
    public void setWorker2(GameObject w)
    {
        worker2 = w;
    }


    public abstract Tuple<Move, Move> GetMove(Gamecore.GameController gc);
    public abstract Tuple<Move, Move> getWorkerPlacements(Gamecore.GameController gc);
}