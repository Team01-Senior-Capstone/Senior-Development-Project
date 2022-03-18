using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InstructionsManager : MonoBehaviour
{
    public GameObject[] Panels;
    GameObject curSlide;
    int curIndex = 0;
    public GameObject anchor;
    
    // Start is called before the first frame update
    public void Start()
    {
        curSlide = Instantiate(Panels[curIndex], anchor.transform.position, Quaternion.identity);
    }

    public void goBack()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void slideForward()
    {
        if (curIndex < 3)
        {
            Destroy(curSlide);
            curIndex++;
            curSlide = Instantiate(Panels[curIndex], anchor.transform.position, Quaternion.identity);
            curSlide.transform.SetParent(anchor.transform);
        }
    }

    public void slideBackward()
    {
        if (curIndex > 0)
        {
            Destroy(curSlide);
            curIndex--;
            curSlide = Instantiate(Panels[curIndex], anchor.transform.position, Quaternion.identity);
            curSlide.transform.SetParent(anchor.transform);
        }
    }
}
