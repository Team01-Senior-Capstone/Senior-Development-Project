using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InstructionsManager : MonoBehaviour
{
    public GameObject[] Panels;
    GameObject curSlide;
    public Button backButton, forwardButton;
    int curIndex = 0;
    public GameObject anchor;

    int numSlides = 3;
    
    // Start is called before the first frame update
    public void Start()
    {
        Debug.Log("Start called");
        curSlide = Instantiate(Panels[curIndex], anchor.transform.position, Quaternion.identity);
        curSlide.transform.SetParent(anchor.transform, false);
        backButton.interactable = false;
    }

    public void goBack()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void slideForward()
    {
        if (curIndex < numSlides)
        {
            Destroy(curSlide);
            curIndex++;
            backButton.interactable = true;
            if(curIndex == numSlides)
            {
                forwardButton.interactable = false;
            }
            curSlide = Instantiate(Panels[curIndex], anchor.transform.position, Quaternion.identity);
            curSlide.transform.SetParent(anchor.transform, false);
        }
    }

    public void slideBackward()
    {

        if (curIndex > 0)
        {
            Destroy(curSlide);
            curIndex--;
            forwardButton.interactable = true;
            if(curIndex == 0)
            {
                backButton.interactable = false;
            }
            curSlide = Instantiate(Panels[curIndex], anchor.transform.position, Quaternion.identity);
            curSlide.transform.SetParent(anchor.transform, false);
        }
    }
}
