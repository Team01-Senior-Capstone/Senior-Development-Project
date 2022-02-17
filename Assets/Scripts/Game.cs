using UnityEngine;

public class Game : MonoBehaviour
{

    public string worker1_tag, worker2_tag;
    public bool playerGoesFirst, hostGoFirst, host, netWorkGame;
    private Gamecore.GameController gameController;
    
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

    public Gamecore.GameController getGameController () {

        return this.gameController;
    }
}
