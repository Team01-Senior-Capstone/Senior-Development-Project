using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WaitingText : MonoBehaviour
{
    int seconds = 60;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(updateText());
    }

    IEnumerator updateText()
    {
        if (seconds > 0)
        {
            yield return new WaitForSeconds(1);
            seconds--;
            GetComponent<TMP_Text>().text = "Opponent has " + seconds + " seconds to reconnect";
            StartCoroutine(updateText());
        }
        else
        {
            GameObject.Find("GameManager").GetComponent<GameManager>().goToMainMenu();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
