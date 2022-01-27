using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using UnityEngine.SceneManagement;
using System.Linq;

public class MultiPlayerManager : MonoBehaviour
{
    public Button hostButton;
    public Button joinButton;
    public Button submit;
    public TMP_InputField roomName;

    private static System.Random random = new System.Random();

    public int roomNameLength = 7;

    public string getRandomString()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, roomNameLength)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    public void Awake()
    {
        roomName.gameObject.SetActive(false);
    }

    public void host()
    {
        hostButton.gameObject.SetActive(false);
        joinButton.gameObject.SetActive(false);
        roomName.gameObject.SetActive(true);
        roomName.placeholder.GetComponent<Text>().text = getRandomString();
        submit.gameObject.SetActive(true);
    }

    public void goBack()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void onSumbit()
    {
        Debug.Log(roomName.text);
    }
}
