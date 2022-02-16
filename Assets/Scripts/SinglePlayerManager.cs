using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


public class SinglePlayerManager : MonoBehaviour
{

    public TMP_Dropdown drop;

    public int numCharacters = 2;
    public GameObject[] characters;

    int workerOneIndex;
    public GameObject workerOneAnchor;

    int workerTwoIndex;
    public GameObject workerTwoAnchor;

    GameObject currentWorkerOne;
    GameObject currentWorkerTwo;

    GameObject game;
    Game g;

    public GameObject UI_Oppoenent_Object;
    public OpponentManager oppMan;


    public void Start()
    {
        Vector3 middle_one = workerOneAnchor.transform.position;
        Vector3 middle_two = workerTwoAnchor.transform.position;

        currentWorkerOne = Instantiate(characters[0], middle_one, Quaternion.Euler(new Vector3(0, 180, 0)));
        currentWorkerOne.transform.SetParent(workerOneAnchor.transform);

        currentWorkerTwo = Instantiate(characters[0], middle_two, Quaternion.Euler(new Vector3(0, 180, 0)));
        currentWorkerTwo.transform.SetParent(workerTwoAnchor.transform);

        game = GameObject.Find("Game");
        g = game.GetComponent<Game>();

        if (g.netWorkGame)
        {
            drop.gameObject.SetActive(false);
            System.Random rand = new System.Random();
            //g.hostGoFirst = rand.NextDouble() >= 0.5;
        }
        else
        {

            Debug.Log(drop.value);
            if (drop.value == 0)
            {
                g.playerGoesFirst = true;
            }
            else
            {
                g.playerGoesFirst = false;
            }
        }

        UI_Oppoenent_Object = GameObject.Find("Opponent");
        oppMan = UI_Oppoenent_Object.GetComponent<OpponentManager>();
    }

    public void goesFirstChanged()
    {
        Debug.Log(drop.value);
        if (drop.value == 0)
        {
            g.playerGoesFirst = true;
        }
        else
        {
            g.playerGoesFirst = false;
        }
    }

    public void goBack()
    {
        SceneManager.LoadScene("Main Menu");
    }



    public void playGame()
    {

        //Shouldn't need to check if its a network game
        if (g.netWorkGame)
        {
            //oppMan.getOpp().SendReady(true);
            //Debug.Log("Opponent Ready? " + oppMan.getOpp().ready);
            //Debug.Log("Opponent GetReady? " + oppMan.getOpp().GetReady());

            //if (oppMan.getOpp().GetReady())
            //{
            //    SceneManager.LoadScene("Main Game");
            //}
            //else
            //{
            //    Debug.Log("Sorry other player isn't ready");
            //}

            oppMan.getOpp().SendWorkerTags(g.worker1_tag, g.worker2_tag);

            oppMan.getOpp().SendReady(true);
            Debug.Log("Opponent Ready? " + oppMan.getOpp().ready);
            Debug.Log("Opponent GetReady? " + oppMan.getOpp().GetReady());
            StartCoroutine(waitForRead());
        }
        else
        {

            selectWorker1();
            selectWorker2();
            oppMan.AI_Game();

            oppMan.getOpp().SendWorkerTags(g.worker1_tag, g.worker2_tag);
            SceneManager.LoadScene("Main Game");
        }

    }

    public IEnumerator waitForRead()
    {

        selectWorker1();
        selectWorker2();

        yield return new WaitUntil(oppMan.getOpp().GetReady);



        SceneManager.LoadScene("Main Game");
       
    }

    public void moveWorkerOneForward()
    {
        Destroy(currentWorkerOne);
        workerOneIndex++; 
        if(workerOneIndex >= numCharacters)
        {
            workerOneIndex = 0;
        }

        Vector3 middle_one = workerOneAnchor.transform.position;
        currentWorkerOne = Instantiate(characters[workerOneIndex], middle_one, Quaternion.Euler(new Vector3(0, 180, 0)));
    }
    public void moveWorkerOneBack()
    {
        Destroy(currentWorkerOne);
        workerOneIndex--;
        if (workerOneIndex < 0)
        {
            workerOneIndex = numCharacters - 1;
        }

        Vector3 middle_one = workerOneAnchor.transform.position;
        currentWorkerOne = Instantiate(characters[workerOneIndex], middle_one, Quaternion.Euler(new Vector3(0, 180, 0)));
    }

    public void selectWorker1()
    {
        g.worker1_tag = currentWorkerOne.gameObject.tag;
        Debug.Log("Selected 1: " + g.worker1_tag);
    }

    public void selectWorker2()
    {
        g.worker2_tag = currentWorkerTwo.gameObject.tag;
        Debug.Log("Selected 2: " + g.worker2_tag);
    }

    public void moveWorkerTwoForward()
    {
        Destroy(currentWorkerTwo);
        workerTwoIndex++;
        if (workerTwoIndex >= numCharacters)
        {
            workerTwoIndex = 0;
        }

        Vector3 middle_two = workerTwoAnchor.transform.position;
        currentWorkerTwo = Instantiate(characters[workerTwoIndex], middle_two, Quaternion.Euler(new Vector3(0, 180, 0)));
    }
    public void moveWorkerTwoBack()
    {
        Destroy(currentWorkerTwo);
        workerTwoIndex--;
        if (workerTwoIndex < 0)
        {
            workerTwoIndex = numCharacters - 1;
        }

        Vector3 middle_two = workerTwoAnchor.transform.position;
        currentWorkerTwo = Instantiate(characters[workerTwoIndex], middle_two, Quaternion.Euler(new Vector3(0, 180, 0)));
    }
}
