using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : Cell
{
    private void Awake()
    {
        IsWalkable = false;
    }
}
