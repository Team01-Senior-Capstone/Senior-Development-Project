using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OpponentManager : MonoBehaviour
{
    //public bool multiplayer;

    Game game;

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
        if (game.netWorkGame)
        {
            return ref network;
        }
        else
        {
            return ref opp;
        }
    }

    public void AI_Game(int mode)
    {
        game.netWorkGame = false;
        if (mode == 1)
        {
            opp = new AI_Simple();
        }
        else
        {
            opp = new AI_Rand_Base();
        }
    }

    public void Network_Game()
    {
        //Connect 
        game.netWorkGame = true;
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
        game = GameObject.Find("Game").GetComponent<Game>();
 
        network = new Network();
        DontDestroyOnLoad(this.gameObject);
    }

    public void reset()
    {
        opp = null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
