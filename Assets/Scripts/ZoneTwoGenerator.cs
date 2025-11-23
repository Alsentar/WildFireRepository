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
    public GameObject chestPrefab;         // Prefab del cofre
    public int minChests = 1;              // Cantidad mínima de cofres por mapa
    public int maxChests = 2;              // Cantidad máxima de cofres por mapa

    private Zone2TileType[,] map;
    private List<Vector2Int> roomCenters = new List<Vector2Int>();
    private List<Vector2Int> lampPositions = new List<Vector2Int>();

    [Header("Enemigos")]
    [SerializeField] private GameObject[] enemyPrefabs;  // Flameskull, Zombie, Esqueleto
    [SerializeField] private int enemyCount = 6;         // Número de enemigos por mapa

    private List<Rect> rooms = new List<Rect>();

    void Start()
    {
        BattleLoader.Instance.currentZone = 2;

        if (BattleLoader.Instance != null && BattleLoader.Instance.savedMapData != null)
        {
            Debug.Log("[ZoneTwoGenerator] Restaurando mapa guardado...");
            RestoreSavedMap(BattleLoader.Instance.savedMapData);
            BattleLoader.Instance.savedMapData = null;
            return;
        }

        // Si no hay mapa guardado, generar nuevo
        GenerateMap();
        RenderMap();
        PlacePlayer();
        PlaceEnemies();
        PlaceStairs();
        PlaceChests();
    }

    void GenerateMap()
    {
        map = new Zone2TileType[width, height];

        // Rellenar todo con paredes
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                map[x, y] = Zone2TileType.Wall;

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
                    map[x, y] = Zone2TileType.Floor;
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
            map[x, y] = Zone2TileType.Floor;
            x += x < b.x ? 1 : -1;
        }

        while (y != b.y)
        {
            map[x, y] = Zone2TileType.Floor;
            y += y < b.y ? 1 : -1;
        }
    }

    void RenderMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x, y] == Zone2TileType.Floor)
                {
                    if (Random.value < 0.02f)
                    {
                        Instantiate(lampFloorPrefab, new Vector3(x * tileSpacing, y * tileSpacing, 0), Quaternion.identity);
                        lampPositions.Add(new Vector2Int(x, y));
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
        return IsInsideMap(x, y) && map[x, y] == Zone2TileType.Floor;
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
                if (map[x, y] == Zone2TileType.Floor)
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

    

    void PlaceStairs()
    {
        List<Vector2Int> validStairPositions = new List<Vector2Int>();

        // Recolectar todas las celdas que son piso
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x, y] == Zone2TileType.Floor)
                {
                    validStairPositions.Add(new Vector2Int(x, y));
                }
            }
        }

        if (validStairPositions.Count == 0)
        {
            Debug.LogError("No se encontraron celdas válidas para colocar escaleras en Zona 2.");
            return;
        }

        // Elegir una celda de piso aleatoria
        Vector2Int pos = validStairPositions[Random.Range(0, validStairPositions.Count)];

        // Convertir a coordenadas del mundo (Zona 2 usa tileSpacing)
        Vector3 worldPos = new Vector3(pos.x * tileSpacing, pos.y * tileSpacing, 0);

        Instantiate(stairPrefab, worldPos, Quaternion.identity);
    }

    void PlaceChests()
    {
        List<Vector2Int> validPositions = new List<Vector2Int>();

        // Recolectar todas las celdas de tipo piso
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x, y] == Zone2TileType.Floor)
                {
                    validPositions.Add(new Vector2Int(x, y));
                }
            }
        }

        // Decidir aleatoriamente cuántos cofres colocar
        int chestCount = Random.Range(minChests, maxChests + 1);

        for (int i = 0; i < chestCount; i++)
        {
            if (validPositions.Count == 0) break;

            // Elegir posición aleatoria y eliminarla para evitar repeticiones
            int index = Random.Range(0, validPositions.Count);
            Vector2Int pos = validPositions[index];
            validPositions.RemoveAt(index);

            Instantiate(chestPrefab, new Vector3(pos.x * tileSpacing, pos.y * tileSpacing, 0), Quaternion.identity);

        }
    }


    // ===============================
    //  GUARDAR MAPA COMPLETO (ZONA 2)
    // ===============================
    public void SaveCurrentMap()
    {
        if (BattleLoader.Instance == null) return;
        Debug.Log("[ZoneTwoGenerator] Guardando mapa completo...");

        SavedMapData data = new SavedMapData();
        data.width = width;
        data.height = height;
        data.tileTypes = new TileType[width * height]; // Usamos el TileType GLOBAL

        // Guardar cada celda del mapa
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                data.tileTypes[y * width + x] = (TileType)map[x, y]; // conversión explícita
            }
        }

        // Guardar enemigos actuales
        foreach (var enemy in FindObjectsOfType<EnemyIdentifier>())
        {
            Vector2Int pos = Vector2Int.RoundToInt(enemy.transform.position / tileSpacing);
            data.enemies.Add(new SavedEnemy
            {
                prefabName = enemy.gameObject.name.Replace("(Clone)", "").Trim(),
                position = pos,
                enemyID = enemy.enemyID
            });
        }

        // Guardar cofres
        foreach (var chest in GameObject.FindGameObjectsWithTag("Chest"))
        {
            Vector2Int pos = Vector2Int.RoundToInt(chest.transform.position / tileSpacing);
            data.chestPositions.Add(pos);
        }

        // Guardar posición del jugador
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            data.playerPosition = Vector2Int.RoundToInt(player.transform.position / tileSpacing);

        // Guardar posición de las escaleras
        GameObject stairs = GameObject.FindGameObjectWithTag("Stairs");
        if (stairs != null)
            data.stairPosition = Vector2Int.RoundToInt(stairs.transform.position / tileSpacing);

        BattleLoader.Instance.savedMapData = data;
    }

    // ===============================
    //  RESTAURAR MAPA GUARDADO (ZONA 2)
    // ===============================
    void RestoreSavedMap(SavedMapData data)
    {
        Debug.Log("[ZoneTwoGenerator] Restaurando mapa guardado (layout completo)...");

        width = data.width;
        height = data.height;
        map = new Zone2TileType[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                map[x, y] = (Zone2TileType)data.tileTypes[y * width + x];
            }
        }

        RenderMap();

        // Validar que la escalera esté en un tile de piso
        Vector2Int restored = data.stairPosition;

        if (!IsInsideMap(restored.x, restored.y) || map[restored.x, restored.y] != Zone2TileType.Floor)
        {
            Debug.LogWarning("[ZoneTwoGenerator] Stair was saved in invalid position, searching closest floor...");
            restored = FindNearestValidFloor(restored);
        }

        Instantiate(
            stairPrefab,
            new Vector3(restored.x * tileSpacing, restored.y * tileSpacing, 0),
            Quaternion.identity
        );


        foreach (var pos in data.chestPositions)
        {
            Instantiate(chestPrefab, new Vector3(pos.x * tileSpacing, pos.y * tileSpacing, 0), Quaternion.identity);
        }


        foreach (var enemyData in data.enemies)
        {
            if (BattleLoader.Instance.eliminatedEnemies.Contains(enemyData.enemyID))
                continue;

            GameObject prefab = FindEnemyPrefabByName(enemyData.prefabName);
            if (prefab == null)
                continue;

            GameObject enemy = Instantiate(prefab, new Vector3(enemyData.position.x * tileSpacing, enemyData.position.y * tileSpacing, 0), Quaternion.identity);

            //  Normalizar escala visual igual que en PlaceEnemies()
            enemy.transform.localScale = new Vector3(1.5f, 1.5f, 1f);

            EnemyIdentifier id = enemy.GetComponent<EnemyIdentifier>() ?? enemy.AddComponent<EnemyIdentifier>();
            id.enemyID = enemyData.enemyID;


            Collider2D col = enemy.GetComponent<Collider2D>() ?? enemy.AddComponent<BoxCollider2D>();
            col.isTrigger = true;

            EnemyTrigger trigger = enemy.AddComponent<EnemyTrigger>();
            trigger.enemyPrefab = prefab;
        }

        GameObject player = Instantiate(playerPrefab, new Vector3(data.playerPosition.x * tileSpacing, data.playerPosition.y * tileSpacing, 0), Quaternion.identity);

        //  Normalizar escala para que coincida con el jugador original
        player.transform.localScale = Vector3.one;

        //  Para que se vea mas grande:
        // player.transform.localScale = new Vector3(1.5f, 1.5f, 1f);

        CameraFollow cam = FindObjectOfType<CameraFollow>();
        if (cam != null)
            cam.target = player.transform;

    }

    Vector2Int FindNearestValidFloor(Vector2Int origin)
    {
        Queue<Vector2Int> frontier = new Queue<Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        frontier.Enqueue(origin);
        visited.Add(origin);

        Vector2Int[] dirs = {
        new Vector2Int(1,0), new Vector2Int(-1,0),
        new Vector2Int(0,1), new Vector2Int(0,-1)
    };

        while (frontier.Count > 0)
        {
            Vector2Int pos = frontier.Dequeue();

            if (IsInsideMap(pos.x, pos.y) && map[pos.x, pos.y] == Zone2TileType.Floor)
                return pos;

            foreach (var d in dirs)
            {
                Vector2Int next = pos + d;
                if (!visited.Contains(next))
                {
                    visited.Add(next);
                    frontier.Enqueue(next);
                }
            }
        }

        // fallback de emergencia
        return new Vector2Int(1, 1);
    }


    GameObject FindEnemyPrefabByName(string name)
    {
        foreach (GameObject prefab in enemyPrefabs)
        {
            if (prefab != null && prefab.name == name)
            {
                return prefab;
            }
        }

        Debug.LogWarning($"[ZoneTwoGenerator] No se encontró ningún prefab que coincida con el nombre: {name}");
        return null;
    }

    // ===============================
    //  ENUM LOCAL PARA ZONA 2
    // ===============================
    public enum Zone2TileType
    {
        Wall,
        Floor
    }
}
