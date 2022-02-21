using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OpponentManager : MonoBehaviour
{
    public bool multiplayer;

    Opponent opp;
    Opponent network;
    public string roomName;
    static int viewID = 1;

    public static int getViewID()
    {
        int temp = viewID;
        viewID++;
        return temp;
    }

    public bool ready = false;
    public void disconnect()
    {
        ((Network)network).disconnect();
    }
    public ref Opponent getOpp()
    {
        if (multiplayer)
        {
            return ref network;
        }
        else
        {
            return ref opp;
        }
    }

    public void AI_Game()
    {
        multiplayer = false;
        opp = new AI_Rand();
    }

    public void Network_Game()
    {
        //Connect 
        multiplayer = true;
        //opp = new Network();
    }

    public void host(string roomName)
    {
        ((Network)network).HostRoom(roomName);
    }

    public void join(string roomName)
    {
        ((Network)network).JoinRoom(roomName);
        SceneManager.LoadScene("WorkerSelection");
    }
    // Start is called before the first frame update
    void Start()
    {
        multiplayer = false;
        network = new Network();
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
