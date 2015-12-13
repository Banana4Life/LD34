using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class HexGrid : MonoBehaviour
{

    public GameObject hexPrefab;
    public int gridWidth = 10;
    public int gridHeight = 20;

    private float hexWidth;
    private float hexHeight;

    public List<GridPopulator> populators = new List<GridPopulator>();

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
                hex.GetComponent<TileBehaviour>().tile = tile;
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

        foreach (var populator in populators)
        {
            populator.populate(grid);
        }

        var start = grid[0, 0].GetComponent<TileBehaviour>().tile;
        var end = grid[grid.GetLength(0) - 1, grid.GetLength(1) - 1].GetComponent<TileBehaviour>().tile;

        var path = PathFinder.FindPath<Tile>(start, end);
        if (path != null)
        {
            drawPath(path, Color.red, t => t.GameObject.transform.position);
        }

        // TODO spawn me somewhere else!
        var walker = GameObject.Find("legionnaire_spear").AddComponent<PathWalker>();
        walker.followPath(path);

    }

    public static void drawPath<T>(IEnumerable<T> path, Color color, Func<T, Vector3> pos)
    {
        GameObject line = new GameObject("Debug_Line");
        var positions = path.Select(pos).ToArray();
        line.transform.position = positions.First();
        LineRenderer lr = line.AddComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Unlit/Color")) { color = color };
        lr.SetWidth(0.5f, 0.5f);
        lr.SetVertexCount(positions.Length);
        lr.SetPositions(positions);
    }
}
