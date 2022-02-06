using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SinglePlayerManager : MonoBehaviour
{
    public int numCharacters = 2;
    public GameObject[] characters;

    int workerOneIndex;
    public GameObject workerOneAnchor;

    int workerTwoIndex;
    public GameObject workerTwoAnchor;

    GameObject currentWorkerOne;
    GameObject currentWorkerTwo;


    public void Start()
    {
        Vector3 middle_one = workerOneAnchor.transform.position;
        Vector3 middle_two = workerTwoAnchor.transform.position;

        currentWorkerOne = Instantiate(characters[0], middle_one, Quaternion.Euler(new Vector3(0, 180, 0)));
        currentWorkerOne.transform.SetParent(workerOneAnchor.transform);

        currentWorkerTwo = Instantiate(characters[0], middle_two, Quaternion.Euler(new Vector3(0, 180, 0)));
        currentWorkerTwo.transform.SetParent(workerTwoAnchor.transform);
    }

    public void goBack()
    {
        SceneManager.LoadScene("Main Menu");
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
