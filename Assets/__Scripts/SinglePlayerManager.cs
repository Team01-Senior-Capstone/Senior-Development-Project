using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SinglePlayerManager : MonoBehaviour
{
    public void goBack()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
