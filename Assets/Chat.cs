using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
        float startValue = scroll.GetComponent<Renderer>().material.color.a;
        float time = 0;
        float duration = 2f;
        while (time < duration)
        {
            //Color newColor = r.color;
            //Color barColor = loadingBar.GetComponent<MeshRenderer>().material.color;
            Color a = scroll.GetComponent<Material>().color;
            a.a = Mathf.Lerp(startValue, 1, time / duration);
            ///barColor.a = newColor.a;
            //loadingBar.GetComponent<MeshRenderer>().material.color = barColor;
            //r.color = newColor;
            scroll.GetComponent<Renderer>().material.color = a;
            time += Time.deltaTime;
            yield return null;
        }
    }
    public void sendChat()
    {
        StopCoroutine(fadeChat());
        string textToSend =txtInput.text;
        if(txtInput.text.Length > MAX_CHAT_SIZE)
        {
            textToSend = textToSend.Substring(0, MAX_CHAT_SIZE);
        }
        addChat(textToSend);
        ns.SendChatMessage(textToSend);
        StartCoroutine(fadeChat());
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
        GameObject newC = Instantiate(chatMessagePrefab);
        newC.transform.SetParent(content.transform);
        newC.GetComponent<TMP_Text>().text = chatMessage;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
