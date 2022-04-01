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
        //viewID++;
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
            //opp = new AI_Simple();
            opp = new AI_Better();
        }
        else
        {
            //opp = new AI_Rand_Base();
            opp = new AI_Simple();
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

    public int getPlayersInRoom()
    {
        return ((Network)network).getNumPlayers();
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
        GameObject server;
        NetworkServer ns;
        if (GameObject.Find("Server") == null)
        {
            server = new GameObject("Server");

            GameObject.DontDestroyOnLoad(server.gameObject);
            server.AddComponent<NetworkServer>();
            ns = server.GetComponent<NetworkServer>();
        }
        else
        {
            server = null;
            ns = null;
        }
        network = new Network(server, ns);
        DontDestroyOnLoad(this.gameObject);
    }

    public void reset()
    {
        opp = null;
    }

    public bool connected()
    {
        if(game.netWorkGame)
        {
            return ((Network)network).connected();
        }
        else
        {
            return true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
