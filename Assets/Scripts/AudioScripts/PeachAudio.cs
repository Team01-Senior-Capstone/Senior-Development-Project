using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeachAudio : MonoBehaviour
{
    public AudioClip select;
    public static AudioClip selectSound;
    public AudioClip winSound;
    public AudioClip[] peachSounds;
    // Start is called before the first frame update
    void Start()
    {
        selectSound = select;
        Debug.Log(selectSound);
    }
}