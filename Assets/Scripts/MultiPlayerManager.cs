using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.Linq;
using Photon.Realtime;
using System;

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
    public TMP_Text errorJoiningText;

    public GameObject canvas;
    public GameObject roomListAnchor;
    public GameObject roomListScroll;
    public GameObject offlineOverlay;
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

        opp_object = GameObject.Find("Opponent");
        oppMan = opp_object.GetComponent<OpponentManager>();
        game.netWorkGame = true;
        roomName.gameObject.SetActive(false);
        roomSpawnPos = roomName.gameObject.transform.position;
        noGames.gameObject.SetActive(false);
        oppMan.Network_Game();
        net = (Network)oppMan.getOpp();
        
    }

    public void Update()
    {
        if (game.netWorkGame)
        {
            StartCoroutine(checkInternetConnection((isConnected) => {
                if (!isConnected)
                {
                    offlineOverlay.SetActive(true) ;
                }
                else
                {
                    offlineOverlay.SetActive(false);
                }
            }));

        }
    }

    IEnumerator checkInternetConnection(Action<bool> action)
    {
        WWW www = new WWW("http://google.com");
        yield return www;
        if (www.error != null)
        {
            action(false);
        }
        else
        {
            action(true);
        }
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

    void deleteTiles()
    {
       
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("RoomButton");
        Debug.Log(tiles.Length);
        foreach(GameObject go in tiles)
        {
            Destroy(go);
        }
    }


    public void spawnRoomTiles()
    {
        deleteTiles();
        List<RoomInfo> activeRooms = net.rooms();
        int spawnedRooms = 0;
        if (activeRooms != null)
        {
            foreach (RoomInfo ri in activeRooms)
            {
                if(ri.PlayerCount > 1 || ri.PlayerCount == 0)
                {
                    continue;
                }
                GameObject newRoom = Instantiate(roomPrefab, roomSpawnPos, Quaternion.identity);
                newRoom.GetComponentInChildren<TMP_Text>().text = ri.Name;
                newRoom.tag = "RoomButton";
                newRoom.transform.SetParent(roomListScroll.transform, false);
                newRoom.GetComponent<Button>().onClick.AddListener(delegate { tryJoinRoom(ri.Name); }); ;
                roomSpawnPos.y -= 500;
                spawnedRooms++;
            }
            if (spawnedRooms == 0)
            {
                roomListAnchor.SetActive(false);
                noGames.gameObject.SetActive(true);
            }
            else
            {
                roomListAnchor.SetActive(true);
                noGames.gameObject.SetActive(false);
            }
        }
        else
        {
            noGames.gameObject.SetActive(true);
        }
    }
    public GameObject joiningOverlay;
    public void tryJoinRoom(string roomName)
    {
        List<RoomInfo> activeRooms = net.rooms();
        foreach (RoomInfo ri in activeRooms)
        {
            if(ri.Name == roomName)
            {
                if (ri.PlayerCount > 1) {
                    dontJoinRoom();
                }
                else
                {
                    Debug.Log("Got here");
                    oppMan.join(roomName);

                    joiningOverlay.SetActive(true);
                    StartCoroutine(goToGame());
                }
            }
        }
    }

    public void dontJoinRoom()
    {
        joiningOverlay.SetActive(false);
        GameObject[] roomTiles = GameObject.FindGameObjectsWithTag("RoomButton");
        foreach(GameObject go in roomTiles)
        {
            Destroy(go);
        }
        noGames.gameObject.SetActive(false);
        roomListAnchor.SetActive(false);
        hostButton.gameObject.SetActive(true);
        joinButton.gameObject.SetActive(true);
        errorJoiningText.color = new Color(errorJoiningText.color.r, errorJoiningText.color.g, errorJoiningText.color.b, 1);
        StartCoroutine(fadeText());
    }

    IEnumerator fadeText()
    {
        yield return new WaitForSeconds(2f);
        float startValue = 1;
        float time = 0;
        float duration = 2f;
        while (time < duration)
        {

            Color a = errorJoiningText.color;
            a.a = Mathf.Lerp(startValue, 0, time / duration);
            errorJoiningText.color = a;
            time += Time.deltaTime;
            yield return null;
        }
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
        GameObject audio = GameObject.Find("AudioManager");
        Destroy(audio);
        GameObject server = GameObject.Find("Server");
        Destroy(server);
        SceneManager.LoadScene("Main Menu");
    }

    public void returnToMenu()
    {
        deleteTiles();
        hostButton.gameObject.SetActive(true);
        joinButton.gameObject.SetActive(true);
        noGames.SetActive(false);
        roomName.gameObject.SetActive(false);
        submit.gameObject.SetActive(false);
        next.gameObject.SetActive(false);
        roomListAnchor.SetActive(false);
        
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

    IEnumerator goToGame()
    {
        yield return new WaitUntil(() => oppMan.getJoinedRoom() != 0);
        if (oppMan.getJoinedRoom() == -1)
        {
            dontJoinRoom();
        }
        else
        {
            SceneManager.LoadScene("WorkerSelection");
        }
    }

    public bool connected() { return oppMan.connected(); }
}
