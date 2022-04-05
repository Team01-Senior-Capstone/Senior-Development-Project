using System;
using System.Collections;
using UnityEngine;

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
