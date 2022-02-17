using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HelpManager : MonoBehaviour
{
    GameManager gm;
    string place1 = "You need to place your first worker!";
    string place2 = "You need to place your second worker!";
    string select = "You need to select your worker to move and build with! You can deselect them by clicking again on the worker";
    string move = "You need to move your selected worker! Click on the tile you want to move to";
    string build = "You need to build! Click on an adjacent tile to the worker you moved with!";
    string waiting = "You need to wait for your opponent to make a move!";

    public GameObject help;
    public TMP_Text tmp;

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
                break;
            case Action.SECOND_MOVE:
                tmp.text = place2;
                break;
            case Action.SELECT:
                tmp.text = select;
                break;
            case Action.PLAY:
                tmp.text = move;
                break;
            case Action.BUILD:
                if(gm.getWaiting())
                {
                    tmp.text = waiting;
                }
                else
                {
                    tmp.text = build;
                }
                break;
        }

        help.SetActive(true);
    }

    public void close()
    {
        tmp.text = "";
        help.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
