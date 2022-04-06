using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class SinglePlayerManager : MonoBehaviour
{
    AudioManager am;
    public TMP_Dropdown goes_first_drop;
    public TMP_Dropdown AI_Diff_drop;
    public GameObject AI_Difficulty;

    public GameObject[] characters;

    public GameObject workerOneAnchor, workerTwoAnchor;
    public TMP_Text charName1, charName2;
    int workerOneIndex, workerTwoIndex;

    GameObject currentWorkerOne, currentWorkerTwo, game;
    Game g;

    public Button play;

    public GameObject UI_Oppoenent_Object;
    public OpponentManager oppMan;

    public GameObject waitingOverlay;
    public GameObject settingsPopUp, settingsButton, singlePlayerSettings;

    Vector3 middle_one;
    Vector3 middle_two;

    public Button world1Button;
    public Button world2Button;

    int AI_Diff;

    string worldSelected;

    public void Start()
    {
        middle_one = workerOneAnchor.transform.position;
        middle_two = workerTwoAnchor.transform.position;

        //currentWorkerOne = Instantiate(characters[0], middle_one, Quaternion.Euler(new Vector3(0, 180, 0)));
        //currentWorkerOne.transform.SetParent(workerOneAnchor.transform);

        //currentWorkerTwo = Instantiate(characters[0], middle_two, Quaternion.Euler(new Vector3(0, 180, 0)));
        //currentWorkerTwo.transform.SetParent(workerTwoAnchor.transform);

        //am = GameObject.Find("Audio Manager").GetComponent<AudioManager>();

        waitingOverlay.SetActive(false);
        worldSelected = "Main Game";
        game = GameObject.Find("Game");
        g = game.GetComponent<Game>();

        //charName1.text = currentWorkerOne.tag;
        //charName2.text = currentWorkerTwo.tag;

        if (g.netWorkGame)
        {
            settingsButton.gameObject.SetActive(false);
            System.Random rand = new System.Random();
            //g.hostGoFirst = rand.NextDouble() >= 0.5;
        }
        else
        {
            AI_Diff = 0;
            g.playerGoesFirst = true;
        }
        play.interactable = false;
        UI_Oppoenent_Object = GameObject.Find("Opponent");
        oppMan = UI_Oppoenent_Object.GetComponent<OpponentManager>();
    }

    bool oneSelected = false;
    bool twoSelected = false;

    public void openPopUp()
    {
        settingsPopUp.SetActive(true);
        if(g.netWorkGame)
        {
            singlePlayerSettings.SetActive(false);
        }
    }
    public void closePopUp()
    {
        settingsPopUp.SetActive(false);
    }

    public void selectCharacter(string name)
    {
        string tag = name.Split(' ')[0];
        string num = name.Split(' ')[1];

        if(num == "Jr.")
        {
            tag = tag + " " + num;
            num = name.Split(' ')[2];
        }

        foreach(GameObject go in characters)
        {
            if(go.tag == tag)
            {
                if(num == "1")
                {
                    Destroy(currentWorkerOne);
                    currentWorkerOne = Instantiate(go, middle_one, Quaternion.Euler(new Vector3(0, 180, 0)));
                    currentWorkerOne.transform.SetParent(workerOneAnchor.transform);
                    charName1.text = tag;
                    oneSelected = true;
                }
                else
                {
                    Destroy(currentWorkerTwo);
                    currentWorkerTwo = Instantiate(go, middle_two, Quaternion.Euler(new Vector3(0, 180, 0)));
                    currentWorkerTwo.transform.SetParent(workerTwoAnchor.transform);
                    charName2.text = tag;
                    twoSelected = true;
                }
            }
        }

        AudioManager.playCharacterSelectSound(tag);

        if(oneSelected && twoSelected)
        {
            play.interactable = true;
        }
    }

    public void selectWorld1()
    {
        world1Button.interactable = false;
        world2Button.interactable = true;
        worldSelected = "Main Game";
    }
    public void selectWorld2()
    {
        world1Button.interactable = true;
        world2Button.interactable = false;
        worldSelected = "Main Game 2";
    }

    public void AIDiffHard()
    {
        AI_Diff = 1;
    }
    public void AIDiffEasy()
    {
        AI_Diff = 0;
    }

    public void playerGoesFirst()
    {
       
       g.playerGoesFirst = true; 
    }
    public void AIGoesFirst()
    {
        g.playerGoesFirst = false;
    }

    public void returnFromWait()
    {
        Debug.Log("Sending false");
        waitingOverlay.SetActive(false);
        oppMan.getOpp().SendReady(false);
    }

    public void goBack()
    {
        //oppMan.disconnect();
        g.netWorkGame = false;
        g.updateGameType(false);
        GameObject opp = GameObject.Find("Opponent");
        Destroy(opp);
        GameObject gameObj = GameObject.Find("Game");
        Destroy(gameObj);
        GameObject audio = GameObject.Find("AudioManager");
        GameObject server = GameObject.Find("Server");
        Destroy(server);
        Destroy(audio);
        SceneManager.LoadScene("Main Menu");
    }

    IEnumerator waitSendReady()
    {
        yield return new WaitUntil(otherPersonInRoom);
        Debug.Log("Sending true");
        oppMan.getOpp().SendReady(true);
    }
    public void playGame()
    {
        if (g.netWorkGame)
        {
            StartCoroutine(waitSendReady());
            closePopUp();
            waitingOverlay.SetActive(true);
            StartCoroutine(waitForRead());
        }
        else
        {
            selectWorker1();
            selectWorker2();
            oppMan.AI_Game(AI_Diff);

            oppMan.getOpp().SendWorkerTags(g.worker1_tag, g.worker2_tag);
            SceneManager.LoadScene(worldSelected);
        }

    }

    bool otherPersonInRoom()
    {
        return oppMan.getPlayersInRoom() > 1;
    }

    public IEnumerator waitForRead()
    {
        selectWorker1();
        selectWorker2();

        Debug.Log("Line 207!");
        yield return new WaitUntil(otherPersonInRoom);
        Debug.Log("Line 209!");

        oppMan.getOpp().SendWorkerTags(g.worker1_tag, g.worker2_tag);
        Debug.Log("Line 212!");
        yield return new WaitUntil(oppMan.getOpp().GetReady);

        SceneManager.LoadScene(worldSelected);
    }

    public void moveWorkerOneForward()
    {
        Destroy(currentWorkerOne);
        workerOneIndex++; 
        if(workerOneIndex >= characters.Length)
        {
            workerOneIndex = 0;
        }

        Vector3 middle_one = workerOneAnchor.transform.position;
        currentWorkerOne = Instantiate(characters[workerOneIndex], middle_one, Quaternion.Euler(new Vector3(0, 180, 0)));
        charName1.text = currentWorkerOne.tag;
    }

    public void moveWorkerOneBack()
    {
        Destroy(currentWorkerOne);
        workerOneIndex--;
        if (workerOneIndex < 0)
        {
            workerOneIndex = characters.Length - 1;
        }

        Vector3 middle_one = workerOneAnchor.transform.position;
        currentWorkerOne = Instantiate(characters[workerOneIndex], middle_one, Quaternion.Euler(new Vector3(0, 180, 0)));
        charName1.text = currentWorkerOne.tag;
    }

    public void selectWorker1()
    {
        g.worker1_tag = currentWorkerOne.gameObject.tag;
    }

    public void selectWorker2()
    {
        g.worker2_tag = currentWorkerTwo.gameObject.tag;
    }

    public void moveWorkerTwoForward()
    {
        Destroy(currentWorkerTwo);
        workerTwoIndex++;
        if (workerTwoIndex >= characters.Length)
        {
            workerTwoIndex = 0;
        }

        Vector3 middle_two = workerTwoAnchor.transform.position;
        currentWorkerTwo = Instantiate(characters[workerTwoIndex], middle_two, Quaternion.Euler(new Vector3(0, 180, 0)));
        charName2.text = currentWorkerTwo.tag;
    }

    public void moveWorkerTwoBack()
    {
        Destroy(currentWorkerTwo);
        workerTwoIndex--;
        if (workerTwoIndex < 0)
        {
            workerTwoIndex = characters.Length - 1;
        }

        Vector3 middle_two = workerTwoAnchor.transform.position;
        currentWorkerTwo = Instantiate(characters[workerTwoIndex], middle_two, Quaternion.Euler(new Vector3(0, 180, 0)));
        charName2.text = currentWorkerTwo.tag;
    }
}
