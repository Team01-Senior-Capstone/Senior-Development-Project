using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InstructionsManager : MonoBehaviour
{
    public GameObject[] panels;
    int curIndex = 0;
    // Start is called before the first frame update
    public void goBack()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void goForward()
    {
        //Delete the old panel
        //increase the index
        //Instantiate panels[curIndex]
    }

}
