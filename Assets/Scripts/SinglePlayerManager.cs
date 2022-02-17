using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


public class SinglePlayerManager : MonoBehaviour
{

    public TMP_Dropdown drop;

    const int NUM_CHARACTERS = 2;
    public GameObject[] characters;

    public GameObject workerOneAnchor, workerTwoAnchor;
    int workerOneIndex, workerTwoIndex;

    GameObject currentWorkerOne, currentWorkerTwo, game;
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

        if (g.netWorkGame)
        {
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

        oppMan.getOpp().SendWorkerTags(g.worker1_tag, g.worker2_tag);

        yield return new WaitUntil(oppMan.getOpp().GetReady);

        SceneManager.LoadScene("Main Game");
    }

    public void moveWorkerOneForward()
    {
        Destroy(currentWorkerOne);
        workerOneIndex++; 
        if(workerOneIndex >= NUM_CHARACTERS)
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
            workerOneIndex = NUM_CHARACTERS - 1;
        }

        Vector3 middle_one = workerOneAnchor.transform.position;
        currentWorkerOne = Instantiate(characters[workerOneIndex], middle_one, Quaternion.Euler(new Vector3(0, 180, 0)));
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
        if (workerTwoIndex >= NUM_CHARACTERS)
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
            workerTwoIndex = NUM_CHARACTERS - 1;
        }

        Vector3 middle_two = workerTwoAnchor.transform.position;
        currentWorkerTwo = Instantiate(characters[workerTwoIndex], middle_two, Quaternion.Euler(new Vector3(0, 180, 0)));
    }
}
