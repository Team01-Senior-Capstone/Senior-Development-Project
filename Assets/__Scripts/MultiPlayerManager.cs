using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using UnityEngine.SceneManagement;

public class MultiPlayerManager : MonoBehaviour
{
    public Button hostButton;
    public Button joinButton;
    public TMP_InputField roomName;

    public void Awake()
    {
        roomName.gameObject.SetActive(false);
    }

    public void host()
    {
        hostButton.gameObject.SetActive(false);
        joinButton.gameObject.SetActive(false);
        roomName.gameObject.SetActive(true);
    }

    public void goBack()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
