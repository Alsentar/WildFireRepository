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

    // ===============================
    //  GENERACIÓN DE ENEMIGOS - ZONA 2
    // ===============================
    void PlaceEnemies()
    {
        // Verifica que existan prefabs de enemigos configurados
        if (enemyPrefabs == null || enemyPrefabs.Length == 0)
        {
            Debug.LogWarning("[ZoneTwoGenerator] No se asignaron prefabs de enemigos.");
            return;
        }

        // -------------------------------
        //  Recolectar posiciones válidas de tipo piso
        // -------------------------------
        List<Vector2Int> validPositions = new List<Vector2Int>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x, y] == TileType.Floor)
                    validPositions.Add(new Vector2Int(x, y));
            }
        }

        // -------------------------------
        //  Generar enemigos según enemyCount
        // -------------------------------
        for (int i = 0; i < enemyCount; i++)
        {
            if (validPositions.Count == 0) break;

            // Seleccionar una posición aleatoria del mapa
            int index = Random.Range(0, validPositions.Count);
            Vector2Int pos = validPositions[index];
            validPositions.RemoveAt(index);

            // Evitar spawnear enemigos sobre lámparas
            if (lampPositions.Contains(pos))
                continue;

            // Seleccionar un prefab aleatorio del arreglo de enemigos
            int prefabIndex = Random.Range(0, enemyPrefabs.Length);
            GameObject enemyToSpawn = enemyPrefabs[prefabIndex];

            // Instanciar el enemigo en el mundo (respetando tileSpacing)
            GameObject enemyInstance = Instantiate(
                enemyToSpawn,
                new Vector3(pos.x * tileSpacing, pos.y * tileSpacing, 0),
                Quaternion.identity
            );

            // Ajustar su escala para mantener coherencia visual
            enemyInstance.transform.localScale = new Vector3(1.5f, 1.5f, 1f);

            // -------------------------------
            //  Asignar identificador único (EnemyIdentifier)
            // -------------------------------
            EnemyIdentifier identifier = enemyInstance.GetComponent<EnemyIdentifier>();
            if (identifier == null)
                identifier = enemyInstance.AddComponent<EnemyIdentifier>();

            // Si el enemigo ya fue derrotado antes, evitar reaparición
            if (BattleLoader.Instance != null && BattleLoader.Instance.eliminatedEnemies.Contains(identifier.enemyID))
            {
                Debug.Log($"[ZoneTwoGenerator] Evitando reaparición del enemigo eliminado: {identifier.enemyID}");
                Destroy(enemyInstance);
                i--; // Reintentar con otro enemigo
                continue;
            }

            // -------------------------------
            //  Asegurar colisión y trigger
            // -------------------------------
            Collider2D col = enemyInstance.GetComponent<Collider2D>();
            if (col == null)
                col = enemyInstance.AddComponent<BoxCollider2D>();
            col.isTrigger = true;

            // -------------------------------
            //  Asignar EnemyTrigger dinámicamente (igual que en Nivel 1)
            // -------------------------------
            EnemyTrigger trigger = enemyInstance.AddComponent<EnemyTrigger>();
            trigger.enemyPrefab = enemyToSpawn; // Prefab original del Project

            // -------------------------------
            //  Ajustar estadísticas (Zona 2 = enemigos más fuertes)
            // -------------------------------
            CombatUnit cu = enemyInstance.GetComponent<CombatUnit>();
            if (cu != null)
            {
                cu.level += 2;
                cu.attack += 3;
                cu.defense += 2;
                cu.speed += 1;
            }
        }

        Debug.Log($"[ZoneTwoGenerator] Generados {enemyCount} enemigos en Zona 2.");
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
