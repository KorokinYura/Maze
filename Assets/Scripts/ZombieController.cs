using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

public class ZombieController : Controller
{
    protected int[,] DistanceMap { get; set; }
    protected bool[,] IsNotEndCells { get; set; }
    protected bool IsMapReady { get; set; }
    protected List<Cell> FinalWay { get; set; }
    Dictionary<Cell, float> distances = new Dictionary<Cell, float>();

    protected const float delayBeforeEnd = 0.5f;



    protected void Start()
    {
        BaseSpeed = 1;
        Speed = BaseSpeed;
        DistanceMap = new int[Maze.Instance.MazeWidth, Maze.Instance.MazeHeight];
        IsNotEndCells = new bool[Maze.Instance.MazeWidth, Maze.Instance.MazeHeight];

        Spawner.Instance.Player.GetComponent<PlayerController>().OnCoinCollected += InceaseSpeed;
    }

    private void FixedUpdate()
    {
        if (GameController.Instance.IsGameStarted)
            Move();
    }


    protected void InceaseSpeed()
    {
        Speed += 0.05f * BaseSpeed;
    }

    protected override void Move()
    {
        CheckForPlayer();

        if (GameController.Instance.CoinsCollected >= 20)
            AggressiveMoveCount();

        if (IsMoving)
            return;

        if (GameController.Instance.CoinsCollected < 20)
            PassiveMove();
        else
        {
            AggressiveMove();
        }
    }

    protected void PassiveMove()
    {
        var adjacentCells = GetAdjacentCells(CurCell);

        if (adjacentCells.Count() == 0)
        {
            Debug.Log("Enemy can't move");
            return;
        }

        var nextCell = adjacentCells[Random.Range(0, adjacentCells.Count())];

        if (!IsAttacking)
            ChooseFlip(nextCell);

        CurCell.IsBusy = false;
        nextCell.IsBusy = true;

        CurCell = nextCell;
        StartCoroutine(MoveCoroutine(nextCell.transform.position));
    }

    protected void ChooseFlip(Cell nextCell)
    {
        if (nextCell.transform.position.x > CurCell.transform.position.x)
            SpriteRenderer.flipX = true;
        else if (nextCell.transform.position.x < CurCell.transform.position.x)
            SpriteRenderer.flipX = false;
    }

    protected void AggressiveMove()
    {
        if (FinalWay.Count() == 0)
        {
            return;
        }

        var nextCell = FinalWay.First();
        FinalWay.Remove(nextCell);

        if (!IsAttacking)
            ChooseFlip(nextCell);

        CurCell.IsBusy = false;
        nextCell.IsBusy = true;

        CurCell = nextCell;
        StartCoroutine(MoveCoroutine(nextCell.transform.position));
    }

    int countDelay = 50;

    protected void AggressiveMoveCount()
    {
        if (countDelay >= 50)
        {
            countDelay = 0;
            DistanceMap = new int[Maze.Instance.MazeWidth, Maze.Instance.MazeHeight];
            IsNotEndCells = new bool[Maze.Instance.MazeWidth, Maze.Instance.MazeHeight];
            DistanceMap[CurCell.X, CurCell.Y] = -1;   // curent position
            distances = new Dictionary<Cell, float>();
            IsMapReady = false;
            CountDistaceMap(CurCell, Spawner.Instance.Player.GetComponent<Controller>().CurCell);
        }
        countDelay++;
    }

    protected List<Cell> GetAdjacentCells(Cell curCell)
    {
        List<Cell> adjacentCells = new List<Cell>
        {
            Maze.Instance.Cells[curCell.X + 1, curCell.Y],
            Maze.Instance.Cells[curCell.X, curCell.Y + 1],
            Maze.Instance.Cells[curCell.X - 1, curCell.Y],
            Maze.Instance.Cells[curCell.X, curCell.Y - 1]
        }.Where(c => c.IsWalkable).ToList();

        return adjacentCells;
    }

    protected void CountDistaceMap(Cell curCell, Cell destination, int curDistance = 1)
    {
        if (IsMapReady)
            return;
        var adjacentCells = GetAdjacentCells(curCell);
        IsNotEndCells[curCell.X, curCell.Y] = true;

        foreach (var cell in adjacentCells)
        {
            if (destination.X == cell.X && destination.Y == cell.Y)
            {
                DistanceMap[cell.X, cell.Y] = curDistance;
                IsMapReady = true;
                GetWay(cell);
                return;
            }
            if (DistanceMap[cell.X, cell.Y] == 0)
            {
                distances.Add(cell, Vector2.Distance(cell.transform.position, destination.transform.position));
                DistanceMap[cell.X, cell.Y] = curDistance;
            }
        }

        var nextCell = distances.OrderBy(d => d.Value).First().Key;
        distances.Remove(nextCell);
        CountDistaceMap(nextCell, destination, curDistance + 1);
    }

    protected void GetWay(Cell cell)
    {
        int dist = DistanceMap[cell.X, cell.Y];
        FinalWay = new List<Cell>();
        while (dist > 0)
        {
            dist--;
            var nextCell = GetAdjacentCells(cell).FirstOrDefault(c => DistanceMap[c.X, c.Y] == dist && IsNotEndCells[c.X, c.Y]);

            if (nextCell == null)
            {
                DistanceMap[cell.X, cell.Y] = 0;
                nextCell = GetAdjacentCells(cell).FirstOrDefault(c => DistanceMap[c.X, c.Y] == dist + 1);
                continue;
            }

            FinalWay.Add(nextCell);
            cell = nextCell;
        }

        FinalWay.Reverse();
    }

    protected virtual void CheckForPlayer()
    {
        if (Spawner.Instance.Player != null)
            if (Vector2.Distance(gameObject.transform.position,
                                Spawner.Instance.Player.gameObject.transform.position)
                             <= Maze.Instance.MazeGenerator.TileSize / 2)
            {
                StartCoroutine(DelayBeforeEnd());
            }
    }

    protected IEnumerator DelayBeforeEnd()
    {
        IsAttacking = true;
        Animator.SetBool("isAttacking", true);
        yield return new WaitForSeconds(delayBeforeEnd);
        GameController.Instance.EndGame(EndReason.Death);
        Destroy(Spawner.Instance.Player);
    }
}
