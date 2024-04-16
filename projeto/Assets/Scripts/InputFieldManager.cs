using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class InputFieldManager : MonoBehaviour
{
    private string input; // Variável para armazenar o texto digitado pelo usuário

    void Start(){

    }

    void Update(){

    }
    
    public void ReadStringOutput(string s){
        input = s;
        Debug.Log(input);

    }
  
}