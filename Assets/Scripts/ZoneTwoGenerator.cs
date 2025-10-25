using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneTwoGenerator : MonoBehaviour, IZoneGenerator
{
    public int width = 50;
    public int height = 50;
    public int roomCount = 6;
    public int roomWidth = 8;
    public int roomHeight = 6;

    
    public float tileSpacing = 0.5f;

    [Header("Tiles y prefabs")]
    public GameObject floorPrefab;
    public GameObject lampFloorPrefab;
    public GameObject topWallPrefab;
    public GameObject bottomWallPrefab;
    public GameObject leftWallPrefab;
    public GameObject rightWallPrefab;
    public GameObject bottomLeftCornerPrefab;
    public GameObject bottomRightCornerPrefab;
    [SerializeField] private GameObject _playerPrefab;
    public GameObject playerPrefab => _playerPrefab;
    public GameObject stairPrefab;

    private TileType[,] map;
    private List<Vector2Int> roomCenters = new List<Vector2Int>();
    private List<Vector2Int> lampPositions = new List<Vector2Int>();


    [Header("Enemigos")]
    [SerializeField] private GameObject[] enemyPrefabs;  // Flameskull, Zombie, Esqueleto
    [SerializeField] private int enemyCount = 6;         // Número de enemigos por mapa

    void Start()
    {
        GenerateMap();
        RenderMap();
        PlacePlayer();
        PlaceEnemies();
        PlaceStairs();
    }

    void GenerateMap()
    {
        map = new TileType[width, height];

        // Rellenar todo con paredes
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                map[x, y] = TileType.Wall;

        // Generar habitaciones rectangulares conectadas
        for (int i = 0; i < roomCount; i++)
        {
            int rx = Random.Range(1, width - roomWidth - 1);
            int ry = Random.Range(1, height - roomHeight - 1);
            Vector2Int center = new Vector2Int(rx + roomWidth / 2, ry + roomHeight / 2);
            roomCenters.Add(center);

            for (int x = rx; x < rx + roomWidth; x++)
            {
                for (int y = ry; y < ry + roomHeight; y++)
                {
                    map[x, y] = TileType.Floor;
                }
            }

            if (i > 0)
                ConnectRooms(roomCenters[i - 1], roomCenters[i]);
        }
    }

    void ConnectRooms(Vector2Int a, Vector2Int b)
    {
        int x = a.x, y = a.y;

        while (x != b.x)
        {
            map[x, y] = TileType.Floor;
            x += x < b.x ? 1 : -1;
        }

        while (y != b.y)
        {
            map[x, y] = TileType.Floor;
            y += y < b.y ? 1 : -1;
        }
    }

    void RenderMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x, y] == TileType.Floor)
                {
                    GameObject floor = Random.value < 0.02f ? lampFloorPrefab : floorPrefab;
                    if (Random.value < 0.02f)
                    {
                        Instantiate(lampFloorPrefab, new Vector3(x * tileSpacing, y * tileSpacing, 0), Quaternion.identity);
                        lampPositions.Add(new Vector2Int(x, y));  // Registrar posición
                    }
                    else
                    {
                        Instantiate(floorPrefab, new Vector3(x * tileSpacing, y * tileSpacing, 0), Quaternion.identity);
                    }

                }
                else
                {
                    RenderSmartWall(x, y);
                }
            }
        }
    }

    void RenderSmartWall(int x, int y)
    {
        if (!IsInsideMap(x, y)) return;

        bool up = IsFloor(x, y + 1);
        bool down = IsFloor(x, y - 1);
        bool left = IsFloor(x - 1, y);
        bool right = IsFloor(x + 1, y);
        bool upRight = IsFloor(x + 1, y + 1);
        bool upLeft = IsFloor(x - 1, y + 1);

        GameObject wallToPlace = null;

        if (down && !up && !left && !right)
            wallToPlace = topWallPrefab;
        else if (up && !down && !left && !right)
            wallToPlace = bottomWallPrefab;
        else if (right && !left)
            wallToPlace = leftWallPrefab;
        else if (left && !right)
            wallToPlace = rightWallPrefab;
        else if (upRight)
            wallToPlace = bottomLeftCornerPrefab;
        else if (upLeft)
            wallToPlace = bottomRightCornerPrefab;

        if (wallToPlace != null)
            Instantiate(wallToPlace, new Vector3(x * tileSpacing, y * tileSpacing, 0), Quaternion.identity);
    }

    bool IsFloor(int x, int y)
    {
        return IsInsideMap(x, y) && map[x, y] == TileType.Floor;
    }

    bool IsInsideMap(int x, int y)
    {
        return x >= 0 && y >= 0 && x < width && y < height;
    }

    void PlacePlayer()
    {
        Vector2Int spawn = roomCenters[0];
        GameObject player = Instantiate(playerPrefab, new Vector3(spawn.x * tileSpacing, spawn.y * tileSpacing, 0), Quaternion.identity);
        player.transform.localScale = Vector3.one;

        CameraFollow cam = FindObjectOfType<CameraFollow>();
        if (cam != null)
            cam.target = player.transform;
    }

    void PlaceStairs()
    {
        Vector2Int end = roomCenters[roomCenters.Count - 1];
        Instantiate(stairPrefab, new Vector3(end.x * tileSpacing, end.y * tileSpacing, 0), Quaternion.identity);
    }

    void PlaceEnemies()
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
        {
            Debug.LogWarning("No se asignaron prefabs de enemigos a ZoneTwoGenerator.");
            return;
        }

        List<Vector2Int> floorTiles = new List<Vector2Int>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x, y] == TileType.Floor)
                    floorTiles.Add(new Vector2Int(x, y));
            }
        }

        int spawned = 0;
        while (spawned < enemyCount && floorTiles.Count > 0)
        {
            int randomIndex = Random.Range(0, floorTiles.Count);
            Vector2Int pos = floorTiles[randomIndex];
            floorTiles.RemoveAt(randomIndex);

            // Evita spawnear en tiles con lámparas
            if (lampPositions.Contains(pos))
                continue;


            // Evita spawnear muy cerca del jugador
            if (spawned == 0 && Vector2.Distance(new Vector2(pos.x, pos.y), new Vector2(width / 2, height / 2)) < 4)
                continue;

            GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            GameObject enemy = Instantiate(prefab, new Vector3(pos.x * tileSpacing, pos.y * tileSpacing, 0), Quaternion.identity);
            enemy.transform.localScale = new Vector3(1.5f, 1.5f, 1f);


            var id = enemy.GetComponent<EnemyIdentifier>();
            if (id == null) id = enemy.AddComponent<EnemyIdentifier>();
            if (BattleLoader.Instance != null && BattleLoader.Instance.eliminatedEnemies.Contains(id.enemyID))
            {
                Destroy(enemy);
                continue;
            }

            var col = enemy.GetComponent<Collider2D>();
            if (col == null)
            {
                col = enemy.AddComponent<BoxCollider2D>();
                col.isTrigger = true;
            }

            if (enemy.GetComponent<EnemyTrigger>() == null)
            {
                var trig = enemy.AddComponent<EnemyTrigger>();
                trig.enemyPrefab = prefab;
            }

            var stats = enemy.GetComponent<CombatUnit>();
            if (stats != null)
            {
                stats.level += 2;
                stats.attack += 3;
                stats.defense += 2;
                stats.speed += 1;
            }

            spawned++;
        }

        Debug.Log($"Generados {spawned} enemigos en Zona 2.");
    }

    public void SaveCurrentMap()
    {
        // De momento, esta zona no guarda datos del mapa,
        // pero el método se deja vacío para cumplir con la interfaz.
    }


    public enum TileType
    {
        Wall,
        Floor
    }
}
