using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    

    static AudioSource audioSource;
    public AudioClip mainMenu;
    public AudioClip mainGame;
    public AudioClip workerSelection;

    public void Awake()
    {

        audioSource = GetComponent<AudioSource>();
    }
    public void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    void OnEnable()
    {
        //Tell our 'OnLevelFinishedLoading' function to start listening for a scene change as soon as this script is enabled.
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable()
    {
        //Tell our 'OnLevelFinishedLoading' function to stop listening for a scene change as soon as this script is disabled. Remember to always have an unsubscription for every delegate you subscribe to!
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }
    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        playBackgroundMusic(scene.name);
    }
    public void playBackgroundMusic(string name)
    {
        if(name == "Main Menu")
        {
            audioSource.clip = mainMenu;
            
        }
        else if(name == "WorkerSelection")
        {
            audioSource.clip = workerSelection;
        }
        else if(name == "Main Game")
        {
            audioSource.clip = mainGame;
        }
        audioSource.loop = true;
        audioSource.Play();
    }
    public static void playMarioSoundRandom()
    {

    }

    public static void playCharacterSelectSound(string tag)
    {
        AudioClip playSound;
        if(tag == "Mario")
        {
            playSound = MarioAudio.selectSound;
        }
        else if(tag == "Luigi")
        {
            playSound = LuigiAudio.selectSound;
        }
        else
        {
            playSound = YoshiAudio.selectSound;
        }
        Debug.Log(audioSource);
        Debug.Log(playSound);
        audioSource.PlayOneShot(playSound, 3);
    }
}
