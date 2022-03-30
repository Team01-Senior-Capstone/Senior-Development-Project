using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BooAudio : MonoBehaviour
{
    public AudioClip select;
    public static AudioClip selectSound;
    public AudioClip winSound;
    public AudioClip[] booSounds;
    static AudioClip[] _booSounds;
    // Start is called before the first frame update
    void Start()
    {
        selectSound = select;
        _booSounds = booSounds;
    }

    public static AudioClip getRandom()
    {
        System.Random random = new System.Random();
        var rngNum = random.Next(0, _booSounds.Length);
        return _booSounds[rngNum];
    }
}
