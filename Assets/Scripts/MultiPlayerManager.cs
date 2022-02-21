using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.Linq;
using Photon.Realtime;

public class MultiPlayerManager : MonoBehaviour
{

    public GameObject game_object;
    public Game game;

    public GameObject noGames;

    Vector3 roomSpawnPos;

    public GameObject roomPrefab;
    public GameObject next;

    public GameObject opp_object;
    public OpponentManager oppMan;
    Network net;


    public GameObject canvas;
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
        game_object = GameObject.Find("Game");
        game = game_object.GetComponent<Game>();

        game.netWorkGame = true;
        opp_object = GameObject.Find("Opponent");
        oppMan = opp_object.GetComponent<OpponentManager>();
        oppMan.multiplayer = true;
        roomName.gameObject.SetActive(false);
        roomSpawnPos = roomName.gameObject.transform.position;
        noGames.gameObject.SetActive(false);
        oppMan.Network_Game();
        net = (Network)oppMan.getOpp();
        
    }

    public void client()
    {
        game.host = false;
        hostButton.gameObject.SetActive(false);
        joinButton.gameObject.SetActive(false);
        //roomName.gameObject.SetActive(true);
        //submit.gameObject.SetActive(true);
        spawnRoomTiles();
        next.gameObject.SetActive(false);
        
    }

    public void spawnRoomTiles()
    {
        List<RoomInfo> activeRooms = net.rooms();
        if (activeRooms != null)
        {
            foreach (RoomInfo ri in activeRooms)
            {
                if(ri.PlayerCount > 1)
                {
                    continue;
                }
                GameObject newRoom = Instantiate(roomPrefab, roomSpawnPos, Quaternion.identity);
                newRoom.GetComponentInChildren<TMP_Text>().text = ri.Name;
                newRoom.tag = "RoomButton";
                newRoom.transform.SetParent(canvas.transform);
                newRoom.GetComponent<Button>().onClick.AddListener(delegate { oppMan.join(ri.Name); }); ;
                roomSpawnPos.y += 90;
            }
            if (activeRooms.Count == 0)
            {
                noGames.gameObject.SetActive(true);
            }
            else
            {
                noGames.gameObject.SetActive(false);
            }
        }
        else
        {
            noGames.gameObject.SetActive(true);
        }
    }

    public void dontJoinRoom()
    {
        GameObject[] roomTiles = GameObject.FindGameObjectsWithTag("RoomButton");
        foreach(GameObject go in roomTiles)
        {
            Destroy(go);
        }
        noGames.gameObject.SetActive(false);
        hostButton.gameObject.SetActive(true);
        joinButton.gameObject.SetActive(true);
    }

    public void host()
    {
        game.host = true;
        hostButton.gameObject.SetActive(false);
        joinButton.gameObject.SetActive(false);
        roomName.gameObject.SetActive(true);
        ((TextMeshProUGUI)roomName.placeholder).text = getRandomString();
        submit.gameObject.SetActive(true);
        next.gameObject.SetActive(true);
    }

    public void goBack()
    {
        oppMan.disconnect();
        game.netWorkGame = false;
        game.updateGameType(false);

        Debug.Log(game.netWorkGame);
        SceneManager.LoadScene("Main Menu");
    }

    public void returnToMenu()
    {
        hostButton.gameObject.SetActive(true);
        joinButton.gameObject.SetActive(true);
        roomName.gameObject.SetActive(false);
        submit.gameObject.SetActive(false);
        next.gameObject.SetActive(false);
    }

    public void play()
    {
        string text;

        if (roomName.text.Length > 80)
        {
            text = roomName.text.Substring(0, 80);
        }
        else if (roomName.text.Length > 0)
        {
            text = roomName.text;
        }
        else
        {
            text = ((TextMeshProUGUI)roomName.placeholder).text;
        }
        Debug.Log(text);
        submittedRoomName =text;

        oppMan.host(submittedRoomName);
        

       
        SceneManager.LoadScene("WorkerSelection");
    }
}
