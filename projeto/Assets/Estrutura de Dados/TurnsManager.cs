using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Xml.Schema;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

// Script que lida com a lógica do jogo
public class TurnsManager : MonoBehaviour
{
    public List<Unit[]> turnsList; //lista dos turnos
    public Board board;
    public Game game; //jogo a ser jogado
    public bool paused = false; //se jogo está em pausa ou a decorrer
    public bool isCalledByScene = false; //se o utilizador utilizou o botão de nextTurn
    public bool goToPrevious = false; //se o utilizador usou o botão de PreviousTurn
    public Coroutine unitCoroutine; //execução de units
    public GameState state; //estado atual do jogo, ou seja, turno e unit
     //lista dos tiles e das peças lá no final de cada turno
    public List<Dictionary<(int, int), List<Piece>>> oldTurnsPositions = new List<Dictionary<(int, int), List<Piece>>>();
    private InputFieldManager inputFieldManager;

    string player1name;
    string player2name;

    GameObject boardGameObject;

//É chamada pelo XmlReader, e começa o jogo no primeiro turno
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
    }

//Sempre que se muda de turno esta função é chamada, para iniciar o processamento das units
    private void MakeTurn(Unit[] units)
    {
        unitCoroutine = StartCoroutine(ExecuteTurn(units));
    }

