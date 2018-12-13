using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MummyController : ZombieController
{
    protected new void Start()
    {
        base.Start();
        BaseSpeed *= 2;
        Speed = BaseSpeed;
    }

    protected override void CheckForPlayer()
    {
        if (Spawner.Instance.Player != null)
            if (Vector2.Distance(gameObject.transform.position,
                                Spawner.Instance.Player.gameObject.transform.position)
                             <= Maze.Instance.MazeGenerator.TileSize / 2)
            {
                GameController.Instance.CoinsCollected = 0;
                StartCoroutine(DelayBeforeEnd());
            }
    }
}
