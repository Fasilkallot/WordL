using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raw : MonoBehaviour
{
   public Tile[] tiles {  get; private set; }

    private void Awake()
    {
        tiles = GetComponentsInChildren<Tile>();
    }
}
