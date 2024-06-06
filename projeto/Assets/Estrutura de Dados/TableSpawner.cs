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

        Transform canva = table.transform.Find("Canvas");
        Transform screen = canva.transform.Find("InitialScreen");
        Transform path = screen.transform.Find("path");
        TMP_InputField p = path.GetComponent<TMP_InputField>();
        Debug.Log(path);
        Debug.Log(p==null);

        Transform show = canva.transform.Find("show");
        Transform camM = show.transform.Find("CameraSwitch");
        Transform topview = table.transform.Find("TopViewCamera");
        GameObject topviewcam = topview.GetComponent<GameObject>();
        Debug.Log(camM==null);
        Button switchCam = camM.GetComponent<Button>();
        UiManager uim = topview.GetComponent<UiManager>();
        uim.mainCamera = GameObject.Find("Main Camera");
        uim.miniMap = GameObject.Find("MiniMap");
        switchCam.onClick.AddListener(changescene);
        void changescene(){
            uim.changeCamera();
        }
        Debug.Log(uim==null);

        GameObject showbutton = show.gameObject;
        showbutton.SetActive(false);

        p.onEndEdit.AddListener(OnEnd);
        void OnEnd(string input){
                Debug.Log("AHHHHHH");
                showbutton.SetActive(true);
            }

        Transform clearTable = canva.transform.Find("ClearTable");
        ifm.clearB = clearTable.GetComponent<Button>();
    }

    public void startNewGame(){
        Destroy(table);
        table = InstantiateTable();
        linkSetUp(table);
    }

}

