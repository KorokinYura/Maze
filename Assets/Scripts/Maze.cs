using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MazeGenerator))]
public class Maze : MonoBehaviour
{
    [SerializeField]
    private int mazeHeight; // Must be odd
    [SerializeField]
    private int mazeWidth;  // Must be odd

    public int MazeHeight
    {
        get
        {
            return mazeHeight;
        }
        private set { }
    }
    public int MazeWidth
    {
        get
        {
            return mazeWidth;
        }
        private set { }
    }

    public bool IsMazeCreated { get; private set; }

    public static Maze Instance { get; private set; }
    public MazeGenerator MazeGenerator { get; private set; }

    public Cell[,] Cells { get; private set; }
    public List<GameObject> GroundCells { get; private set; }
    
    private void Awake()
    {
        Instance = this;
        GroundCells = new List<GameObject>();
        MazeGenerator = GetComponent<MazeGenerator>();
        Cells = new Cell[MazeWidth, MazeHeight];
    }

    public void GenerateMaze()
    {
        MazeGenerator.CreateMaze();
        IsMazeCreated = true;
    }

    public void DestroyMaze()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        IsMazeCreated = false;

        Awake();
    }
}
