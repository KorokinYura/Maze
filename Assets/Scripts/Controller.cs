using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Controller : MonoBehaviour
{
    public Cell CurCell { get; set; }

    protected float Speed { get; set; }
    protected float BaseSpeed { get; set; }
    protected bool IsMoving { get; set; }
    protected bool IsAttacking { get; set; }

    protected SpriteRenderer SpriteRenderer { get; set; }
    protected Animator Animator { get; set; }

    private void Awake()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
        Animator = GetComponent<Animator>();
    }

    protected abstract void Move();

    protected IEnumerator MoveCoroutine(Vector2 destination)
    {
        IsMoving = true;
        Vector2 direction = new Vector2(destination.x - gameObject.transform.position.x,
                                        destination.y - gameObject.transform.position.y);

        float walkIterations = Maze.Instance.MazeGenerator.TileSize / 
            (Vector2.Distance(gameObject.transform.position, destination) * 
            Maze.Instance.MazeGenerator.TileSize * Time.deltaTime * Speed);
        walkIterations *= 1.2f;
        float iterations = 0;

        while (Vector2.Distance(gameObject.transform.position, destination) > Maze.Instance.MazeGenerator.TileSize / 18 && 
            iterations <= walkIterations && !IsAttacking)
        {
            iterations++;
            yield return new WaitForEndOfFrame();
            gameObject.transform.Translate(direction * Maze.Instance.MazeGenerator.TileSize * Time.deltaTime * Speed);
        }
        if (!IsAttacking)
            gameObject.transform.position = new Vector3(destination.x, destination.y, gameObject.transform.position.z);
        IsMoving = false;
    }
}