//Iteração no turno, com utilização de corrotinas
    private IEnumerator ExecuteTurn(Unit[] units)
    {
        //tupla para guardar os espadachins que atacam numa casa (portanto não devem morrer nesse confronto)
        List<(int, int, int)> fights = new List<(int, int, int)>();
        //lista que guarda as coordenadas atacadas em cada unit para eliminir as 
        //peças que lá se encontram no fim do turno (menos os soldados que apenas atacaram e não morreram, daí a lista acima)
        List<int []> coordenadasAtacadas = new List<int[]>();
        //percorre units
        for (int i = state.currentUnit; i < units.Length; i++){
            Unit unit = units[state.currentUnit];
            //trata cada uma das ações possíveis
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
                    //caso seja um espadachim a atacar
                    if(unit.piece.type == PieceType.Soldier){
                        Tile tile = board.BoardDisplay[unit.posFocoX-1, unit.posFocoY-1];
                        if(tile.type != TileType.Sea){
                        TileType type = tile.type;
                        //vai para uma função que verifica se é uma luta com outro espadachim, 
                        //e se for inicia a visualização animada em 3D
                            bool soldierFight = soldierAttack(unit,type);
                            //adiciona o soldado que não morre à lista
                            if(soldierFight){
                                fights.Add((unit.posFocoX, unit.posFocoY, unit.piece.id));
                            }
                        }
                    }
                    //animações de ataque, excepto para a catapulta
                    if(unit.piece.type != PieceType.Catapult){
                        Animator animate = unit.piece.getGameO().GetComponent<Animator>();
                        animate.SetBool("attack",true);
                        yield return new WaitForSeconds(1f);
                        animate.SetBool("attack", false);
                    }
                        GameObject audio = null;
                        GameObject projectileType = null;
                        //condições para definir o tipo de audio associado ao ataque, e o tipo de projetil
                        if (unit.piece.type == PieceType.Archer){
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
                        //lida com a animação do projetil
                        if(projectileType != null){
                            //vai buscar a casa do tabuleiro alvo, e cria movimento do projetil até essa casa
                            Tile tile = board.BoardDisplay[unit.posFocoX-1, unit.posFocoY-1];
                            GameObject gameTile = tile.getGameO();
                            GameObject projectile = Instantiate(projectileType, unit.piece.getGameO().transform.position, Quaternion.identity);
                            projectile.AddComponent<Projectile>();
                            Projectile projectileScript = projectile.AddComponent<Projectile>();
                            projectileScript.targetPosition = gameTile.transform.position;
                            //aciona o efeito sonoro
                            audio.GetComponent<AudioSource>().Play();
                        }

                    break;
            }
            //atualiza painel de informações sobre o jogo
            //se o botão para recuar um turno tiver sido chamado passa para outra função
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

        handleDeaths(coordenadasAtacadas, fights);
        state.currentUnit = 0;
        NextTurn();
    }

//função que verifica se é uma luta de espadachins e inicia visualização 3D
    public bool soldierAttack(Unit unit, TileType tileType){
        bool soldierFight = false;
        List<Piece> pieces = game.getPiecesInTile(unit.posFocoX, unit.posFocoY);
        foreach(Piece piece in pieces){
            if(piece.type == PieceType.Soldier){
                soldierFight = true;
                inputFieldManager.changeScene(tileType.ToString());
            }
        }
        return true;
    }

// função que trata das mortes em cada turno
    public void handleDeaths(List<int []> coordenadasAtacadas, List<(int, int, int)> fights){
        List<Piece> piecesToRemove = new List<Piece>();

        foreach(int[] coord in coordenadasAtacadas){
            int x = coord[0];
            int y = coord[1];
//percorre as casas guardadas e remove as peças dessa casa, a menos que sejam um espadachim a atacar nessa casa
            foreach (Piece p in game.pieces){
                if(p.x == x && p.y == y){                    
                    if(fights.Exists(fight => fight.Item1 == x && fight.Item2 == y)){
                        if(!fights.Exists(fight => fight.Item3 == p.id)){
                            piecesToRemove.Add(p);  
                        } 
                    } else piecesToRemove.Add(p);
                }
            }
        }

        foreach (Piece p in piecesToRemove){
            StartCoroutine(pieceDeath(p));
        }
    }

//função de corrotina para tratar da animação de morrer
    public IEnumerator pieceDeath(Piece p){
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

// Peça encolhe até desaparecer
        while (elapsedTime < 1f)
        {
            float progress = elapsedTime / 1f;
            peca.transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, elapsedTime / 1f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
// O desaparecimento da peça está associado a um som, e uma nuvem de fumo
        GameObject smoke = Resources.Load<GameObject>("Audio/SmokeEffect");
        GameObject smokeEffect = Instantiate(smoke);

        if(deathAudio != null){
            ParticleSystem ps = smoke.GetComponentInChildren<ParticleSystem>();
            ps.transform.position = peca.transform.position;
            ps.Play();
            deathAudio.GetComponent<AudioSource>().Play();
            ps.Stop();
        }

        // Destroi o objeto da peça
        game.pieces.Remove(p);
        peca.SetActive(false);
        StartCoroutine(DestroyAfterSeconds(smokeEffect, 5f));
}

//Função que serve para colocar as peças numa casa consoante o numero de peças lá colocadas
    public UnityEngine.Vector3[] placePieces(int x, int y, GameObject gameTile){
        int numberOfpieces = game.CountPiecesInTile(x, y);
        Debug.Log(numberOfpieces);
        UnityEngine.Vector3 gameTilePos = gameTile.transform.position;
        UnityEngine.Vector3[] positions = new UnityEngine.Vector3[numberOfpieces];
        float offset = 0.1f; // Distância de offset do centro
        switch(numberOfpieces){
            case 1:
            //se tiver apenas uma, ela é colocada no meio da casa ("tile")
                positions[0] = gameTilePos + new UnityEngine.Vector3(0, 0.025f, 0);
                break;
            case 2:
            // se forem duas, são colocadas uma de cada lado do centro
                positions[0] = gameTilePos + new UnityEngine.Vector3(-offset, 0.025f, 0); 
                positions[1] = gameTilePos + new UnityEngine.Vector3(offset, 0.025f, 0); 
                break;
//se forem três ou quatro, são colocadas à volta do centro seguindo a mesma lógica
             case 3:
                positions[0] = gameTilePos + new UnityEngine.Vector3(-offset, 0.025f, 0); 
                positions[1] = gameTilePos + new UnityEngine.Vector3(offset, 0.025f, 0);
                positions[2] = gameTilePos + new UnityEngine.Vector3(0, 0.025f, offset); 
                break;
            case 4:
                positions[0] = gameTilePos + new UnityEngine.Vector3(-offset, 0.025f, -offset); 
                positions[1] = gameTilePos + new UnityEngine.Vector3(offset, 0.025f, -offset);
                positions[2] = gameTilePos + new UnityEngine.Vector3(-offset, 0.025f, offset); 
                positions[3] = gameTilePos + new UnityEngine.Vector3(offset, 0.025f, offset); 
                break;
        }
        return positions;
    }

//função para obter os prefabs de cada tipo de peça
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

//função que é chamada quando o tipo de ação da unit é spawn
     public void spawn(Board board, Unit unit)
    {
    //Obtém-se as posições no board, o prefab da peça e o objeto da casa onde se vai colocar a peça
        int x = unit.posFocoX - 1;
        int y = unit.posFocoY - 1;

        Tile tile = board.BoardDisplay[x, y];
        GameObject gameTile = tile.getGameO();
        
        GameObject prefabToSpawn = getPrefabs(unit);

//se o prefab não for null, cria-se a peça, com o id únic, adiciona-se a peça ao jogo e 
//coloca-se na casa consoante o numero de peças lá existentes
        if (prefabToSpawn != null)
        {
            Piece p = unit.piece;
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

//função que é chamada quando o tipo de ação da unit é move_to
    public void moveTo(Board board, Unit unit){

        // obtém as coordenadas de destino da unit (subtrai 1 porque os arrays são baseados em zero)
        int x = unit.posFocoX - 1;
        int y = unit.posFocoY - 1;

        // tile destino do tabuleiro 
        Tile tile = board.BoardDisplay[x, y];

        GameObject mover = unit.piece.getGameO();

        // obtém o componente ObjectMover do GameObject ou adiciona um novo se não existir
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
        // Atualiza posição da peça
        Piece p = unit.piece;
        game.UpdatePosPiece(p,unit.posFocoX,unit.posFocoY);

        UnityEngine.Vector3 targetPos = new UnityEngine.Vector3();
       
        //fantasma da peça ao mover-se:
        // obtém o prefab associado à unit e instancia um objeto novo igual
        GameObject charact = getPrefabs(unit);
        Debug.Log(charact);
        GameObject pieceObject = Instantiate(charact);
        pieceObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        pieceObject.transform.position = mover.transform.position;
        Debug.Log(pieceObject.transform.position);

        // processo para tornar o material da peça nova transparente
        Renderer[] renderers = pieceObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            // obtem o material original do renderer
            Material originalMaterial = renderer.material;

            // cria uma cópia do material para não se alterar o material original do prefab
            Material transparentMaterial = new Material(originalMaterial)
            {
                // redefine o shader para ser possível utilizar transparência
                shader = Shader.Find("Universal Render Pipeline/Lit")
            };// Ajusta as propriedades do material para torná-lo transparente
            transparentMaterial.SetFloat("_Surface", 1); // 1 é para Transparente
            transparentMaterial.SetFloat("_Blend", (float)UnityEngine.Rendering.BlendMode.DstAlpha);
            transparentMaterial.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
            transparentMaterial.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            transparentMaterial.SetFloat("_ZWrite", 0);
            transparentMaterial.DisableKeyword("_ALPHATEST_ON");
            transparentMaterial.EnableKeyword("_ALPHABLEND_ON");
            transparentMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            transparentMaterial.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            
            // Aplicar o material transparente ao renderer
            renderer.material = transparentMaterial;

            Color oldColor = renderer.material.color;
            Color newColor = new Color(oldColor.r, oldColor.g, oldColor.b, 0.5f);
            renderer.material.color = newColor; // Ajusta a cor do material
        }

        UnityEngine.Vector3[] positions = placePieces(unit.posFocoX,unit.posFocoY, gameTile);
        GameObject[] objects = game.getObjectsInTile(unit.posFocoX, unit.posFocoY);
        int i = 0;
        Debug.Log(objects);
        Debug.Log(unit.piece.id);
        foreach(Piece pi in game.getPiecesInTile(unit.posFocoX, unit.posFocoY)){
            if(unit.piece.Equals(pi)){
                targetPos = positions[i];
            }else{ 
                if(pi.getGameO()!=mover){
                pi.getGameO().transform.position = positions[i];
                i++;
                }
            }
        }
        // Move o objeto para a posição alvo (movimento)
        objectMover.StartMoving(mover, targetPos);
        // Corrotina para esperar alguns segundos antes de destruir o "fantasma"
        StartCoroutine(DestroyAfterSeconds(pieceObject, 2f));
    }

//Função que destroi objeto após X segundos
    private IEnumerator DestroyAfterSeconds(GameObject obj, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(obj);
    }

//Função que guarda as peças em cada casa no fim de cada turno
    public void SaveTurn(){
        //O dicionário que guarda a casa e a lista de peças lá é percorrido
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
   
    //funcao que avança de turno, também chamada quando se usa o botão
   public void NextTurn(){
    //se ainda existirem turnos para percorrer
        if (state.currentTurn < turnsList.Count - 1){
            isCalledByScene = false;
            SaveTurn();
            //avançar o turno
            state.currentTurn++;
            //colocar unit a 0  
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
        //coloca o jogo em Pausa para não existir concorrência com as alterações que estão a ser feitas
        Pause();
        goToPrevious = false;
        //verificação do turno atual válido
        if (state.currentTurn >= 0){  
            //a unit tem de estar em 0 se vamos começar um turno do inicio 
                state.currentUnit = 0;
                //caso estejamos no turno 0 ou 1 e quisermos recomeçar o turno anterior, o jogo recomeça
                if(state.currentTurn == 0 || state.currentTurn == 1){
                    //as peças atuais são destruidas
                    foreach (Piece piece in game.pieces){
                        Destroy(piece.getGameO());
                }
                
                game.pieces.Clear(); 
                state.currentTurn = 0;
                Play(); // faz o primeiro turno

            } else{
            //noutras situações
            //obter, da lista de dicionários para cada turno, a entrada para o turno que pretendemos
            Dictionary<(int, int), List<Piece>> turnPositions = oldTurnsPositions[state.currentTurn - 2];

            // Obter todas as peças em turnPositions
            HashSet<Piece> turnPositionPieces = new HashSet<Piece>(turnPositions.Values.SelectMany(list => list));

            // Obter as peças que não estão no turnPositions
            List<Piece> piecesToKill = game.pieces
                .Where(piece => !turnPositionPieces.Contains(piece))
                .ToList();

            // remover as peças que estão no jogo e não estavam quando o turno que queremos aceder começou
            foreach (Piece piece in piecesToKill){
                game.pieces.Remove(piece);
                GameObject peca = piece.getGameO();
                Destroy(peca);
            }

            // Obter todas as peças do jogo
            HashSet<Piece> gamePieces = new HashSet<Piece>(game.pieces);

            // Obter todas as peças que estavam no jogo mas entretanto morreram
            List<Piece> piecesToAdd = turnPositions.Values
                .SelectMany(list => list)
                .Where(piece => !gamePieces.Contains(piece))
                .ToList();

            //adicioná-las ao jogo novamente
            foreach (Piece piece in piecesToAdd){
                game.addPiece(piece);
                piece.getGameO().SetActive(true);
            }

            //chegando aqui temos: peças que não existiam apagadas, e peças que morreram mas estavam cá antes vivas again

//percorrendo cada par do dicionário
            foreach (KeyValuePair<(int, int), List<Piece>> kvp in turnPositions)
            {
                (int x, int y) = kvp.Key; //obtemos casa do tabuleiro
                Tile tile = board.BoardDisplay[x-1, y-1];
                GameObject gameTile = tile.getGameO();
                foreach(Piece piece in kvp.Value){
                    game.UpdatePosPiece(piece, x, y);
                }

                //posiciona-mos as peças na casa como estavam naquele turno
                UnityEngine.Vector3[] positions = placePieces(x, y, gameTile);
                GameObject[] objects = game.getObjectsInTile(x, y);
                int i = 0;
                foreach(GameObject obj in objects){
                    obj.transform.position = positions[i];
                    i++;
                }
            }
            //recua-se um turno
                state.currentTurn--;
                UpdateUI();
                Play();
            }         
        }
    }

    

    // Função que controla a paragem do jogo
    public void Pause()
    {
        // Define o estado de pausa como verdadeiro
        paused = true; 
        if (unitCoroutine != null) // verifica se há uma corrotina de uma unit em execução (ou seja se o jogo está a decorrer)
        {
            StopCoroutine(unitCoroutine); //Pára
            unitCoroutine = null; // Atualiza a variável turnCoroutine para null
            Time.timeScale = 0f; //põe o tempo do jogo em pausa, parando todas as animações e atualizações
            inputFieldManager.UpdateUI("", state.currentTurn + 1, "Paused"); //atualiza interface do utilizador
        }

    }

    public void Play()
    {
        paused = false;
        if (unitCoroutine == null) 
        {
            Time.timeScale = 1f;
            MakeTurn(turnsList[state.currentTurn]);// Recomeça o jogo do turno atual
            inputFieldManager.UpdateUI("", state.currentTurn + 1, "Game in Progress");
        }
    }

// Função que atualiza a interface do utilizador com o estado atual do jogo
    private void UpdateUI()
    {
        Debug.Log(state.currentTurn + state.currentUnit);
       // Obtém o nome do jogador cuja peça está a ser movida no turno atual
        string playerName = turnsList[state.currentTurn][state.currentUnit].piece.owner.name;
        int turnCount = state.currentTurn + 1;
        string gameStatus = "Game in Progress";
        inputFieldManager.UpdateUI(playerName, turnCount, gameStatus);
    }


//guardar estado do jogo
public struct GameState
{
    public int currentTurn; //turnoAtual
    public int currentUnit; //Unit atual
}
}
