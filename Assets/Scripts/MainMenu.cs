using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject game;
    public AudioClip playSound;

    Game g;

    public void Start()
    {
        g = game.GetComponent<Game>();
        g.netWorkGame = false;

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
