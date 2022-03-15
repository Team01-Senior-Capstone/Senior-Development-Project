using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeachAudio : MonoBehaviour
{
    public AudioClip select;
    public static AudioClip selectSound;
    public AudioClip winSound;
    public AudioClip[] peachSounds;
    static AudioClip[] _peachSounds;
    // Start is called before the first frame update
    void Start()
    {
        selectSound = select;
        _peachSounds = peachSounds;
        Debug.Log(selectSound);
    }

    public static AudioClip getRandom()
    {
        System.Random random = new System.Random();
        var rngNum = random.Next(0, _peachSounds.Length);
        return _peachSounds[rngNum];
    }
}
