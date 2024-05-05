using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class InputFieldManager : MonoBehaviour
{
    private string input; // Variável para armazenar o texto digitado pelo usuário

    public Button pauseB;
    public Button playB;

    public Button NextTurn;
    public Button PreviousTurn;

    GameObject boardGameObject;

    XMLReader xmlReader = new XMLReader();

    TurnsManager turnsManager;


    void Start(){
        boardGameObject = gameObject;
    }
    
    public void ReadStringOutput(string s){
        input = s;
        Game game = xmlReader.LoadXMLToRead(s, boardGameObject);
        StartXML(game);
        Debug.Log(input);

    }

    public void StartXML(Game game){
        boardGameObject.AddComponent<TurnsManager>();
        turnsManager = boardGameObject.GetComponent<TurnsManager>();
        turnsManager.StartGame(game);
        controls();
    }

    public void controls(){
        pauseB.onClick.AddListener(turnsManager.Pause);
        playB.onClick.AddListener(turnsManager.Play);
        NextTurn.onClick.AddListener(turnsManager.NextTurn);
        PreviousTurn.onClick.AddListener(turnsManager.PreviousTurn);
    }
  
}