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
        GameObject table1 = InstantiateTable1();
        
        linkSetUp(table1);
    }

    public GameObject InstantiateTable1(){
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
        buttons.SetActive(false);
        
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
                buttons.SetActive(true);
                camM.GetComponent<GameObject>().SetActive(true);
                showinfo.GetComponent<GameObject>().SetActive(true);
            }else {
                Debug.LogError("Objeto com a tag 'Buttons' não encontrado.");
            }
        }
    }

}

