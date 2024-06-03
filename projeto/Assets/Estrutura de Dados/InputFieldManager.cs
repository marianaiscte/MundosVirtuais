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

    private bool startListening = false;

    void Start(){
        //Debug.Log(boardGameObject.name);
        boardGameObject = gameObject;
    }

    void Update(){
    if (startListening){
        if (Input.GetKeyDown(KeyCode.Space)){
            Debug.Log("TECLAS " + turnsManager.paused);
            if (turnsManager.paused)
            {
                playB.onClick.Invoke();
                Debug.Log("retomar reprodução");

            }
            else
            {
                pauseB.onClick.Invoke();
                Debug.Log("parar reprodução");
            }
        } else if (Input.GetKeyDown(KeyCode.RightArrow)){
            Debug.Log(KeyCode.RightArrow);
            NextTurn.onClick.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow)){
            Debug.Log(KeyCode.LeftArrow);
            PreviousTurn.onClick.Invoke();
        }
    }
}

    
    public void ReadStringOutput(string s){
        input = s;
        //Debug.Log(boardGameObject.name);
        Game game = xmlReader.LoadXMLToRead(s, boardGameObject);
        StartXML(game);
        Debug.Log(input);
    }

    public void StartXML(Game game){
        boardGameObject.AddComponent<TurnsManager>();
        turnsManager = boardGameObject.GetComponent<TurnsManager>();
        turnsManager.StartGame(game);
        controls();
        startListening = true;
    }

    public void controls(){
        pauseB.onClick.AddListener(turnsManager.Pause);
        playB.onClick.AddListener(turnsManager.Play);
        NextTurn.onClick.AddListener(nextTM);
        PreviousTurn.onClick.AddListener(previousTM);
    }

//esta lógica ainda não funciona bem
    private void nextTM(){
        turnsManager.isCalledByScene = true;
    }
    
    private void previousTM(){
        turnsManager.goToPrevious = true;
    }
}