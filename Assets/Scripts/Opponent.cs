using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Opponent
{
    public GameObject worker1;

    public GameObject worker2;

    public abstract Move GetMove();
}
