using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentManager : MonoBehaviour
{
    public bool multiplayer;

    Opponent opp;


    public Opponent getOpp()
    {
        return opp;
    }

    public void AI_Game()
    {
        opp = new AI_Rand();
    }
    // Start is called before the first frame update
    void Start()
    {
        multiplayer = false;
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
