using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TableSpawner : MonoBehaviour
{
    public GameObject gameTablePrefab; // O prefab da mesa de jogo
    public Transform tablePosition1; // Referência ao primeiro GameObject vazio
    public Transform tablePosition2; // Referência ao segundo GameObject vazio
    public Button pauseB;
    public Button playB;
    public Button NextTurn;
    public Button PreviousTurn;
    public Button XMLLoader;
    public GameObject mainCamera;
    public GameObject topViewCamera;
    public Light luz;
    public Slider lightSlider;

    void Start()
    {
        // Instancia a primeira mesa
        GameObject table1 = Instantiate(gameTablePrefab, tablePosition1.position, tablePosition1.rotation);
        table1.transform.SetParent(tablePosition1);
        
        // Instancia a segunda mesa
        GameObject table2 = Instantiate(gameTablePrefab, tablePosition2.position, tablePosition2.rotation);
        table2.transform.SetParent(tablePosition2);

        Transform Tabuleiro = table1.transform.Find("Tabuleiro");
        InputFieldManager ifm = Tabuleiro.GetComponent<InputFieldManager>();
        Debug.Log(ifm==null);
        ifm.pauseB = pauseB;
        ifm.playB = playB;
        ifm.NextTurn = NextTurn;
        ifm.PreviousTurn = PreviousTurn;

        Transform deskS = table1.transform.Find("(Prb)Desk1 (1)");
        Transform desk1 = deskS.transform.Find("desk1");
        DeskScript desk = desk1.GetComponent<DeskScript>();
        Debug.Log(desk==null);
        desk.XMLLoader = XMLLoader;
        //desk.Start();

        Transform camM = table1.transform.Find("CameraManager");
        UiManager uim = camM.GetComponent<UiManager>();
        Debug.Log(uim==null);
        uim.mainCamera = mainCamera;
        //este temos de mudar para dar para as duas mesas
        uim.topViewCamera = topViewCamera;
       
        Transform deskLight = table1.transform.Find("(Prb)DeskLight");
        Transform dl = deskLight.transform.Find("deskLight");
        Transform lamp = dl.transform.Find("deskLight_base");
        LampLight deskLamp = lamp.GetComponent<LampLight>();
        deskLamp.luz = luz;
        deskLamp.lightSlider = lightSlider;
    }
}

