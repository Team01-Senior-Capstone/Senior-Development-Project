using UnityEngine;
using UnityEngine.SceneManagement;

public class GameVideoManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MainMenu();
    }

    public void MainMenu() {

        SceneManager.LoadScene("Main Menu");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
