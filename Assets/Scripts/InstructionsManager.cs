using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InstructionsManager : MonoBehaviour
{
    public GameObject[] panels;
    GameObject curSlide;
    int curIndex = 0;
    
    // Start is called before the first frame update
    public void Start()
    {

    }

    public void goBack()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void slideForward()
    { 
        Destroy(curSlide);
        curIndex++;
        curSlide = Instantiate(panels[curIndex]);
    }

    
}
