using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YoshiAudio : MonoBehaviour
{
    public AudioClip select;
    public static AudioClip selectSound;
    public AudioClip winSound;
    public AudioClip[] yoshiSounds;
    static AudioClip[] _yoshiSounds;
    // Start is called before the first frame update
    void Start()
    {
        selectSound = select;
        _yoshiSounds = yoshiSounds;
    }

    public static AudioClip getRandom()
    {
        System.Random random = new System.Random();
        var rngNum = random.Next(0, _yoshiSounds.Length);
        return _yoshiSounds[rngNum];
    }

}
