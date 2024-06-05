using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class InputFieldManager : MonoBehaviour
{
    private string input; // Variável para armazenar o texto digitado pelo usuário

    public Button pauseB;
    public Button playB;
    public Button restart;

    public Button NextTurn;
    public Button PreviousTurn;

    public GameObject gameInfo;
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI turnCountText;
    public TextMeshProUGUI gameStatusText;

    public GameObject changeScenePanel;
    public Button yesChange;
    public Button noChange;

    GameObject boardGameObject;

    XMLReader xmlReader = new XMLReader();

    TurnsManager turnsManager;

    private bool startListening = false;

    public string terraintypeString;
    public string nameTabuleiro;

    void Start(){
        //Debug.Log(boardGameObject.name);
        boardGameObject = gameObject;
        nameTabuleiro = boardGameObject.name;
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
        turnsManager.StartGame(game,boardGameObject);
        controls();
        startListening = true;
    }

    public void controls(){
        pauseB.onClick.AddListener(turnsManager.Pause);
        playB.onClick.AddListener(turnsManager.Play);
        NextTurn.onClick.AddListener(nextTM);
        PreviousTurn.onClick.AddListener(previousTM);
        restart.onClick.AddListener(() => {
            turnsManager.state.currentTurn = 0;
            turnsManager.PreviousTurn();
        });
    }

//esta lógica ainda não funciona bem
    private void nextTM(){
        turnsManager.isCalledByScene = true;
    }
    
    private void previousTM(){
        turnsManager.goToPrevious = true;
    }

    public void UpdateUI(string playerName, int turnCount, string gameStatus)
    {
        playerNameText.text = "Player: " + playerName;
        turnCountText.text = "Turn: " + turnCount;
        gameStatusText.text = gameStatus;
    }

    public void showInfo(){
        if (gameInfo.activeSelf){
            gameInfo.SetActive(false);
        }else{
            gameInfo.SetActive(true);
        }
    }

    public void changeScene(string tiletype){
        //turnsManager.Pause();
        changeScenePanel.SetActive(true);
        terraintypeString = tiletype;
        GameObject gameObjectTerrainType = GameObject.Find("terrainType");
        if (gameObjectTerrainType != null)
        {
            // Acesse a variável pública contendo a string
            terrainTypeHolder stringTerrainComp = gameObjectTerrainType.GetComponent<terrainTypeHolder>();
            
            if (stringTerrainComp != null)
            {
               stringTerrainComp.terrainType = terraintypeString;
            }
            else
            {
                Debug.LogWarning("Componente StringHolder não encontrado no GameObject.");
            }
        }
        else
        {
            Debug.LogWarning("GameObject não encontrado com o nome especificado.");
        }
    }
    


    public void changeSceneYES(){
        //Debug.Log("vou entrar na scene");
        SceneManager.LoadScene("Parte 2 terrain");
    }

    public void changeSceneNO(){
        changeScenePanel.SetActive(false);
        //turnsManager.Play();
    }

}