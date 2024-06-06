using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script que trata do rasto percorrido pelo jogador
public class RouteViewer : MonoBehaviour
{
    Piece piece;
    private bool isShowingRoute = false;
    private LineRenderer lineRenderer;

    public void SetPiece(Piece p)
    {
        piece = p;
    }

    void Start()
    {
        // Adiciona um LineRenderer ao GameObject
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        //Largura da linha
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        // Aplica o material ao LineRenderer
        lineRenderer.material = Resources.Load<Material>("Materials/Path");;
        lineRenderer.positionCount = 0;
    }

    // Ao pressionar no boneco uma vez, mostra-se o caminho, ao voltar a pressionar ele desaparece
    void OnMouseDown()
    { 
        if (isShowingRoute)
        {
            RestoreState();
        }
        else
        { // se estiver o booleano estiver a false
            ShowRoute();
        }
    }

    private void ShowRoute()
    {
        if (piece == null) return;
        isShowingRoute = true;

        //lista para guardar as posições
        List<Vector3> routePositions = new List<Vector3>();

        // Adiciona as posições dos pontos da rota, que estão guardados na própria peça
        // Converte cada posição do tabuleiro para uma posição no espaço do jogo 
        for (int i = 0; i < piece.oldPositions.Count; i++)
        {
            Tile tile = piece.game.board.BoardDisplay[piece.oldPositions[i].Item1 - 1, piece.oldPositions[i].Item2 - 1];
            GameObject gameTile = tile.getGameO();
            Vector3 gameTilePos = gameTile.transform.position;
            Vector3 position = new Vector3(gameTilePos.x, gameTilePos.y, gameTilePos.z); 
            routePositions.Add(position);
        }
        routePositions.Add(piece.getGameO().transform.position); // posição atual da peça
        // Atualiza o LineRenderer com as posições da rota
        lineRenderer.positionCount = routePositions.Count;
        lineRenderer.SetPositions(routePositions.ToArray());
    }

    private void RestoreState()
    {
        // Remove a linha do tabuleiro
        lineRenderer.positionCount = 0;
        isShowingRoute = false;
    }
}