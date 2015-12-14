using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class HexGrid : MonoBehaviour
{

    public GameObject hexPrefab;
    private static List<Path<Tile>> markedPaths = new List<Path<Tile>>();
    public int gridWidth = 10;
    public int gridHeight = 20;

    public float hexWidth;
    public float hexHeight;

    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    public List<TilePopulator> populators = new List<TilePopulator>();

    public static List<GameObject> villages = new List<GameObject>();

    //Method to calculate the position of the first hexagon tile
    //The center of the hex grid is (0,0,0)
    Vector3 initialPosition() {
        //the initial position will be in the left upper corner
        var x = -hexWidth*gridWidth/2f + hexWidth/2;
        var y = hexHeight * gridHeight / 2f * .75f - hexHeight / 2;
        return new Vector3(x, y, 0);
    }

    //method used to convert hex grid coordinates to game world coordinates
    public Vector3 toWorldPosition(Vector3 initPos, float x, float y)
    {
        //Every second row is offset by half of the tile width
        float offset = 0;
        if (y % 2 != 0) {
            offset = hexWidth / 2;
        }

        float worldX = initPos.x + offset + x * hexWidth;
        //Every new line is offset in z direction by 3/4 of the hexagon height
        float worldY = initPos.y - y * hexHeight * 0.75f;
        return new Vector3(worldX, worldY, 0);
    }

    //Finally the method which initialises and positions all the tiles
    GameObject[,] createGrid()
    {
        //Position of the first hex tile
        var initialPos = initialPosition();
        //Game object which is the parent of all the hex tiles
        var elements = new GameObject[gridWidth, gridHeight];
        var board = new Dictionary<Point, Tile>();
        
        for (int y = 0; y < gridHeight; y++)
        {
            for (float x = 0; x < gridWidth; x++)
            {
                var hex = (GameObject)Instantiate(hexPrefab);
                //Current position in grid
                hex.transform.position = toWorldPosition(initialPos, x, y);
                hex.transform.parent = this.gameObject.transform;
                var tile = new Tile((int)(x - (y / 2)), (int)y, hex);
                hex.GetComponent<TileHolder>().tile = tile;
                elements[(int)x, (int)y] = hex;
                board.Add(tile.Location, tile);
                // Debug.Log(hex.transform.position + " is tile: " + tile.X + " - " + tile.Y);
            }
        }

        foreach (var value in board.Values)
        {
            value.FindNeighbours(board);
        }

        return elements;

    }

    void setSizes()
    {
        var size = hexPrefab.GetComponent<Renderer>().bounds.size;
        hexWidth = size.x;
        hexHeight = size.y;
    }

    //The grid should be generated on game start
    void Start()
    {
        setSizes();
        var grid = createGrid();
        setupForCamera();

        foreach (var populator in populators)
        {
            populator.populate(grid);
        }

        findVillages(grid);

        SendMessage("GridReady", grid);
    }

    private void findVillages(GameObject[,] grid)
    {
        foreach (var hex in grid)
        {
            var v = hex.GetComponentInChildren<Village>();
            if (v)
            {
                villages.Add(v.gameObject);
            }
        }
    }

    private void setupForCamera()
    {
        var calcHeight = hexWidth / 2;
        calcHeight = (float) Math.Sqrt(hexHeight * hexHeight - calcHeight * calcHeight);

        var vertExtent = hexWidth * (gridHeight - 3);
        var horzExtent = calcHeight * (gridWidth - 3);

        Debug.Log(vertExtent + "x" + horzExtent);
        // Calculations assume map is position at the origin
        minX = - horzExtent / 2;
        maxX = horzExtent / 2;
        minY = -vertExtent / 2;
        maxY = vertExtent / 2;
    }

    public static GameObject drawPath<T>(IEnumerable<T> path, Color color, Func<T, Vector3> pos)
    {
        GameObject line = new GameObject("Debug_Line");
        var positions = path.Select(pos).ToArray();
        line.transform.position = positions.First();
        LineRenderer lr = line.AddComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Unlit/Color")) { color = color };
        lr.SetWidth(0.5f, 0.5f);
        lr.SetVertexCount(positions.Length);
        lr.SetPositions(positions);
        return line;
    }

    public static void clearTilePaths(Material normal)
    {
        foreach (var markedPath in markedPaths)
        {
            foreach (var tile in markedPath)
            {
                tile.GameObject.GetComponent<MeshRenderer>().material = normal;
            }
        }
        markedPaths.Clear();
    }


    public static void markTilePath(Path<Tile> path, Material coloring)
    {
        markedPaths.Add(path);
        foreach (var tile in path)
        {
            tile.GameObject.GetComponent<MeshRenderer>().material = coloring;
        }
    }

}
