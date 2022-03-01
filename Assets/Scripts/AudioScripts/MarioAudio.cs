using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarioAudio : MonoBehaviour
{
    public AudioClip select;
    public static AudioClip selectSound;
    public AudioClip winSound;
    public AudioClip[] marioSounds;
    // Start is called before the first frame update
    void Start()
    {
        selectSound = select;
        Debug.Log(selectSound);
    }

    

    // Update is called once per frame
    void Update()
    {
        
    }
}
