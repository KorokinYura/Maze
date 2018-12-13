using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public static Spawner Instance { get; private set; }

    [SerializeField]
    private GameObject player;
    [SerializeField]
    private GameObject coin;
    [SerializeField]
    private GameObject zombie;
    [SerializeField]
    private GameObject mummy;

    private Vector2 PlayerSpawnCoordinates { get; set; }
    public GameObject Player { get; private set; }

    private void Awake()
    {
        Instance = this;
        PlayerSpawnCoordinates = new Vector2(1, 1);
    }
    
    public void SpawnPlayer()
    {
        if (player == null)
        {
            Debug.Log("Player prefub is null");
        }

        var p = Instantiate(player);
        var cell = Maze.Instance.Cells[(int)PlayerSpawnCoordinates.x, (int)PlayerSpawnCoordinates.y];
        cell.GetComponent<Cell>().IsBusy = true;
        p.transform.position = new Vector3(cell.transform.position.x, cell.transform.position.y, p.transform.position.z);
        p.GetComponent<Controller>().CurCell = Maze.Instance.Cells[(int)PlayerSpawnCoordinates.x, (int)PlayerSpawnCoordinates.y];
        Player = p;
        p.transform.parent = Maze.Instance.gameObject.transform;
    }

    public void SpawnZombie()
    {
        SpawnEnemy(zombie);
    }

    public void SpawnMummy()
    {
        SpawnEnemy(mummy);
    }

    private void SpawnEnemy(GameObject enemy)
    {
        if (enemy == null)
        {
            Debug.Log("Object to spawn is null");
            return;
        }

        var cell = ChooseEnemySpawnPoint();
        var newEnemy = Instantiate(enemy);
        newEnemy.transform.position = new Vector3(cell.transform.position.x, cell.transform.position.y, newEnemy.transform.position.z);
        newEnemy.GetComponent<Controller>().CurCell = Maze.Instance.Cells[cell.GetComponent<Cell>().X,
                                                                          cell.GetComponent<Cell>().Y];
        newEnemy.transform.parent = Maze.Instance.gameObject.transform;
    }

    private Cell ChooseEnemySpawnPoint()
    {
        var spawnList = new List<Cell>
        {
            Maze.Instance.Cells[Maze.Instance.MazeWidth - 2, Maze.Instance.MazeHeight - 2],
            Maze.Instance.Cells[1, Maze.Instance.MazeHeight - 2],
            Maze.Instance.Cells[Maze.Instance.MazeWidth - 2, 1],
        };
        return spawnList[Random.Range(0, spawnList.Count)];
    }

    public bool SpawnCoin()
    {
        if (coin == null)
        {
            Debug.Log("Coin is null");
        }

        var cell = ChooseSpawnPoint();
        if (cell == null)
        {
            Debug.Log("All cells are busy");
            return false;
        }
        var c = Instantiate(coin);
        cell.GetComponent<Cell>().IsBusy = true;
        cell.GetComponent<Cell>().PlaceCoin(c);
        c.transform.position = new Vector3(cell.transform.position.x, cell.transform.position.y, c.transform.position.z);
        c.transform.parent = Maze.Instance.gameObject.transform;

        return true;
    }

    private GameObject ChooseSpawnPoint()
    {
        var cells = Maze.Instance.GroundCells.Where(c => c.GetComponent<Cell>().IsBusy == false);
        if (cells.Count() > 0)
            return cells.ElementAt(Random.Range(0, cells.Count()));
        else
            return null;
    }
}
