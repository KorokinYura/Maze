using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Spawner))]
public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    [SerializeField]
    private float delayBetweenCoins;
    private bool isDelayWorking;

    [SerializeField]
    private int coinCoef;
    [SerializeField]
    private int maxCoinsAmmount;
    public int CurCoinsAmmount { get; private set; }
    public int CoinsCollected { get; set; }

    private int zombiesAmmount = 0;
    private bool isMummySpawned;

    public bool IsGameStarted { get; private set; }

    private Spawner SpawnerInstance { get; set; }

    public Result TempResult { get; private set; }



    private void Awake()
    {
        Instance = this;
        SpawnerInstance = GetComponent<Spawner>();
        TempResult = new Result();
    }

    private void Update()
    {
        if (IsGameStarted)
        {
            if (!Maze.Instance.IsMazeCreated)
                Maze.Instance.GenerateMaze();
            if (Spawner.Instance.Player == null)
                Spawner.Instance.SpawnPlayer();
            SpawnCoins();
            SpawnEnemies();

            CheckForExit();
        }
    }

    private void CheckForExit()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            EndGame();
        }
    }


    public void StartGame()
    {
        IsGameStarted = true;
        UIController.Instance.StartGame();

        TempResult.alias = PlayerPrefs.GetString("alias", "");
        TempResult.startDate = DateTime.Now;
    }

    public void EndGame(EndReason endReason = EndReason.Escape)
    {
        IsGameStarted = false;
        UIController.Instance.EndGame();
        Maze.Instance.DestroyMaze();

        TempResult.lifetime = (int)TimeSpan.FromTicks(DateTime.Now.Ticks - TempResult.startDate.Ticks).TotalSeconds;
        TempResult.endReason = endReason.ToString();
        TempResult.coinsCollected = CoinsCollected;
        XmlController.UpdateResults(TempResult);

        CoinsCollected = 0;
        CurCoinsAmmount = 0;
        zombiesAmmount = 0;
        isMummySpawned = false;

    }

    public void AddPoints()
    {
        CurCoinsAmmount--;
        CoinsCollected += coinCoef;
    }

    private void SpawnCoins()
    {
        if (CurCoinsAmmount < maxCoinsAmmount && !isDelayWorking)
        {
            if (SpawnerInstance.SpawnCoin())
            {
                CurCoinsAmmount++;
            }
            StartCoroutine(SpawnDelay());
        }
    }

    private IEnumerator SpawnDelay()
    {
        isDelayWorking = true;
        yield return new WaitForSeconds(delayBetweenCoins);
        isDelayWorking = false;
    }

    private void SpawnEnemies()
    {
        if (zombiesAmmount == 0)
        {
            SpawnerInstance.SpawnZombie();
            zombiesAmmount++;
        }
        else if (CoinsCollected >= 5 && zombiesAmmount == 1)
        {
            SpawnerInstance.SpawnZombie();
            zombiesAmmount++;
        }
        else if (CoinsCollected >= 10 && !isMummySpawned)
        {
            SpawnerInstance.SpawnMummy();
            isMummySpawned = true;
        }
    }
}
