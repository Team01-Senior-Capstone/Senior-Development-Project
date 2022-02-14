using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using UnityEngine.SceneManagement;
using System.Linq;

public class MultiPlayerManager : MonoBehaviour
{

    public GameObject game_object;
    public Game game;

    public GameObject opp_object;
    public OpponentManager oppMan;
    public Button hostButton;
    public Button joinButton;
    public Button submit;
    public TMP_InputField roomName;

    public string submittedRoomName;

    private static System.Random random = new System.Random();

    public int roomNameLength = 7;

    public string getRandomString()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, roomNameLength)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    public void Awake()
    {
        game.netWorkGame = true;
        game_object = GameObject.Find("Game");
        game = game_object.GetComponent<Game>();
        opp_object = GameObject.Find("Opponent");
        oppMan = opp_object.GetComponent<OpponentManager>();
        oppMan.multiplayer = true;
        roomName.gameObject.SetActive(false);
    }

    public void client()
    {
        game.host = false;
    }


    public void host()
    {
        game.host = true;
        hostButton.gameObject.SetActive(false);
        joinButton.gameObject.SetActive(false);
        roomName.gameObject.SetActive(true);
        ((TextMeshProUGUI)roomName.placeholder).text = getRandomString();
        submit.gameObject.SetActive(true);
    }

    public void goBack()
    {
        game.netWorkGame = false;
        SceneManager.LoadScene("Main Menu");
    }

    public void onSumbit()
    {
        string text;

        if(roomName.text.Length > 80)
        {
            text = roomName.text.Substring(0, 80);
        }
        else if(roomName.text.Length > 0)
        {
            text = roomName.text;
        }
        else
        {
            text = ((TextMeshProUGUI)roomName.placeholder).text;
        }
        Debug.Log(text);
        text = submittedRoomName;
    }

    public void play()
    {
        oppMan.Network_Game(submittedRoomName, game.host);

        SceneManager.LoadScene("WorkerSelection");
    }
}
