using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioSource source;
    //classe que permite o manuseamento de som da parte 2
    //associada a um GameObject empty (chamado AudioManager)
    private void Awake(){
        instance = this;
        source = GetComponent<AudioSource>();
    }
    
}
