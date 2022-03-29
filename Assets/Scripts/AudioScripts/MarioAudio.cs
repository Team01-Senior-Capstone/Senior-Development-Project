using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarioAudio : MonoBehaviour
{
    public AudioClip select;
    public static AudioClip selectSound;
    public static AudioClip[] _marioSounds;
    public AudioClip winSound;
    public AudioClip[] marioSounds;
    // Start is called before the first frame update
    void Start()
    {
        selectSound = select;
        _marioSounds = marioSounds;
        Debug.Log(selectSound);
    }

    public static AudioClip getRandom()
    {
        System.Random random = new System.Random();
        var rngNum = random.Next(0, _marioSounds.Length);
        return _marioSounds[rngNum];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
