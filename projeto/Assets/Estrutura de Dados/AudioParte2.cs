using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioParte2 : MonoBehaviour
{
  [SerializeField] AudioClip[] sounds = default;

  void sword(){

    AudioManager.instance.source.PlayOneShot(sounds[0]);

  }

  void walk(){
        AudioManager.instance.source.PlayOneShot(sounds[1]);
  }

  void run(){
    AudioManager.instance.source.PlayOneShot(sounds[2]);

  }
  

  void block(){
    AudioManager.instance.source.PlayOneShot(sounds[3]);
  }

  void die(){
    AudioManager.instance.source.PlayOneShot(sounds[4]);

  }

  void dance(){
    AudioManager.instance.source.PlayOneShot(sounds[5]);
  }
    
}
