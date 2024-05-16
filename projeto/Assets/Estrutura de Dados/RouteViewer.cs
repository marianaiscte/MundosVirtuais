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
    public void SetPieceAndSprite(Piece p, Sprite ps)
    {
        piece = p;
        pointSprite = ps;
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

        // Instanciar pontos nas posições antigas
        //tentar primeiro com esferas porque agora não está a dar com sprites (imagens 2D)
        for (int i = 0; i < piece.oldPositions.Count; i++)
        {
            Tile tile = piece.game.board.BoardDisplay[piece.oldPositions[i].Item1 - 1, piece.oldPositions[i].Item2 - 1];
            GameObject gameTile = tile.getGameO();
            UnityEngine.Vector3 gameTilePos = gameTile.transform.position;
            Vector3 position = new Vector3(gameTilePos.x, gameTilePos.y + 1, gameTilePos.z); // Ajustar a altura conforme necessário
            GameObject point = new GameObject("RoutePoint");
            point.transform.position = position;
            SpriteRenderer sr = point.AddComponent<SpriteRenderer>();
            sr.sprite = pointSprite;
            routePoints.Add(point);
        }
    }


    private void RestoreState()
    {
        /* Reativar os outros GameObjects
        foreach (GameObject go in otherGameObjects)
        {
            go.SetActive(true);
        }
        otherGameObjects.Clear();
        */

        // Destruir os pontos de rota
        foreach (GameObject point in routePoints)
        {
            Destroy(point);
        }
        routePoints.Clear();

        isShowingRoute = false;
    }
}
