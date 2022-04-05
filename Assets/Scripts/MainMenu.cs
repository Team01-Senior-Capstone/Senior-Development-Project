using System;
using System.Collections;
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

    }

    public void quitGame()
    {
        Application.Quit();
    }

    public void Update()
    {
        StartCoroutine(checkInternetConnection((isConnected) =>
        {
            if (!isConnected)
            {
                multiplayerButton.interactable = false;
                offlineSymbol.SetActive(true);
            }
            else
            {
                multiplayerButton.interactable = true;
                offlineSymbol.SetActive(false);
            }
        }));
    }
    public static IEnumerator checkInternetConnection(Action<bool> syncResult)
    {
        const string echoServer = "https://www.harding.edu/";

        bool result;
        using (var request = UnityWebRequest.Head(echoServer))
        {
            request.timeout = 5;
            yield return request.SendWebRequest();
            result = !request.isNetworkError && !request.isHttpError && request.responseCode == 200;
        }
        syncResult(result);
    }

    //IEnumerator checkInternetConnection(Action<bool> action)
    //{
    //    WWW www = new WWW("https://google.com");
    //    yield return www;
    //    if (www.error != null)
    //    {
    //        action(false);
    //    }
    //    else
    //    {
    //        action(true);
    //    }
    //}

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
