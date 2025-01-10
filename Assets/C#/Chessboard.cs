using System;
using UnityEngine;
using UnityEngine.UIElements;

public class Chessboard : MonoBehaviour
{
    [Header("Art stuff")]
    [SerializeField] private Material tileMaterial;
    [SerializeField] private float tileSize = 1.0f;
    [SerializeField] private float yOffset = 0.2f;
    [SerializeField] private Vector3 boardCenter = Vector3.zero;


    [Header("Prefabs & Materials")]
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private Material[] teamMaterials;


    //Logic
    private ChessPice[,] chessPieces;
    private const int TILE_COUNT_X = 8;
    private const int TILE_COUNT_Y = 8;
    private GameObject[,] tiles;
    private Camera currentCamera;
    private Vector2Int currentHover; // store the current tile which hited by the ray.
    private Vector3 bounds;


    private void Awake()
    {
        GenerateAllTiles(tileSize, TILE_COUNT_X, TILE_COUNT_Y);

        SpawnAllPices();

        PositionAllPieces();
    }

    private void Update()
    {
        if (!currentCamera)
        {
            currentCamera = Camera.main;
            return;
        }

        RaycastHit info;
        Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out info, 100, LayerMask.GetMask("Tile", "Hover")))
        {
            // Get the indexes of the tile i've hit
            Vector2Int hitPosition = LookUpTileIndex(info.transform.gameObject);

            // If we're hovering a tile after not hovering any tiles
            if (currentHover == -Vector2Int.one)
            {
                tiles[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer("Tile");
                currentHover = -Vector2Int.one;
            }

            // If we were already hovering a tile, change the previous one
            if (currentHover != hitPosition)
            {
                tiles[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer("Tile");
                currentHover = hitPosition;
                tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
            }
            else
            {
                if (currentHover != -Vector2Int.one)
                {
                    currentHover = hitPosition;
                    tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
                }
            }
        }
    }

    //For to create tiles with a size and number of rows and colounms 
    private void GenerateAllTiles(float tileSize, int tileCountX, int tileCountY)
    {
        yOffset += transform.position.y;
        bounds = new Vector3((tileCountX / 2) * tileSize, 0, (tileCountX / 2) * tileSize) + boardCenter;


        tiles = new GameObject[tileCountX, tileCountY];
        for (int x = 0; x < tileCountX; x++)
        {
            for (int y = 0; y < tileCountY; y++)
            {
                tiles[x, y] = GenerateSingleTile(tileSize, x, y);
            }
        }
    }

    // this gonna create indivitual tiles.
    private GameObject GenerateSingleTile(float tileSize, int x, int y)
    {
        // the firt line create a new gameobject named tile.
        // the second line is set the parent of the tileboject to the current gameobject's transform.
        GameObject tileObject = new GameObject(string.Format("X:{0}, Y:{1}", x, y));
        tileObject.transform.parent = transform;

        // making a mesh for the gameobject.
        Mesh mesh = new Mesh();
        tileObject.AddComponent<MeshFilter>().mesh = mesh;
        tileObject.AddComponent<MeshRenderer>().material = tileMaterial;

        //make 4 poins for to create a square 
        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(x * tileSize, yOffset, y * tileSize) - bounds;
        vertices[1] = new Vector3(x * tileSize, yOffset, (y + 1) * tileSize) - bounds;
        vertices[2] = new Vector3((x + 1) * tileSize, yOffset, y * tileSize) - bounds;
        vertices[3] = new Vector3((x + 1) * tileSize, yOffset, (y + 1) * tileSize) - bounds;

        //connect the pooints to create a sqare.
        int[] tris = new int[] { 0, 1, 2, 1, 3, 2 };

        // add the vertieces and triangles to the mesh
        mesh.vertices = vertices;
        mesh.triangles = tris;

        // normals are vectors that perpendicular to the surface . 
        mesh.RecalculateNormals();

        tileObject.layer = LayerMask.NameToLayer("Tile");

        // adding box collider to the tile
        tileObject.AddComponent<BoxCollider>();

        return tileObject;
    }


    //Spwaning of the pieces
    private void SpawnAllPices()
    {
        chessPieces = new ChessPice[TILE_COUNT_X, TILE_COUNT_Y];

        int whiteTeam = 0, blackTeam = 1;

        //White team
        chessPieces[0, 0] = SpawnSinglePice(ChessPieceType.Rook, whiteTeam);
        chessPieces[1, 0] = SpawnSinglePice(ChessPieceType.Knight, whiteTeam);
        chessPieces[2, 0] = SpawnSinglePice(ChessPieceType.Bishop, whiteTeam);
        chessPieces[3, 0] = SpawnSinglePice(ChessPieceType.King, whiteTeam);
        chessPieces[4, 0] = SpawnSinglePice(ChessPieceType.Queen, whiteTeam);
        chessPieces[5, 0] = SpawnSinglePice(ChessPieceType.Bishop, whiteTeam);
        chessPieces[6, 0] = SpawnSinglePice(ChessPieceType.Knight, whiteTeam);
        chessPieces[7, 0] = SpawnSinglePice(ChessPieceType.Rook, whiteTeam);

        for(int i=0; i<TILE_COUNT_X; i++)
        {
            chessPieces[i, 1] = SpawnSinglePice(ChessPieceType.Pawn, whiteTeam);
        }

        //Black team
        chessPieces[0, 7] = SpawnSinglePice(ChessPieceType.Rook, blackTeam);
        chessPieces[1, 7] = SpawnSinglePice(ChessPieceType.Knight, blackTeam);
        chessPieces[2, 7] = SpawnSinglePice(ChessPieceType.Bishop, blackTeam);
        chessPieces[3, 7] = SpawnSinglePice(ChessPieceType.King, blackTeam);
        chessPieces[4, 7] = SpawnSinglePice(ChessPieceType.Queen, blackTeam);
        chessPieces[5, 7] = SpawnSinglePice(ChessPieceType.Bishop, blackTeam);
        chessPieces[6, 7] = SpawnSinglePice(ChessPieceType.Knight, blackTeam);
        chessPieces[7, 7] = SpawnSinglePice(ChessPieceType.Rook, blackTeam);

        for (int i = 0; i < TILE_COUNT_X; i++)
        {
            chessPieces[i, 6] = SpawnSinglePice(ChessPieceType.Pawn, blackTeam);
        }
    }
    private ChessPice SpawnSinglePice(ChessPieceType type, int team)
    {
        // Instantiate the chess piece
        ChessPice cp = Instantiate(prefabs[(int)type - 1], transform).GetComponent<ChessPice>();

        // Set the type and team
        cp.type = type;
        cp.team = team;

        // Assign the material based on the team
        cp.GetComponent<MeshRenderer>().material = teamMaterials[team];

        // Position the chess piece at the center of its tile
        Vector3 tilePosition = tiles[(int)cp.type, team].transform.position;
        cp.transform.position = tilePosition + new Vector3(0, yOffset, 0); // Adjust yOffset if necessary to align

        // Set rotation to (0, 0, 0)
        cp.transform.localRotation = Quaternion.identity;

        return cp;
    }


    //positioning
    private void PositionAllPieces()
    {
        for(int x=0;x<TILE_COUNT_X;x++)
        {
            for(int y = 0; y < TILE_COUNT_Y; y++)
            {
                if (chessPieces[x,y] != null)
                {
                    PositionSinglePiece(x, y, true);
                }
            }
        }
    }

    private void PositionSinglePiece(int x , int y , bool force = false)
    {
        chessPieces[x, y].currentX = x;
        chessPieces[x,y].currentY = y;
        chessPieces[x, y].transform.position = GetTileCenter(x, y);
    }
    private Vector3 GetTileCenter(int x, int y)
    {
        return new Vector3(x * tileSize, yOffset, y * tileSize) - bounds + new Vector3(tileSize / 2, 0, tileSize / 2);
    }

    // finds the index of hited gameobject and returns to the "hitPositioin".
    private Vector2Int LookUpTileIndex(GameObject hitInfo)
    {
        for (int x = 0; x < TILE_COUNT_X; x++)
        {
            for (int y = 0; y < TILE_COUNT_Y; y++)
            {
                if (tiles[x, y] == hitInfo)
                {
                    return new Vector2Int(x, y);
                }
            }
        }
        return -Vector2Int.one; //invalid;
    }
}