using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioParte2 : MonoBehaviour
{//Classe que trata dos eventos sonoros possíveis na parte 2 
  [SerializeField] AudioClip[] sounds = default; //os áudios estao associados a
  //todos os prefabs que produzem som (no caso o defender,attacker e o main character)
  //e usam uma instancia do AudioManager

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
