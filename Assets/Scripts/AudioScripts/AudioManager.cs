using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{

    private static AudioManager _instance;

    public static AudioManager Instance { get { return _instance; } }

  
    public AudioClip _pipeBuildSound, _pipeSound, _winSound, _loseSound, _walk;
    public static AudioClip pipeBuildSound, pipeSound, winSound, loseSound, walk;
    static AudioSource audioSource;
    public AudioClip mainMenu;
    public AudioClip mainGame;
    public AudioClip network;
    public AudioClip workerSelection;


    public void Awake()
    {
        //if (_instance != null && _instance != this)
        //{
        //    Destroy(this.gameObject);
        //}
        //else
        //{
        //    _instance = this;
        //}
        audioSource = GetComponent<AudioSource>();
    }

    public static void changeVolume(float volume)
    {
        audioSource.volume = volume;
    }

    public void Start()
    {
        pipeBuildSound = _pipeBuildSound;
        pipeSound = _pipeSound;
        winSound = _winSound;
        loseSound = _loseSound;
        walk = _walk;
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
        audioSource.loop = false;
        audioSource.Stop();
        playBackgroundMusic(scene.name);
    }
    public void playBackgroundMusic(string name)
    {

        if (name == "Main Menu")
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
        else if(name == "MultiPlayer")
        {
            audioSource.clip = network;
        }
        audioSource.loop = true;
        audioSource.Play();
    }
    public static void playCharacterRandom(string tag) 
    {
        AudioClip playSound;
        if (tag == "Mario")
        {
            playSound = MarioAudio.getRandom();
        }
        else if (tag == "Luigi")
        {
            playSound = LuigiAudio.getRandom();
        }
        else if (tag == "Yoshi")
        {
            playSound = YoshiAudio.getRandom();
        }
        else if (tag == "Peach")
        {
            playSound = PeachAudio.getRandom();
        }
        else if (tag == "Boo")
        {
            playSound = BooAudio.getRandom();
        }
        else
        {
            playSound = BowserJrAudio.getRandom();
        }
        Debug.Log(audioSource);
        Debug.Log(playSound);
        audioSource.PlayOneShot(playSound, 3);
    }

    public static void playCharacterSelectSound(string tag)
    {
        AudioClip playSound;
        if (tag == "Mario")
        {
            playSound = MarioAudio.selectSound;
        }
        else if (tag == "Luigi")
        {
            playSound = LuigiAudio.selectSound;
        }
        else if (tag == "Yoshi")
        {
            playSound = YoshiAudio.selectSound;
        }
        else if (tag == "Peach")
        {
            playSound = PeachAudio.selectSound;
        }
        else if(tag == "Boo")
        {
            playSound = BooAudio.selectSound;
        }
        else
        {
            playSound = BowserJrAudio.selectSound;
        }
        Debug.Log(audioSource);
        Debug.Log(playSound);
        audioSource.PlayOneShot(playSound, 3);
    }


    public static void playPipeSound()
    {
        AudioClip clip = pipeSound;
        audioSource.PlayOneShot(clip, 3);
    }

    public static void playBuildSound()
    {
        audioSource.PlayOneShot(pipeBuildSound, 1);
        //pipeBuildSound.Play();
    }

    public static void playWalkSound()
    {
        audioSource.PlayOneShot(walk, 3);
        //pipeBuildSound.Play();
    }

    public static void playWinSound()
    {
        audioSource.Stop();
        audioSource.PlayOneShot(winSound, 5);
    }


    public static void playLoseSound()
    {
        audioSource.Stop();
        audioSource.PlayOneShot(loseSound, 5);
    }
}
