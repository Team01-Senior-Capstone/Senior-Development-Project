using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LuigiAudio : MonoBehaviour
{
    public AudioClip select;
    public static AudioClip selectSound;
    public AudioClip winSound;
    public AudioClip[] luigiSounds;
    static AudioClip[] _luigiSounds;
    // Start is called before the first frame update
    void Start()
    {
        selectSound = select;
        _luigiSounds = luigiSounds;
        Debug.Log(selectSound);
    }
    public static AudioClip getRandom()
    {
        System.Random random = new System.Random();
        var upperBound = 200;
        var rngNum = random.Next(0, _luigiSounds.Length);
        return _luigiSounds[rngNum];
    }
}
