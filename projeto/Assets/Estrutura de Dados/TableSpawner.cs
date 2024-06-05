using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class TableSpawner : MonoBehaviour
{
    public GameObject gameTablePrefab; // O prefab da mesa de jogo
    public Transform tablePosition; // Referência ao primeiro GameObject vazio

    GameObject table;
    
    /*public Button pauseB;
    public Button playB;
    public Button NextTurn;
    public Button PreviousTurn;
    public Button Restart;
    public GameObject mainCamera;
    public GameObject topViewCamera;
    public GameObject MiniMap;
    public GameObject GameControls; */

    void Start()
    {
        table = InstantiateTable();
        
        linkSetUp(table);
    }

    public GameObject InstantiateTable(){
        // Instancia a primeira mesa
        GameObject table1 = Instantiate(gameTablePrefab, tablePosition.position, tablePosition.rotation);
        table1.transform.SetParent(tablePosition);
        Debug.Log("criei1");
        return table1;
    }

    public void linkSetUp(GameObject table){

        Transform Tabuleiro = table.transform.Find("Tabuleiro");
        InputFieldManager ifm = Tabuleiro.GetComponent<InputFieldManager>();
        GameObject GameControls = GameObject.Find("GameControls");
        Debug.Log(ifm==null);
        GameObject pauseButtonTransform = GameObject.Find("Pause");
        Debug.Log(pauseButtonTransform==null);
        ifm.pauseB = pauseButtonTransform.GetComponent<Button>();
        GameObject playButtonTransform = GameObject.Find("Play");
        ifm.playB = playButtonTransform.GetComponent<Button>();
        GameObject nextButtonTransform = GameObject.Find("NextTurn");
        ifm.NextTurn = nextButtonTransform.GetComponent<Button>();
        GameObject previousButtonTransform = GameObject.Find("PreviousTurn");
        ifm.PreviousTurn = previousButtonTransform.GetComponent<Button>();
        GameObject restartButtonTransform = GameObject.Find("Restart");
        ifm.restart = restartButtonTransform.GetComponent<Button>();

        GameObject buttons = GameObject.Find("Buttons");
        Button[] bs = buttons.GetComponents<Button>();
        foreach(Button b in bs){
            Debug.Log(b==null);
            b.GetComponent<CanvasRenderer>().SetAlpha(0.0f); // Tornar visível
            b.interactable = false; // Tornar interativo
        }

        Transform canva = table.transform.Find("Canvas");
        Transform screen = canva.transform.Find("InitialScreen");
        Transform path = screen.transform.Find("path");
        TMP_InputField p = path.GetComponent<TMP_InputField>();
        Debug.Log(path);
        Debug.Log(p==null);

        Transform camM = canva.transform.Find("CameraSwitch");
        Transform topview = table.transform.Find("TopViewCamera");
        Camera topviewcam = topview.GetComponent<Camera>();
        Debug.Log(camM==null);
        Button switchCam = camM.GetComponent<Button>();
        UiManager uim = topview.GetComponent<UiManager>();
        switchCam.onClick.AddListener(changescene);
        void changescene(){
            uim.changeCamera();
        }
        Debug.Log(uim==null);
        uim.mainCamera = GameObject.Find("Main Camera");
        uim.miniMap = GameObject.Find("MiniMap");

        Transform showinfo = canva.transform.Find("ShowGameInfo");
        Button showgameinfo = showinfo.GetComponent<Button>();
        showgameinfo.onClick.AddListener(ifm.showInfo);

        p.onEndEdit.AddListener(OnEnd);
        void OnEnd(string input){
            if (buttons != null){
                foreach(Button b in bs){
                    b.GetComponent<CanvasRenderer>().SetAlpha(1.0f); // Tornar visível
                    b.interactable = true; // Tornar interativo
                }
                switchCam.GetComponent<CanvasRenderer>().SetAlpha(1.0f); // Tornar visível
                switchCam.interactable = true; // Tornar interativo
                showgameinfo.GetComponent<CanvasRenderer>().SetAlpha(1.0f); // Tornar visível
                showgameinfo.interactable = true; // Tornar interativo
            }else {
                Debug.LogError("Objeto com a tag 'Buttons' não encontrado.");
            }
        }
    }

    public void startNewGame(){
        Destroy(table);
        table = InstantiateTable();
        linkSetUp(table);
    }

}

