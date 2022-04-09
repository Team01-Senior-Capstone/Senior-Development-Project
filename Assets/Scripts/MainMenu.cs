using System;
using System.Collections;
using System.Net.NetworkInformation;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject game;
    public AudioClip playSound;

    public Button multiplayerButton;
    public GameObject offlineSymbol;

    Game g;

    public void Start()
    {
        g = game.GetComponent<Game>();
        g.netWorkGame = false;

        InvokeRepeating("_update", 0f, 1f);
    }

    public void quitGame()
    {
        Application.Quit();
    }

    public void _update()
    {
        Thread t = new Thread(new ThreadStart(testCon));
        t.Start();
        if (!connected)
        {
            multiplayerButton.interactable = false;
            offlineSymbol.SetActive(true);
        }
        else
        {
            multiplayerButton.interactable = true;
            offlineSymbol.SetActive(false);

        }
    }
    bool connected = true;
    public void testCon()
    {
        connected = testConnection();
    }

    bool testConnection()
    {
        try
        {
            System.Net.NetworkInformation.Ping myPing = new System.Net.NetworkInformation.Ping();
            String host = "google.com";
            byte[] buffer = new byte[32];
            int timeout = 1000;
            PingOptions pingOptions = new PingOptions();
            PingReply reply = myPing.Send(host, timeout, buffer, pingOptions);
            return (reply.Status == IPStatus.Success);
        }
        catch (Exception)
        {
            return false;
        }
    }

    public void SinglePlayer()
    {
        g.updateGameType(false);
        //StartCoroutine("playMusic");
        SceneManager.LoadScene("WorkerSelection");
        
    }

    //IEnumerator playMusic()
    //{
    //    AudioSource audio = GetComponent<AudioSource>();
    //    audio.Stop();
    //    audio.clip = playSound;
    //    audio.Play();
    //    yield return new WaitForSeconds(audio.clip.length);

    //    //audio.Play();
    //}


    public void MultiPlayer()
    {
        g.updateGameType(true);
        SceneManager.LoadScene("Multiplayer");
    }

    public void LoadInstructions()
    {
        SceneManager.LoadScene("Instructions");
    }



    
}
