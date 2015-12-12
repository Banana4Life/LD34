using UnityEngine;
using System.Collections;

public class HexGrid : MonoBehaviour
{
    //following public variable is used to store the hex model prefab;
    //instantiate it by dragging the prefab on this variable using unity editor
    public GameObject hexPrefab;
    //next two variables can also be instantiated using unity editor
    public int gridWidth = 20;
    public int gridHeight = 10;

    //Hexagon tile width and height in game world
    private float hexWidth;
    private float hexHeight;

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
    void createGrid()
    {
        //Position of the first hex tile
        var initialPos = initialPosition();
        //Game object which is the parent of all the hex tiles
        var elements = new GameObject[gridWidth, gridHeight];
        
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                //GameObject assigned to hexPrefab public variable is cloned
                var hex = (GameObject)Instantiate(hexPrefab);
                //Current position in grid
                hex.transform.position = toWorldPosition(initialPos, x, y);
                hex.transform.parent = this.gameObject.transform;
                elements[x, y] = hex;
            }
        }


    }

    //Method to initialise Hexagon width and height
    void setSizes()
    {
        //renderer component attached to the hexPrefab prefab is used to get the current width and height
        var size = hexPrefab.GetComponent<Renderer>().bounds.size;
        hexWidth = size.x;
        hexHeight = size.y;
    }

    //The grid should be generated on game start
    void Start()
    {
        setSizes();
        createGrid();
    }
}
