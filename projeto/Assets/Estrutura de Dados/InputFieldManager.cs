using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class InputFieldManager : MonoBehaviour
{
    // inicializacao de varios elementos do hud
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

    public Button clearB;

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
        clearB.GetComponent<CanvasRenderer>().SetAlpha(0.0f); // Tornar visível
        clearB.interactable = false;
    }

    void Update(){
    // usar o teclado para controlar o hud (colocar os jogos em causa, retom-los, avancar uma jogada e andar para tras uma jogada)
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
            //Debug.Log(KeyCode.RightArrow);
            NextTurn.onClick.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow)){
            //Debug.Log(KeyCode.LeftArrow);
            PreviousTurn.onClick.Invoke();
        }
    }
}

    // Método para ler a entrada de string e inicializar o jogo
    public void ReadStringOutput(string s){
        input = s;
        //Debug.Log(boardGameObject.name);
        Game game = xmlReader.LoadXMLToRead(s, boardGameObject);
        StartXML(game);
        //Debug.Log(input);
    }

    // Método para iniciar o jogo a partir dos dados carregados
    public void StartXML(Game game){
        boardGameObject.AddComponent<TurnsManager>();
        turnsManager = boardGameObject.GetComponent<TurnsManager>();
        turnsManager.StartGame(game,boardGameObject);
        controls();
        startListening = true;
    }

    // Método que liga os botoes as funcoes do turns manager que controlam o jogo
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

    private void nextTM(){
        turnsManager.isCalledByScene = true;
    }
    
    private void previousTM(){
        turnsManager.goToPrevious = true;
    }


    // dar update às informacoes do jogo e no caso do jogo ter acabado, surgir um botao para resetar a mesa (poder comecar um novo jogo)
    public void UpdateUI(string playerName, int turnCount, string gameStatus)
    {
        playerNameText.text = "Player: " + playerName;
        turnCountText.text = "Turn: " + turnCount;
        gameStatusText.text = gameStatus;
        Debug.Log("irei entrar");
        if(gameStatus=="Game Over" && turnsManager.state.currentTurn >= turnsManager.turnsList.Count - 1){
            clearB.GetComponent<CanvasRenderer>().SetAlpha(1.0f); // Tornar visível
            clearB.interactable = true; // Tornar interativo
            TableSpawner scriptTS = GetComponentInParent<TableSpawner>();
            clearB.onClick.AddListener(() => {
                scriptTS.startNewGame();
            });
        }
    }


    // mete ativo o painel com as informacoes do jogo
    public void showInfo(){
        if (gameInfo.activeSelf){
            gameInfo.SetActive(false);
        }else{
            gameInfo.SetActive(true);
        }
    }

    // mudança para a scene 2 (a das batalhas)
    public void changeScene(string tiletype){
        //turnsManager.Pause();
        changeScenePanel.SetActive(true);
        terraintypeString = tiletype;
        GameObject gameObjectTerrainType = GameObject.Find("terrainType");
        if (gameObjectTerrainType != null)
        {
            terrainTypeHolder stringTerrainComp = gameObjectTerrainType.GetComponent<terrainTypeHolder>();
            
            if (stringTerrainComp != null)
            {
                // Define a propriedade terrainType do componente com o valor de terraintypeString
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
    

    // aceitar a mudanca para a scene da bathalha
    public void changeSceneYES(){
        //Debug.Log("vou entrar na scene");
        SceneManager.LoadScene("Parte 2 terrain");
    }

    // nao aceitar a mudanca de scene
    public void changeSceneNO(){
        changeScenePanel.SetActive(false);
        //turnsManager.Play();
    }

}