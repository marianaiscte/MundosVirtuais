using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class TurnsManager : MonoBehaviour
{
    public List<Unit[]> turnsList;
    public Board board;
    public Game game;
    public bool paused = false;
    public bool isCalledByScene = false;
    public bool goToPrevious = false;
    public Coroutine unitCoroutine;
    public GameState state;
    public List<Dictionary<(int, int), List<Piece>>> oldTurnsPositions = new List<Dictionary<(int, int), List<Piece>>>();
    private InputFieldManager inputFieldManager;

    string player1name;
    string player2name;

    GameObject boardGameObject;

    public void StartGame(Game games, GameObject boardGameObjectgiven)
    {
        this.game = games;
        this.turnsList = game.turns;
        this.board = game.board;
        Player [] names = game.roles;
        player1name = names[0].name;
        player1name = names[1].name;
        state.currentTurn = 0;
        state.currentUnit = 0;
        inputFieldManager = FindObjectOfType<InputFieldManager>();
        boardGameObject = boardGameObjectgiven;
        UpdateUI();
        MakeTurn(turnsList[0]); // faz o primeiro turno
        //Debug.Log("começou");
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
            //Debug.Log(unit);
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
                    if(unit.piece.type == PieceType.Soldier){
                        Tile tile = board.BoardDisplay[unit.posFocoX-1, unit.posFocoY-1];
                        if(tile.type != TileType.Sea){
                        TileType type = tile.type;
                            soldierAttack(unit,type);
                        }
                    }
                    if(unit.piece.type != PieceType.Catapult){
                        Animator animate = unit.piece.getGameO().GetComponent<Animator>();
                        animate.SetBool("attack",true);
                        coordenadasAtacadas.Add(unit.attack());
                        yield return new WaitForSeconds(1f);
                        animate.SetBool("attack", false);
                    }
                    GameObject audio = null;
                        GameObject projectileType = null;
                        if (unit.piece.type == PieceType.Archer){
                            //Debug.Log("PROJETEIS!");
                            projectileType = Resources.Load<GameObject>("Projectiles/arrow");
                            audio = GameObject.Find("arrowAudio");
                        }
                         if (unit.piece.type == PieceType.Mage){
                            projectileType = Resources.Load<GameObject>("Projectiles/fireball");
                            audio = GameObject.Find("fireballAudio");
                        }
                        if (unit.piece.type == PieceType.Catapult){
                            projectileType = Resources.Load<GameObject>("Projectiles/rock");
                            audio = GameObject.Find("deathAudio");
                        }

                        if(projectileType != null){
                            Tile tile = board.BoardDisplay[unit.posFocoX-1, unit.posFocoY-1];
                            GameObject gameTile = tile.getGameO();
                            GameObject projectile = Instantiate(projectileType, unit.piece.getGameO().transform.position, Quaternion.identity);
                            projectile.AddComponent<Projectile>();
                            Projectile projectileScript = projectile.AddComponent<Projectile>();
                            projectileScript.targetPosition = gameTile.transform.position;
                            audio.GetComponent<AudioSource>().Play();
                        }

                    break;
            }
            UpdateUI();
            state.currentUnit++;
            if(goToPrevious){
                PreviousTurn();
            }
            if(!isCalledByScene){
                yield return new WaitForSecondsRealtime(3f); 
            } 
            
        }
          //yield return new WaitForSecondsRealtime(3f); 

        handleDeaths(coordenadasAtacadas);
        state.currentUnit = 0;
        NextTurn();
    }

    public void soldierAttack(Unit unit, TileType tileType){
        //Debug.Log("vou procurar");
        List<Piece> pieces = game.getPiecesInTile(unit.posFocoX, unit.posFocoY);
        foreach(Piece piece in pieces){
            if(piece.type == PieceType.Soldier){
                //Debug.Log("DA LHEEEE");
                inputFieldManager.changeScene(tileType.ToString());
            }
        }
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
        //UnityEngine.Debug.Log("Peça "+ p.id + " vai morrer!");
        GameObject peca = p.getGameO();
        GameObject deathAudio = GameObject.Find("deathAudio");

        // Aciona a animação de "morte"
        if(p.type != PieceType.Catapult){
            Animator animate = p.getGameO().GetComponent<Animator>();
            animate.SetBool("died", true);

            // Espere pela duração da animação de "morte"
            float deathAnimationDuration = animate.GetCurrentAnimatorClipInfo(0)[0].clip.length;
            yield return new WaitForSeconds(1f);
        }
        
        float elapsedTime = 0;
        Vector3 originalScale = peca.transform.localScale; 

        while (elapsedTime < 1f)
        {
            float progress = elapsedTime / 1f;
            peca.transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, elapsedTime / 1f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        GameObject smoke = Resources.Load<GameObject>("Audio/SmokeEffect");
        GameObject smokeEffect = Instantiate(smoke);

        if(deathAudio != null){
            //Debug.Log("FDSSSS");
            ParticleSystem ps = smoke.GetComponentInChildren<ParticleSystem>();
            ps.transform.position = peca.transform.position;
            ps.Play();
            deathAudio.GetComponent<AudioSource>().Play();
            ps.Stop();
        }

        // Destrua o objeto da peça
        game.pieces.Remove(p);
        peca.SetActive(false);
        StartCoroutine(DestroyAfterSeconds(smokeEffect, 5f));
}

    public UnityEngine.Vector3[] placePieces(int x, int y, GameObject gameTile){
        int numberOfpieces = game.CountPiecesInTile(x, y);
        UnityEngine.Vector3 gameTilePos = gameTile.transform.position;
        UnityEngine.Vector3[] positions = new UnityEngine.Vector3[numberOfpieces];
        float offset = 0.1f; // Distância de offset do centro
        //Debug.Log(numberOfpieces);
        switch(numberOfpieces){
            case 1:
               // objects[0] = cyl;
                positions[0] = gameTilePos + new UnityEngine.Vector3(0, 0.025f, 0);
                break;
            case 2:
                positions[0] = gameTilePos + new UnityEngine.Vector3(-offset, 0.025f, 0); 
                //objects[1] = cyl;
                positions[1] = gameTilePos + new UnityEngine.Vector3(offset, 0.025f, 0); 
                break;
             case 3:
                positions[0] = gameTilePos + new UnityEngine.Vector3(-offset, 0.025f, 0); 
                positions[1] = gameTilePos + new UnityEngine.Vector3(offset, 0.025f, 0);
                //objects[2] = cyl;
                positions[2] = gameTilePos + new UnityEngine.Vector3(0, 0.025f, offset); 
                break;
            case 4:
                positions[0] = gameTilePos + new UnityEngine.Vector3(-offset, 0.025f, -offset); 
                positions[1] = gameTilePos + new UnityEngine.Vector3(offset, 0.025f, -offset);
                positions[2] = gameTilePos + new UnityEngine.Vector3(-offset, 0.025f, offset); 
                //objects[3] = cyl;
                positions[3] = gameTilePos + new UnityEngine.Vector3(offset, 0.025f, offset); 
                break;
        }
        return positions;
    }

    private GameObject getPrefabs(Unit unit){
        GameObject prefabToSpawn = null;

        switch(unit.piece.type.ToString())
        {
            case "Soldier":
                if(unit.rolePlaying == player1name){
                    prefabToSpawn = Resources.Load<GameObject>("Pieces/soldier1");
                }else{
                    prefabToSpawn = Resources.Load<GameObject>("Pieces/soldier2");
                }
                break;

            case "Archer":
            if(unit.rolePlaying == player1name){
                prefabToSpawn = Resources.Load<GameObject>("Pieces/archer1");
            }else{
                prefabToSpawn = Resources.Load<GameObject>("Pieces/archer2");
            }
                break;

            case "Mage":
            if(unit.rolePlaying == player1name){
                prefabToSpawn = Resources.Load<GameObject>("Pieces/mage1");
            }else{
                prefabToSpawn = Resources.Load<GameObject>("Pieces/mage1");

            }
                break;

            case "Catapult":
                prefabToSpawn = Resources.Load<GameObject>("Pieces/catapult");
                break;

            default:
                Debug.LogWarning("Prefab não encontrado para o tipo de peça: " + unit.piece.type.ToString());
                break;
        }
        return prefabToSpawn;
    }

     public void spawn(Board board, Unit unit)
    {
        int x = unit.posFocoX - 1;
        int y = unit.posFocoY - 1;

        Tile tile = board.BoardDisplay[x, y];
        GameObject gameTile = tile.getGameO();
        
        GameObject prefabToSpawn = getPrefabs(unit);

        if (prefabToSpawn != null)
        {
            Piece p = unit.piece;
            //Debug.Log("Peça "+ p.id + " inicializada em x = "+p.x+ " e y =" +p.y);
            game.addPiece(p);
            GameObject pieceObject = Instantiate(prefabToSpawn);
            pieceObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            unit.piece.associateObj(pieceObject);
            pieceObject.transform.parent = boardGameObject.transform;

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
        ObjectMover objectMover = mover.GetComponent<ObjectMover>();

        if (objectMover == null)
        {
            objectMover = mover.AddComponent<ObjectMover>();
        }
        else
        {
            objectMover = mover.GetComponent<ObjectMover>();
        }

        GameObject gameTile = tile.getGameO();
        //Debug.Log(gameTile);
        Piece p = unit.piece;
        //UnityEngine.Debug.Log("Peça "+ p.id + " tem de se mover para x = "+unit.posFocoX+ " e y =" +unit.posFocoY);
        game.UpdatePosPiece(p,unit.posFocoX,unit.posFocoY);

        UnityEngine.Vector3 targetPos = new UnityEngine.Vector3();

        GameObject charact = getPrefabs(unit);
        Debug.Log(charact);
        GameObject pieceObject = Instantiate(charact);
        pieceObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        pieceObject.transform.position = mover.transform.position;
        Debug.Log(pieceObject.transform.position);

        Renderer[] renderers = pieceObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            // Obtenha o material original do renderer
            Material originalMaterial = renderer.material;

            // Crie uma cópia do material para não alterar o material original do prefab
            Material transparentMaterial = new Material(originalMaterial)
            {
                // Defina o shader para Universal Render Pipeline/Lit
                shader = Shader.Find("Universal Render Pipeline/Lit")
            };

            // Ajuste as propriedades do material para torná-lo transparente
            transparentMaterial.SetFloat("_Surface", 1); // 1 é para Transparente
            transparentMaterial.SetFloat("_Blend", (float)UnityEngine.Rendering.BlendMode.DstAlpha);
            transparentMaterial.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
            transparentMaterial.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            transparentMaterial.SetFloat("_ZWrite", 0);
            transparentMaterial.DisableKeyword("_ALPHATEST_ON");
            transparentMaterial.EnableKeyword("_ALPHABLEND_ON");
            transparentMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            transparentMaterial.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            
            // Aplique o material transparente ao renderer
            renderer.material = transparentMaterial;

            Color oldColor = renderer.material.color;
            Color newColor = new Color(oldColor.r, oldColor.g, oldColor.b, 0.5f);
            //Debug.Log("Old Color: " + oldColor);
            //Debug.Log("New Color: " + newColor);
            renderer.material.color = newColor; // Ajusta a cor do material
        }

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
        
        //ObjectMover objm = mover.GetComponent<ObjectMover>();
        objectMover.StartMoving(mover, targetPos);
        StartCoroutine(DestroyAfterSeconds(pieceObject, 2f));
    }

    private IEnumerator DestroyAfterSeconds(GameObject obj, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(obj);
    }

    public void SaveTurn(){
        Dictionary<(int, int), List<Piece>> turnPositions = new Dictionary<(int, int), List<Piece>>();
        for (int x = 1; x <= board.Width; x++)
        {
            for (int y = 1; y <= board.Height; y++)
            {
                turnPositions.Add((x, y), game.getPiecesInTile(x, y));
            }
        }
        Debug.Log(turnPositions==null);
        oldTurnsPositions.Add(turnPositions);
        Debug.Log(oldTurnsPositions.Count);  
        game.SaveOldPositions(oldTurnsPositions);
    }
   
    //funcao a ser chamada no botao para proxima jogada
   public void NextTurn(){
        if (state.currentTurn < turnsList.Count - 1){
            isCalledByScene = false;
            //Debug.Log("Estado atual" + state.currentTurn);
            SaveTurn();
            state.currentTurn++; 
            state.currentUnit = 0;
            UpdateUI();
            MakeTurn(turnsList[state.currentTurn]);
        }else{
            UnityEngine.Debug.Log("O jogo acabou!");
            inputFieldManager.UpdateUI("", state.currentTurn + 1, "Game Over");
        }
    }

    // funcao a ser chamada no botao para a jogada anterior
    public void PreviousTurn()
    {
        Pause();
        goToPrevious = false;
        if (state.currentTurn >= 0){   
                state.currentUnit = 0;
                if(state.currentTurn == 0 || state.currentTurn == 1){
                    foreach (Piece piece in game.pieces){
                        Destroy(piece.getGameO());
                }
                
                game.pieces.Clear(); 
                state.currentTurn = 0;
                Play(); // faz o primeiro turno

            } else{

            Dictionary<(int, int), List<Piece>> turnPositions = oldTurnsPositions[state.currentTurn - 2];

            // Obter todas as peças em turnPositions
            HashSet<Piece> turnPositionPieces = new HashSet<Piece>(turnPositions.Values.SelectMany(list => list));

            // Obter as peças que não estão no turnPositions
            List<Piece> piecesToKill = game.pieces
                .Where(piece => !turnPositionPieces.Contains(piece))
                .ToList();

            foreach (Piece piece in piecesToKill){
                game.pieces.Remove(piece);
                GameObject peca = piece.getGameO();
                Destroy(peca);
            }

            // Obter todas as peças do jogo
            HashSet<Piece> gamePieces = new HashSet<Piece>(game.pieces);

            // Obter todas as peças em turnPositions que não estão em game.pieces
            List<Piece> piecesToAdd = turnPositions.Values
                .SelectMany(list => list)
                .Where(piece => !gamePieces.Contains(piece))
                .ToList();

            foreach (Piece piece in piecesToAdd){
                game.addPiece(piece);
                piece.getGameO().SetActive(true);
            }

                //chegando aqui temos: peças que não existiam apagadas, e peças que morreram mas estavam cá antes vivas again

            foreach (KeyValuePair<(int, int), List<Piece>> kvp in turnPositions)
            {
                (int x, int y) = kvp.Key;
                Tile tile = board.BoardDisplay[x-1, y-1];
                GameObject gameTile = tile.getGameO();
                foreach(Piece piece in kvp.Value){
                    game.UpdatePosPiece(piece, x, y);
                }

                Debug.Log("vou tratar de tudo");
                UnityEngine.Vector3[] positions = placePieces(x, y, gameTile);
                GameObject[] objects = game.getObjectsInTile(x, y);
                int i = 0;
                foreach(GameObject obj in objects){
                    obj.transform.position = positions[i];
                    i++;
                }
            }
                state.currentTurn--;
                UpdateUI();
                Play();
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
            //Debug.Log("Parou");
            unitCoroutine = null; // Atualiza a variável turnCoroutine para null
            Time.timeScale = 0f;
            inputFieldManager.UpdateUI("", state.currentTurn + 1, "Paused");
        }
    }

    public void Play()
    {
        paused = false;
        if (unitCoroutine == null) 
        {
            Time.timeScale = 1f;
            //Debug.Log("Voltou a andar");
            MakeTurn(turnsList[state.currentTurn]); 
            inputFieldManager.UpdateUI("", state.currentTurn + 1, "Game in Progress");
        }
    }

    //ambas as de cima tem de ter mais logica por detras com as animaçoes, tipo o pause temos que garantir que faz com que as cenas parem e 
    //guardem o estado delas (acho eu) e o play tem de ter em consideraçao o estado atual e completar o turn se necessario

    private void UpdateUI()
    {
        Debug.Log(state.currentTurn + state.currentUnit);
        string playerName = turnsList[state.currentTurn][state.currentUnit].piece.owner.name;
        int turnCount = state.currentTurn + 1;
        string gameStatus = "Game in Progress";
        inputFieldManager.UpdateUI(playerName, turnCount, gameStatus);
    }


//guardar estado do jogo
public struct GameState
{
    public int currentTurn;
    public int currentUnit;
}
}
