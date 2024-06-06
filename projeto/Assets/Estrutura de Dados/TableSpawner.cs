using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//Script que gera uma mesa para ser utilizada num jogo
public class TableSpawner : MonoBehaviour
{
    public GameObject gameTablePrefab; // O prefab da mesa de jogo
    public Transform tablePosition; // Referência ao GameObject vazio
    GameObject table;
    
    void Start()
    {
        // Instancia as mesas assim que o jogo começa: 
        //tantas mesas quanto os objetos empty com este script associado
        table = InstantiateTable();
        linkSetUp(table);
    }

    public GameObject InstantiateTable(){
        // Instancia a primeira mesa
        GameObject table1 = Instantiate(gameTablePrefab, tablePosition.position, tablePosition.rotation);
        table1.transform.SetParent(tablePosition);
        return table1;
    }

// Definir as ligações e referências entre os vários scripts e elementos do prefab e da cena
    public void linkSetUp(GameObject table){
        // Encontra o objeto Tabuleiro no prefab
        Transform Tabuleiro = table.transform.Find("Tabuleiro");

        // Vai ter o seguinte script associado, ao qual temos de associar GameControls da cena
        InputFieldManager ifm = Tabuleiro.GetComponent<InputFieldManager>();
        GameObject GameControls = GameObject.Find("GameControls");

        // Encontra os controlos do jogo e atribui ao botão respetivo do script de cima
        GameObject pauseButtonTransform = GameObject.Find("Pause");
        ifm.pauseB = pauseButtonTransform.GetComponent<Button>();
        GameObject playButtonTransform = GameObject.Find("Play");
        ifm.playB = playButtonTransform.GetComponent<Button>();
        GameObject nextButtonTransform = GameObject.Find("NextTurn");
        ifm.NextTurn = nextButtonTransform.GetComponent<Button>();
        GameObject previousButtonTransform = GameObject.Find("PreviousTurn");
        ifm.PreviousTurn = previousButtonTransform.GetComponent<Button>();
        GameObject restartButtonTransform = GameObject.Find("Restart");
        ifm.restart = restartButtonTransform.GetComponent<Button>();

        // Encontra o canvas dentro do prefab, que vai ser parent dos dois elementos UI abaixo
        Transform canva = table.transform.Find("Canvas");
        Transform screen = canva.transform.Find("InitialScreen");
        Transform path = screen.transform.Find("path");
        TMP_InputField p = path.GetComponent<TMP_InputField>();

        // Encontra o show dentro do prefab, que vai ser parent do botão para alterar a vista
        Transform show = canva.transform.Find("show");
        Transform camM = show.transform.Find("CameraSwitch");
        Transform topview = table.transform.Find("TopViewCamera");
        GameObject topviewcam = topview.GetComponent<GameObject>();
 
        Button switchCam = camM.GetComponent<Button>();
        UiManager uim = topview.GetComponent<UiManager>();
      
        // Associa as câmeras da cena ao script associado ao botão acima
        uim.mainCamera = GameObject.Find("Main Camera");
        //uim.miniMap = minimap;

        // Ação a executar ao pressionar o botão acima
        switchCam.onClick.AddListener(changescene);
        void changescene(){
            uim.changeCamera();
        }

        GameObject showbutton = show.gameObject;

        if(gameObject.name == "TableL"){
            showbutton.transform.localPosition = new Vector3(-1678f, 0f, 0f);
        }
        showbutton.SetActive(false);

        // Ação a executar quando se carrega no enter ao introduzir o caminho
        p.onEndEdit.AddListener(OnEnd);
        void OnEnd(string input){
                showbutton.SetActive(true);
            }

    }

// Função para começar um jogo novo numa mesa onde acabou um jogo
    public void startNewGame(){
        Destroy(table);
        table = InstantiateTable();
        linkSetUp(table);
    }

}

