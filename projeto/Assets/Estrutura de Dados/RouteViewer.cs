using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RouteViewer : MonoBehaviour
{
    Piece piece;
    Sprite pointSprite; // Sprite das coisinhas do caminho
    private bool isShowingRoute = false;
    private List<GameObject> routePoints = new List<GameObject>();
    private List<GameObject> otherGameObjects = new List<GameObject>();
    private LineRenderer lineRenderer;

    public void SetPiece(Piece p)
    {
        piece = p;
    }

    void Start()
    {
        // Adiciona um LineRenderer ao GameObject
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        // Aplica o material ao LineRenderer
        lineRenderer.material = Resources.Load<Material>("Materials/Path");;
        lineRenderer.positionCount = 0;
    }

        //ir buscar as posições que estão guardadas na peça em piece.oldPositions
        //pôr pontos por cima desses tiles (no meio), e entre cada ponto/posição pôr pontos do mesmo prefab mas mais pequenos
        //pôr setActive a false dos outros bonecos todos
        //isto durar X segundos ou então qd se clica no boneco novamente volta a ficar normal

    void OnMouseDown()
    { 
        if (isShowingRoute)
        {
            RestoreState();
        }
        else
        {
            //otherGameObjects = piece.game.getAllObjects();
            ShowRoute();
        }
    }

    private void ShowRoute()
    {
        if (piece == null) return;
        isShowingRoute = true;

        /* Desativar outros GameObjects
        foreach (GameObject go in otherGameObjects)
        {
            if (go != this.gameObject && go != piece.getGameO())
            {
                go.SetActive(false);
            }
        }
        */

         List<Vector3> routePositions = new List<Vector3>();

        // Adiciona as posições dos pontos da rota
        for (int i = 0; i < piece.oldPositions.Count; i++)
        {
            Tile tile = piece.game.board.BoardDisplay[piece.oldPositions[i].Item1 - 1, piece.oldPositions[i].Item2 - 1];
            GameObject gameTile = tile.getGameO();
            Vector3 gameTilePos = gameTile.transform.position;
            Vector3 position = new Vector3(gameTilePos.x, gameTilePos.y, gameTilePos.z); // Ajustar a altura conforme necessário
            routePositions.Add(position);
        }
        routePositions.Add(piece.getGameO().transform.position);
        // Atualiza o LineRenderer com as posições da rota
        lineRenderer.positionCount = routePositions.Count;
        lineRenderer.SetPositions(routePositions.ToArray());
    }

    private void RestoreState()
    {
        // Reseta o LineRenderer
        lineRenderer.positionCount = 0;
        isShowingRoute = false;
    }
}