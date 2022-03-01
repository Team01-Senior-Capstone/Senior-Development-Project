using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LuigiAudio : MonoBehaviour
{
    public AudioClip select;
    public static AudioClip selectSound;
    public AudioClip winSound;
    public AudioClip[] luigiSounds;
    // Start is called before the first frame update
    void Start()
    {
        selectSound = select;
        Debug.Log(selectSound);
    }

}
