using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentManager : MonoBehaviour
{
    public bool multiplayer;

    Opponent opp;
    public string roomName;
    static int viewID = 1;

    public static int getViewID()
    {
        int temp = viewID;
        viewID++;
        return temp;
    }

    public bool ready = false;

    public ref Opponent getOpp()
    {
        return ref opp;
    }

    public void AI_Game()
    {
        multiplayer = false;
        opp = new AI_Rand();
    }

    public void Network_Game(string roomName, bool host)
    {
        //Connect 
        multiplayer = true;
        opp = new Network(roomName, host);
        this.roomName = roomName;
    }
    // Start is called before the first frame update
    void Start()
    {
        multiplayer = false;
        DontDestroyOnLoad(this.gameObject);
    }

    public void reset()
    {
        multiplayer = false;
        opp = null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
