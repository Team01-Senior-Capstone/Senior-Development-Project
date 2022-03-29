using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowserJrAudio : MonoBehaviour
{
    public AudioClip select;
    public static AudioClip selectSound;
    public AudioClip winSound;
    public AudioClip[] bowserJrSounds;
    static AudioClip[] _bowserJrSounds;
    // Start is called before the first frame update
    void Start()
    {
        selectSound = select;
        _bowserJrSounds = bowserJrSounds;
    }

    public static AudioClip getRandom()
    {
        System.Random random = new System.Random();
        var rngNum = random.Next(0, _bowserJrSounds.Length);
        return _bowserJrSounds[rngNum];
    }
}
