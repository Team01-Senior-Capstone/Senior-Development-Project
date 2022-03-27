using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Chat : MonoBehaviour
{
    const int MAX_CHAT_SIZE = 255;
    GameManager gm;
    public GameObject content;
    public TMP_InputField txtInput;
    Network ns;
    public GameObject chatMessagePrefab;
    public GameObject scroll;
    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();

        if(gm.game.netWorkGame)
        {
            gameObject.SetActive(true);
            ns = (Network)gm.oppMan.getOpp();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
    public IEnumerator fadeChat()
    {
        yield return new WaitForSeconds(2f);
        float startValue = scroll.GetComponent<Image>().color.a;
        float time = 0;
        float duration = 2f;
        while (time < duration)
        {
           
            Color a = scroll.GetComponent<Image>().color;
            a.a = Mathf.Lerp(startValue, 0, time / duration);
            scroll.GetComponent<Image>().color = a;

            TMP_Text[] children = scroll.GetComponentsInChildren<TMP_Text>();
            Color newColor;
            foreach (TMP_Text child in children)
            {
                newColor = child.color;
                newColor.a = a.a;
                child.color = newColor;
            }
            time += Time.deltaTime;
            yield return null;
        }
        //scroll.SetActive(false);
    }
    public void sendChat()
    {
        StopCoroutine(fadeChat());
        string textToSend =txtInput.text;
        if(txtInput.text.Length > MAX_CHAT_SIZE)
        {
            textToSend = textToSend.Substring(0, MAX_CHAT_SIZE);
        }
        addChat("Me: " + textToSend);
        ns.SendChatMessage(textToSend);
        StartCoroutine(fadeChat());
        txtInput.text = "";
    }

    public IEnumerator checkForChat()
    {
        while(true)
        {
            if(ns.hasMessage())
            {
                string chatMessage = "Opp: " + ns.getChatMessage();
                addChat(chatMessage);

            }
            yield return new WaitForSeconds(.5f);
        }
    }

    void addChat(string chatMessage)
    {
        Debug.Log("Adding chat with text: " + chatMessage);
        GameObject newC = Instantiate(chatMessagePrefab);
        newC.transform.SetParent(content.transform, false);
        newC.GetComponent<TMP_Text>().text = chatMessage;
    }

    public void restoreChat()
    {
        Color newC = scroll.GetComponent<Image>().color;
        newC.a = 100.0f / 255.0f;
        scroll.GetComponent<Image>().color = newC;
        scroll.SetActive(true);
    }

    public void minimizeChat() 
    {
        Color a = scroll.GetComponent<Image>().color;
        a.a = 0;
        scroll.GetComponent<Image>().color = a;

        TMP_Text[] children = scroll.GetComponentsInChildren<TMP_Text>();
        Color newColor;
        foreach (TMP_Text child in children)
        {
            newColor = child.color;
            newColor.a = a.a;
            child.color = newColor;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            sendChat();
        }
        if(Input.GetKeyDown(KeyCode.Return))
        {
            sendChat();
        }
    }
}
