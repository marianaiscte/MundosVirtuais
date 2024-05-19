using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class TurnsManager : MonoBehaviour
{
    public List<Unit[]> turnsList;
    public Board board;
    public Game game;
    private bool isPaused = false;
    public bool paused = false;
    public bool isCalledByScene = false;
    public Coroutine unitCoroutine;
    public GameState state;

    public List<Dictionary<(int, int), List<Piece>>> oldTurnsPositions = new List<Dictionary<(int, int), List<Piece>>>();

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
                    Animator animate = unit.piece.getGameO().GetComponent<Animator>();
                    animate.SetBool("attack",true);
                    coordenadasAtacadas.Add(unit.attack());
                    yield return new WaitForSeconds(1f);
                    animate.SetBool("attack", false);
                        GameObject projectileType = null;
                        if (unit.piece.type == PieceType.Archer){
                            //Debug.Log("PROJETEIS!");
                            projectileType = Resources.Load<GameObject>("Projectiles/arrow");
                        }
                         if (unit.piece.type == PieceType.Mage){
                            projectileType = Resources.Load<GameObject>("Projectiles/fireball");
                        }
                        if (unit.piece.type == PieceType.Catapult){
                            projectileType = Resources.Load<GameObject>("Projectiles/rock");
                        }

                        if(projectileType != null){
                            Tile tile = board.BoardDisplay[unit.posFocoX-1, unit.posFocoY-1];
                            GameObject gameTile = tile.getGameO();
                            GameObject projectile = Instantiate(projectileType, unit.piece.getGameO().transform.position, Quaternion.identity);
                            projectile.AddComponent<Projectile>();
                            Projectile projectileScript = projectile.AddComponent<Projectile>();
                            projectileScript.targetPosition = gameTile.transform.position;
                        }
                    break;
            }
            state.currentUnit++;
            yield return new WaitForSecondsRealtime(3f); 
        }
          //yield return new WaitForSecondsRealtime(3f); 

        handleDeaths(coordenadasAtacadas);
        state.currentUnit = 0;
        /*Dictionary<(int,int), Piece> turnPositions = new Dictionary<(int,int), Piece>();
        foreach (Piece p in game.pieces){
            turnPositions.Add((p.x, p.y), p);
        }
        oldTurnsPositions.Add(turnPositions);*/
        NextTurn(false);
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
            StartCoroutine(pieceDeath(p));
        }
    }

    public IEnumerator pieceDeath(Piece p){
        UnityEngine.Debug.Log("Peça "+ p.id + " vai morrer!");

        // Aciona a animação de "morte"
        Animator animate = p.getGameO().GetComponent<Animator>();
        animate.SetBool("died", true);

        // Espere pela duração da animação de "morte"
        float deathAnimationDuration = animate.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        yield return new WaitForSeconds(1f);

        // Destrua o objeto da peça
        game.pieces.Remove(p);
        GameObject peca = p.getGameO();
        Destroy(peca);
}

    public UnityEngine.Vector3[] placePieces(int x, int y, GameObject gameTile){
        int numberOfpieces = game.CountPiecesInTile(x, y);
        UnityEngine.Vector3 gameTilePos = gameTile.transform.position;
        UnityEngine.Vector3[] positions = new UnityEngine.Vector3[numberOfpieces];
        float offset = 0.2f; // Distância de offset do centro
        Debug.Log(numberOfpieces);
        switch(numberOfpieces){
            case 1:
               // objects[0] = cyl;
                positions[0] = gameTilePos;
                break;
            case 2:
                positions[0] = gameTilePos + new UnityEngine.Vector3(-offset, 0, 0); 
                //objects[1] = cyl;
                positions[1] = gameTilePos + new UnityEngine.Vector3(offset, 0, 0); 
                break;
             case 3:
                positions[0] = gameTilePos + new UnityEngine.Vector3(-offset, 0, 0); 
                positions[1] = gameTilePos + new UnityEngine.Vector3(offset, 0, 0);
                //objects[2] = cyl;
                positions[2] = gameTilePos + new UnityEngine.Vector3(0, 0, offset); 
                break;
            case 4:
                positions[0] = gameTilePos + new UnityEngine.Vector3(-offset, 0, -offset); 
                positions[1] = gameTilePos + new UnityEngine.Vector3(offset, 0, -offset);
                positions[2] = gameTilePos + new UnityEngine.Vector3(-offset, 0, offset); 
                //objects[3] = cyl;
                positions[3] = gameTilePos + new UnityEngine.Vector3(offset, 0, offset); 
                break;
        }
        return positions;
    }


     public void spawn(Board board, Unit unit)
    {
        int x = unit.posFocoX - 1;
        int y = unit.posFocoY - 1;

        Tile tile = board.BoardDisplay[x, y];
        GameObject gameTile = tile.getGameO();
        
        GameObject prefabToSpawn = null;

        switch(unit.piece.type.ToString())
        {
            case "Soldier":
                prefabToSpawn = Resources.Load<GameObject>("Pieces/soldier");
                break;
            case "Archer":
                prefabToSpawn = Resources.Load<GameObject>("Pieces/archer");
                break;
            case "Mage":
                prefabToSpawn = Resources.Load<GameObject>("Pieces/mage");
                break;
            case "Catapult":
                prefabToSpawn = Resources.Load<GameObject>("Pieces/catapult");
                break;
            default:
                Debug.LogWarning("Prefab não encontrado para o tipo de peça: " + unit.piece.type.ToString());
                break;
        }

        if (prefabToSpawn != null)
        {
            Piece p = unit.piece;
            Debug.Log("Peça "+ p.id + " inicializada em x = "+p.x+ " e y =" +p.y);
            game.addPiece(p);
            GameObject pieceObject = Instantiate(prefabToSpawn);
            pieceObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

            unit.piece.associateObj(pieceObject);

            UnityEngine.Vector3[] positions = placePieces(unit.posFocoX, unit.posFocoY, gameTile);
            GameObject[] objects = game.getObjectsInTile(unit.posFocoX, unit.posFocoY);
            int i = 0;
            foreach(GameObject obj in objects){
                obj.transform.position = positions[i];
                i++;
            }
        }
        else
        {
            Debug.LogError("Falha ao carregar prefab para o tipo de peça: " + unit.piece.type.ToString());
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
        Piece p = unit.piece;
        UnityEngine.Debug.Log("Peça "+ p.id + " tem de se mover para x = "+unit.posFocoX+ " e y =" +unit.posFocoY);
        game.UpdatePosPiece(p,unit.posFocoX,unit.posFocoY);

        UnityEngine.Vector3 targetPos = new UnityEngine.Vector3();

        UnityEngine.Vector3[] positions = placePieces(unit.posFocoX,unit.posFocoY, gameTile);
        GameObject[] objects = game.getObjectsInTile(unit.posFocoX, unit.posFocoY);
        int i = 0;
        foreach(GameObject obj in objects){
            if(mover.Equals(obj)){
                targetPos = positions[i];
            }
            else{  
                obj.transform.position = positions[i];
                i++;
            }
        }
        
        ObjectMover objm = mover.GetComponent<ObjectMover>();
        objm.StartMoving(mover, targetPos);
        
    }
   
    //funcao a ser chamada no botao para proxima jogada
   public void NextTurn(bool buttonCall){
        if (state.currentTurn < turnsList.Count - 1){
            isCalledByScene = buttonCall;
            Debug.Log("Estado atual" + state.currentTurn);
            Dictionary<(int, int), List<Piece>> turnPositions = new Dictionary<(int, int), List<Piece>>();
            for (int x = 1; x <= board.Width; x++)
            {
                for (int y = 1; y <= board.Height; y++)
                {
                    foreach(Piece p in game.getPiecesInTile(x, y)){
                        //Debug.Log((x, y) + ", " +p.id);
                    }
                    turnPositions.Add((x, y), game.getPiecesInTile(x, y));
                }
            }
            oldTurnsPositions.Add(turnPositions);
            Debug.Log(oldTurnsPositions.Count);  
            game.SaveOldPositions(oldTurnsPositions);
            state.currentTurn++; 
            state.currentUnit = 0;
            MakeTurn(turnsList[state.currentTurn]);
        }else{
            UnityEngine.Debug.Log("O jogo acabou!");
        }
    }

    // funcao a ser chamada no botao para a jogada anterior
    public void PreviousTurn()
    {
        Debug.Log(state.currentTurn);
        if (state.currentTurn >= 0){   
                state.currentUnit = 0;
                if(state.currentTurn == 0 || state.currentTurn == 1){
                    foreach (Piece piece in game.pieces){
                    if (piece.getGameO() != null){
                        Destroy(piece.getGameO());
                    }
                }
                
                game.pieces.Clear();
                //faço o que se faz no play para os movimentos continuarem
                Time.timeScale = 1f;
                state.currentTurn = 0;
                MakeTurn(turnsList[0]); // faz o primeiro turno
            } else{


           /* Dictionary<(int, int), Piece> turnPositions = oldTurnsPositions[state.currentTurn];

            foreach(KeyValuePair<(int, int), Piece> kvp in turnPositions)
            {
                Piece piece = kvp.Value;
                (int x, int y) = kvp.Key;

                pieceDeath(piece);
                Tile tile = board.BoardDisplay[x, y];

                GameObject cyl = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                Renderer renderer = cyl.GetComponent<Renderer>();

                GameObject gameTile = tile.getGameO();

                game.addPiece(piece);

                //cyl.transform.position = gameTile.transform.position; // Posiciona a peça no centro do tile
                //cyl.transform.localScale = new UnityEngine.Vector3(0.1f, 0.1f, 0.1f); // Define a escala da peça

                switch(piece.type.ToString()){
                    
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

                Debug.Log("vou tratar de tudo");
                UnityEngine.Vector3[] positions = placePieces(x, y, gameTile);
                GameObject[] objects = game.getObjectsInTile(x, y);
                int i = 0;
                foreach(GameObject obj in objects){
                    obj.transform.position = positions[i];
                    i++;
                }
                resize(objects);
            }*/                
        MakeTurn(turnsList[state.currentTurn]);
            }         
        }
    }

    

    // funcao que controla a paragem do jogo
    public void Pause()
    {
        paused = true;
        if (unitCoroutine != null)
        {
            StopCoroutine(unitCoroutine);
            Debug.Log("Parou");
            unitCoroutine = null; // Atualiza a variável turnCoroutine para null
            Time.timeScale = 0f;
        }
    }

    public void Play()
    {
        paused = false;
        if (unitCoroutine == null) 
        {
            Time.timeScale = 1f;
            Debug.Log("Voltou a andar");
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
