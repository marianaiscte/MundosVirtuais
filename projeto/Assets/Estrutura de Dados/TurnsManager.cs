using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TurnsManager : MonoBehaviour
{
    public List<Unit[]> turnsList;
    
    public int currentTurn = 0; 

    public Boolean paused = false;

    public Board board;

    

    public void StartGame( Game game)
    {
        this.turnsList = game.turns;
        this.board = game.board;
        MakeTurn(turnsList[0]); // faz o primeiro turno
    }

    private void MakeTurn(Unit[] units){

        for(int i = 0; i < units.Length; i++){

            switch (units[i].action.ToString()) {
                    case "spawn":
                    spawn(board,units[i]);
                    break;

                    case "move_to":
                    moveTo(board,units[i]);
                    break;

                    case "hold":
                    units[i].hold();
                    break;

                    case "attack":
                    units[i].attack();
                    break;

            }
        }
        NextTurn();
    }

    public void spawn(Board board, Unit unit){

        int x = unit.posFocoX - 1;
        int y = unit.posFocoY - 1;

        //Debug.Log(x + " " + y);

        Tile tile = board.BoardDisplay[x, y];

        GameObject cyl = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        Renderer renderer = cyl.GetComponent<Renderer>();

        GameObject gameTile = tile.getGameO();
        
        cyl.transform.position = gameTile.transform.position;
        cyl.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        unit.piece.associateObj(cyl);
        //Debug.Log(unit.piece.getGameO());

        switch(unit.piece.type.ToString()){
            
            case "Soldier":
            renderer.material.color = Color.black;
            break;

            case"Archer":
            renderer.material.color = Color.green;
            break;

            case "Mage":
            renderer.material.color = Color.cyan;
            break;

            case"Catapult":
            renderer.material.color = Color.gray;
            break;

        }

    }

    public void moveTo(Board board, Unit unit){

        int x = unit.posFocoX - 1;
        int y = unit.posFocoY - 1;

        Tile tile = board.BoardDisplay[x, y];

        GameObject mover = unit.piece.getGameO();
        mover.AddComponent<ObjectMover>();

        GameObject gameTile = tile.getGameO();
        //Debug.Log(gameTile);

        Vector3 targetPos = gameTile.transform.position;

        ObjectMover objm = mover.GetComponent<ObjectMover>();
        objm.StartMoving(mover, targetPos);
        //mover.transform.position = Vector3.MoveTowards(mover.transform.position, targetPos, 4 * Time.deltaTime);
        //este nao funciona, acho que e porque nao temos um update() so que nao sei o que fazer, isso nao implica mudar o codigo todo?

        //mover.transform.position = targetPos;
        
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
