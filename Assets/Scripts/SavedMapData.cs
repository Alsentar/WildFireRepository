using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//[Serializable]
public class SavedMapData
{
    public int width;
    public int height;
    public TileType[] tileTypes; // Flattened array

    public List<SavedEnemy> enemies = new List<SavedEnemy>();
    public List<Vector2Int> chestPositions = new List<Vector2Int>();
    public Vector2Int playerPosition;
    public Vector2Int stairPosition;
}

//[Serializable]
public class SavedEnemy
{
    public string prefabName;
    public Vector2Int position;
    public string enemyID;
}
