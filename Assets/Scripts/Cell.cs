using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Cell : MonoBehaviour
{
    public int X { get; set; }
    public int Y { get; set; }
    public bool IsWalkable { get; protected set; }
    public bool IsBusy { get; set; }
    
    public GameObject Coin { get; private set; }

    public void PlaceCoin(GameObject coin)
    {
        if (Coin != null)
        {
            Debug.Log("Coin is already placed in Cell[" + X + ", " + Y + "]");
            return;
        }
        Coin = coin;
    }

    public void RemoveCoin()
    {
        Destroy(Coin);
    }
}
