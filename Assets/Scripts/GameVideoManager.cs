using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameVideoManager : MonoBehaviour
{

    public GameObject mario, pipe1, pipe2, pipe3;
    Vector3 pipeOneBottom, pipeOneTop, pipeTwoBottom, pipeTwoTop, pipeThreeBottom, pipeThreeTop;

    // Start is called before the first frame update
    void Start()
    {
        //MainMenu();
        pipeOneBottom = new Vector3(pipe1.transform.position.x, pipe1.transform.position.y - 1, pipe1.transform.position.z);
        pipeOneTop = new Vector3(pipe1.transform.position.x, pipe1.transform.position.y + 1, pipe1.transform.position.z);
        pipeTwoBottom = new Vector3(pipe2.transform.position.x, pipe2.transform.position.y - 1.5f, pipe2.transform.position.z);
        pipeTwoTop = new Vector3(pipe2.transform.position.x, pipe2.transform.position.y + 1.5f, pipe2.transform.position.z);
        pipeThreeBottom = new Vector3(pipe3.transform.position.x, pipe3.transform.position.y - 1.5f, pipe3.transform.position.z);
        pipeThreeTop = new Vector3(pipe3.transform.position.x, pipe3.transform.position.y + 1.5f, pipe3.transform.position.z);
        StartCoroutine(moveTo(pipeOneBottom, pipeTwoBottom, pipeTwoTop, true));
    }

    public void MainMenu() {

        SceneManager.LoadScene("Main Menu");
    }

    IEnumerator moveDown(Vector3 posBottom)
    {
        yield return new WaitForSeconds(.5f);
        while (mario.transform.position != posBottom)
        {

            mario.transform.position = Vector3.MoveTowards(mario.transform.position, posBottom, Time.deltaTime * 2f);
            yield return new WaitForSeconds(.001f);
        }
        yield return new WaitForSeconds(1f);
        MainMenu();
    }

    IEnumerator moveTo(Vector3 posBottom, Vector3 newPosBottom, Vector3 newPosTop, bool repeat)
    {
        yield return new WaitForSeconds(.5f);
        //AudioManager.playPipeSound();
        while (mario.transform.position != posBottom)
        {

            mario.transform.position = Vector3.MoveTowards(mario.transform.position, posBottom, Time.deltaTime * 2f);
            yield return new WaitForSeconds(.001f);
        }
        //anim.Play("Wait");
        mario.transform.position = newPosBottom;
        //yield return new WaitForSeconds(.5f);
        //yield return new WaitForSeconds(.75f);
        while (mario.transform.position != newPosTop)
        {

            mario.transform.position = Vector3.MoveTowards(mario.transform.position, newPosTop, Time.deltaTime * 4f);
            yield return new WaitForSeconds(.001f);
        }

        if(repeat)
        {

            StartCoroutine(moveTo(pipeTwoBottom, pipeThreeBottom, pipeThreeTop, false));
        }
        else
        {
            StartCoroutine(moveDown(pipeThreeBottom));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
