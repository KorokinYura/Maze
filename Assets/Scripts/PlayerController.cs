using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Controller
{
    public event Action OnCoinCollected;



    private void Start()
    {
        Speed = 4;
        Animator.speed = Speed / 2;
    }

    private void Update()
    {
        if (GameController.Instance.IsGameStarted)
            Move();
    }


    protected override void Move()
    {
        Animator.SetBool("isMoving", IsMoving);

        if (IsMoving)
            return;

        Cell nextCell = CurCell;

        if (Input.GetKey(KeyCode.W))
        {
            nextCell = Maze.Instance.Cells[CurCell.X, CurCell.Y + 1];
        }
        else if (Input.GetKey(KeyCode.S))
        {
            nextCell = Maze.Instance.Cells[CurCell.X, CurCell.Y - 1];
        }
        else if (Input.GetKey(KeyCode.A))
        {
            SpriteRenderer.flipX = true;
            nextCell = Maze.Instance.Cells[CurCell.X - 1, CurCell.Y];
        }
        else if (Input.GetKey(KeyCode.D))
        {
            SpriteRenderer.flipX = false;
            nextCell = Maze.Instance.Cells[CurCell.X + 1, CurCell.Y];
        }

        if (nextCell.IsWalkable && nextCell != CurCell)
        {
            CheckIfCoin(nextCell);

            CurCell.IsBusy = false;
            nextCell.IsBusy = true;

            CurCell = nextCell;
            StartCoroutine(MoveCoroutine(nextCell.transform.position));
        }
    }

    private void CheckIfCoin(Cell cell)
    {
        if (cell.Coin != null)
        {
            GameController.Instance.AddPoints();
            OnCoinCollected();
            cell.RemoveCoin();
        }
    }
}
