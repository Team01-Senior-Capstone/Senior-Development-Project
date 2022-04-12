using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;
using System.Collections;

public class HelpManager : MonoBehaviour
{
    GameManager gm;
    string place1 = "You need to place your first worker!";
    string place2 = "You need to place your second worker!";
    string select = "You need to select your worker to move and build with! You can deselect them by clicking again on the worker";
    string move = "You need to move your selected worker! Click on the tile you want to move to";
    string build = "You need to build! Click on an adjacent tile to the worker you moved with!";
    string waiting = "You need to wait for your opponent to make a move!";

    string promptPlace = "Place Worker!";
    string promptSelect = "Select Worker!";
    string promptMove = "Move Worker!";
    string promptBuild = "Build!";
    string promptWaiting = "Opponents turn";

    public GameObject help;
    public TMP_Text tmp;
    public TMP_Text onScreenPrompts;

    public GameObject settingsPopUp;

    List<Boolean> enabledTiles;

    bool on = true;
    public void toggleHelpText()
    {
        on = !on;
        onScreenPrompts.alpha = on ? 255 : 0;
    }
    // Start is called before the first frame update
    void Start()
    {
        GameObject game_object = GameObject.Find("GameManager");
        gm = game_object.GetComponent<GameManager>();
    }

    public void helpClick()
    {
        switch(gm.getAction())
        {
            case Action.FIRST_MOVE:
                tmp.text = place1;
                onScreenPrompts.text = promptPlace;
                break;
            case Action.SECOND_MOVE:
                tmp.text = place2;
                onScreenPrompts.text = promptPlace;
                break;
            case Action.SELECT:
                tmp.text = select;
                onScreenPrompts.text = promptSelect;
                break;
            case Action.PLAY:
                tmp.text = move;
                onScreenPrompts.text = promptMove;
                break;
            case Action.BUILD:
                if(gm.getWaiting())
                {
                    tmp.text = waiting;
                    onScreenPrompts.text = promptWaiting;
                }
                else
                {
                    tmp.text = build;
                    onScreenPrompts.text = promptBuild;
                }
                break;
        }

        help.SetActive(true);
        //this.enabledTiles = gm.disableBoard();
    }
    IEnumerator waitTillTurn()
    {
        yield return new WaitUntil(() => gm.getWaiting() == false);
        onScreenPrompts.text = promptSelect;
    }

    public void toggleHelpString()
    {
        switch (gm.getAction())
        {
            case Action.FIRST_MOVE:
                onScreenPrompts.text = promptPlace;
                break;
            case Action.SECOND_MOVE:
                onScreenPrompts.text = promptSelect;
                break;
            case Action.SELECT:
                onScreenPrompts.text = promptMove;
                break;
            case Action.PLAY:
                onScreenPrompts.text = promptBuild;
                break;
            case Action.BUILD:
                if (gm.getWaiting())
                {
                    onScreenPrompts.text = promptWaiting;
                    StartCoroutine(waitTillTurn());
                }
                else
                {
                    onScreenPrompts.text = promptSelect;
                }
                break;
        }

        
        //this.enabledTiles = gm.disableBoard();
    }

    public void settings()
    {
        settingsPopUp.SetActive(true);
    }
    public void closeSettingS()
    {
        settingsPopUp.SetActive(false);
    }

    public void mainMenu()
    {
        
        GameObject goGame = GameObject.Find("Game");
        if (goGame != null)
        {
            Destroy(goGame);
        }
        //oppMan.reset();
        GameObject opp = GameObject.Find("Opponent");
        if (opp != null)
        {
            //opp.GetComponent<OpponentManager>().disconnect();
            Destroy(opp);
        }
        GameObject server = GameObject.Find("Server");
        Destroy(server);
        GameObject audio = GameObject.Find("AudioManager");
        Destroy(audio);
        SceneManager.LoadScene("Main Menu");
    }

    public void closeHelp()
    {
        tmp.text = "";
        help.SetActive(false);
        //gm.enableBoard(this.enabledTiles);
    }
}
