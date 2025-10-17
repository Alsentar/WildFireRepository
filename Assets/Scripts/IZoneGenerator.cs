using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IZoneGenerator
{
    GameObject playerPrefab { get; }
    void SaveCurrentMap();
}
