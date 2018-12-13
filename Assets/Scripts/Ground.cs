using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : Cell
{
    private void Awake()
    {
        IsWalkable = true;
    }
}
