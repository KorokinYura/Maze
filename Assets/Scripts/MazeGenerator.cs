using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public int[,] MazePlan { get; set; }

    [SerializeField]
    private GameObject wall;
    [SerializeField]
    private GameObject ground;

    [SerializeField]
    private float tileSize;

    public float TileSize
    {
        get
        {
            return tileSize;
        }
        private set { }
    }

    // element of random in wall spawning
    private readonly float placementThreshold = 0.1f;

    public void CreateMaze()
    {
        MazePlan = new int[Maze.Instance.MazeWidth, Maze.Instance.MazeHeight];
        Generate();
        Display();
    }


    public void Display()
    {
        for (int i = 0; i < Maze.Instance.MazeWidth; i++)
        {
            for (int j = 0; j < Maze.Instance.MazeHeight; j++)
            {
                GameObject curObject;
                Cell curCell;

                if (MazePlan[i, j] == 0)
                {
                    curObject = Instantiate(ground);
                    curCell = curObject.GetComponent<Ground>();

                    Maze.Instance.GroundCells.Add(curObject);
                }
                else
                {
                    curObject = Instantiate(wall);
                    curCell = curObject.GetComponent<Wall>();
                }

                Maze.Instance.Cells[i, j] = curCell;
                curCell.X = i;
                curCell.Y = j;

                curObject.transform.parent = Maze.Instance.transform;
                curObject.transform.position = new Vector2(i * tileSize, j * tileSize);
            }
        }

        CheckIfSurrounded();
    }
    
    private void CheckIfSurrounded()
    {
        foreach (var obj in Maze.Instance.GroundCells)
        {
            var cell = obj.GetComponent<Cell>();
            if (!Maze.Instance.Cells[cell.X + 1, cell.Y].GetComponent<Cell>().IsWalkable &&
                !Maze.Instance.Cells[cell.X - 1, cell.Y].GetComponent<Cell>().IsWalkable &&
                !Maze.Instance.Cells[cell.X, cell.Y + 1].GetComponent<Cell>().IsWalkable &&
                !Maze.Instance.Cells[cell.X, cell.Y - 1].GetComponent<Cell>().IsWalkable)
            {
                cell.IsBusy = true;
            }
        }
    }

    public void Generate()
    {
        for (int i = 0; i < Maze.Instance.MazeWidth; i++)
        {
            for (int j = 0; j < Maze.Instance.MazeHeight; j++)
            {
                // walls around maze
                if (i == 0 || j == 0 || i == Maze.Instance.MazeWidth - 1 || j == Maze.Instance.MazeHeight - 1)
                {
                    MazePlan[i, j] = 1;
                }
                // walls in maze
                else if (i % 2 == 0 && j % 2 == 0)
                {
                    if (Random.value > placementThreshold)
                    {
                        MazePlan[i, j] = 1;

                        int a = Random.value < 0.5 ? 0 : (Random.value < 0.5 ? -1 : 1);
                        int b = a != 0 ? 0 : (Random.value < 0.5 ? -1 : 1);
                        MazePlan[i + a, j + b] = 1;
                    }
                }
            }
        }
    }
}
