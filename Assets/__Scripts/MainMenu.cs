using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{


    public void SinglePlayer()
    {
        SceneManager.LoadScene("SinglePlayer");
    }

    public void MultiPlayer()
    {
        SceneManager.LoadScene("Multiplayer");
    }

    public void LoadInstructions()
    {
        SceneManager.LoadScene("Instructions");
    }



    
}
