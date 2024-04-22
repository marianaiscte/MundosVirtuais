using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnsManager : MonoBehaviour
{
    public List<Unit[]> turnsList;
    
    public int currentTurn = 0; 

    public Boolean paused = false;

    public void StartGame( List<Unit[]> turnsList)
    {
        this.turnsList = turnsList;
        MakeTurn(turnsList[0]); // faz o primeiro turno
    }

    private void MakeTurn(Unit[] units)
    {
        //logica por implementar de como e que as coisas mexem etc
    }

    //funcao a ser chamada no botao para proxima jogada
    public void NextTurn(){
        if (currentTurn < turnsList.Count - 1)
        {
            currentTurn++;
            MakeTurn(turnsList[currentTurn]);
        }
    }

    // funcao a ser chamada no botao para a jogada anterior
    public void PreviousTurn()
    {
        if (currentTurn > 0)
        {
            currentTurn--;
             MakeTurn(turnsList[currentTurn]);
        }
    }

    // funcao que controla a paragem do jogo
    public void Pause()
    {
        paused = true;
    }

    // funcao que mete play  
    public void play()
    {
        paused = false;
    }

    //ambas as de cima tem de ter mais logica por detras com as animaçoes, tipo o pause temos que garantir que faz com que as cenas parem e 
    //guardem o estado delas (acho eu) e o play tem de ter em consideraçao o estado atual e completar o turn se necessario
    
}
