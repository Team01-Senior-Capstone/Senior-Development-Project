using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{

    public string worker1_tag;
    public string worker2_tag;

    public bool playerGoesFirst;

    public bool hostGoFirst;
    public bool host;

    public bool netWorkGame;
    public Gamecore.GameController gameController;
    // Start is called before the first frame update
    public void Start()
    {
        netWorkGame = false;
        hostGoFirst = true;
        gameController = new Gamecore.GameController(netWorkGame);
        DontDestroyOnLoad(this.gameObject);
    }

    public void reset()
    {
        netWorkGame = false;
        hostGoFirst = true;
        gameController = new Gamecore.GameController(netWorkGame);
    }

    public void updateGameType(bool isNetwork)
    {
        netWorkGame = isNetwork;
        gameController = new Gamecore.GameController(netWorkGame);
    }

    public bool goesFirst()
    {
        return (host && hostGoFirst) || (!host && !hostGoFirst) || (!netWorkGame && (playerGoesFirst));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
