using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioSource source;

    private void Awake(){
        instance = this;
        source = GetComponent<AudioSource>();
    }
    
}
