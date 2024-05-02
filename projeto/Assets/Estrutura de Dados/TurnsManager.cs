using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnsManager : MonoBehaviour
{
    public List<Unit[]> turnsList;
    public Board board;
    public Game game;
    private bool isPaused = false;
    private bool paused = false;
    public Coroutine unitCoroutine;
    public GameState state;

    public void StartGame(Game games)
    {
        this.game = games;
        this.turnsList = game.turns;
        this.board = game.board;
        state.currentTurn = 0;
        state.currentUnit = 0;
        MakeTurn(turnsList[0]); // faz o primeiro turno
        Debug.Log("começou");

    }

    private void MakeTurn(Unit[] units)
    {
        unitCoroutine = StartCoroutine(ExecuteTurn(units));
    }

    private IEnumerator ExecuteTurn(Unit[] units)
    {
        List<int []> coordenadasAtacadas = new List<int[]>();
        for (int i = state.currentUnit; i < units.Length; i++){
            Unit unit = units[state.currentUnit];
            Debug.Log(unit);
            // Execute as ações para cada unidade do turno
            // Por exemplo:
            switch (unit.action.ToString())
            {
                case "spawn":
                    spawn(board, unit);
                    break;

                case "move_to":
                    moveTo(board, unit);
                    break;

                case "hold":
                    unit.hold();
                    break;

                case "attack":
                    coordenadasAtacadas.Add(unit.attack());
                    break;
            }
            state.currentUnit++;
            yield return new WaitForSecondsRealtime(5f); 
        }
          yield return new WaitForSecondsRealtime(5f); 

        handleDeaths(coordenadasAtacadas);
        state.currentUnit = 0;
        NextTurn();
    }

    public void handleDeaths(List<int []> coordenadasAtacadas){

        List<Piece> piecesToRemove = new List<Piece>();

        foreach(int[] coord in coordenadasAtacadas){
            int x = coord[0];
            int y = coord[1];

            foreach (Piece p in game.pieces){
                if(p.x == x && p.y == y){
                    piecesToRemove.Add(p);
                }
            }
        }

        foreach (Piece p in piecesToRemove){
            pieceDeath(p);
        }
    }


    public void pieceDeath(Piece p){
    
        UnityEngine.Debug.Log("Peça "+ p.id + " vai morrer!");
        game.pieces.Remove(p);
        GameObject peca = p.getGameO();
        Destroy(peca);
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

        Piece p = unit.piece;
        UnityEngine.Debug.Log("Peça "+ p.id + " inicializada em x = "+p.x+ " e y =" +p.y);
        game.addPiece(p);

        

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

        Piece p = unit.piece;
        UnityEngine.Debug.Log("Peça "+ p.id + " tem de se mover para x = "+unit.posFocoX+ " e y =" +unit.posFocoY);
        game.UpdatePosPiece(p,unit.posFocoX,unit.posFocoY);
        
        
    }
   
    //funcao a ser chamada no botao para proxima jogada
    public void NextTurn(){
        if (state.currentTurn < turnsList.Count - 1){
            state.currentTurn++; 
            MakeTurn(turnsList[state.currentTurn]);
            Debug.Log(turnsList[state.currentTurn]);
        }else{
            UnityEngine.Debug.Log("O jogo acabou!");
        }
    }

    // funcao a ser chamada no botao para a jogada anterior
    public void PreviousTurn()
    {
        if (state.currentTurn > 0)
        {
            state.currentTurn--;
            MakeTurn(turnsList[state.currentTurn]);
        }
    }

    // funcao que controla a paragem do jogo
    public void Pause()
    {
        paused = true;
        if (unitCoroutine != null)
        {
            StopCoroutine(unitCoroutine);
            Debug.Log("devia parar");
            unitCoroutine = null; // Atualiza a variável turnCoroutine para null
            Time.timeScale = 0f;
        }
        Debug.Log("Pausado");
    }

    public void Play()
    {
        paused = false;
        Debug.Log(unitCoroutine);

        if (unitCoroutine == null) 
        {
            Time.timeScale = 1f;
            MakeTurn(turnsList[state.currentTurn]); 
        }
    }


    //ambas as de cima tem de ter mais logica por detras com as animaçoes, tipo o pause temos que garantir que faz com que as cenas parem e 
    //guardem o estado delas (acho eu) e o play tem de ter em consideraçao o estado atual e completar o turn se necessario
    
}

//guardar estado do jogo
public struct GameState
{
    public int currentTurn;
    public int currentUnit;
}
