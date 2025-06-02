using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType
{
    Wall,
    Floor
}

public class DungeonGenerator : MonoBehaviour
{
    public int width = 50;
    public int height = 50;
    public int roomCount = 5;
    public int roomMinSize = 5;
    public int roomMaxSize = 10;

    public GameObject floorPrefab;
    public GameObject wallPrefab;
    public GameObject playerPrefab;
    public GameObject stairPrefab;
    public GameObject[] enemyPrefabs; // Lista de prefabs de enemigos
    public int enemiesPerLevel = 5;   // Cuántos enemigos generar por nivel
    public GameObject chestPrefab;         // Prefab del cofre
    public int minChests = 1;              // Cantidad mínima de cofres por mapa
    public int maxChests = 2;              // Cantidad máxima de cofres por mapa




    public CameraFollow cameraFollow;



    private TileType[,] map;

    private List<Rect> rooms = new List<Rect>();

    IEnumerator Start()
    {
        // Espera un frame para que BattleLoader se inicialice por completo
        while (BattleLoader.Instance == null)
        {
            yield return null;
        }

        if (BattleLoader.Instance != null && BattleLoader.Instance.savedMapData != null)
        {
            Debug.Log("Restaurando mapa guardado...");
            RestoreSavedMap(BattleLoader.Instance.savedMapData);
            BattleLoader.Instance.savedMapData = null;
            yield break;
        }

        Debug.Log("No hay mapa guardado. Generando nuevo nivel...");
        GenerateMap();
        RenderMap();
        PlaceStairs();
        PlaceEnemies();
        PlaceChests();

        if (rooms.Count > 0)
        {
            Vector2Int startPos = GetSafeSpawnPosition();
            Debug.Log("Instanciando jugador en posición: " + startPos);

            


            CharacterData kasai = BattleLoader.Instance.GetCharacter("Kasai");

            if (kasai == null)
            {
                Debug.LogError("Kasai no se encontró en la party.");
                yield break;
            }

            GameObject kasaiPrefab = Resources.Load<GameObject>(kasai.prefabName);
            if (kasaiPrefab == null)
            {
                Debug.LogError("No se pudo cargar el prefab de Kasai desde Resources.");
                yield break;
            }

            GameObject player = Instantiate(kasaiPrefab, new Vector3(startPos.x, startPos.y, 0), Quaternion.identity);
            PlayerUnit playerUnit = player.GetComponent<PlayerUnit>();

            Animator anim = player.GetComponent<Animator>();
            if (anim != null)
            {
                anim.SetBool("inCombat", false);
            }


            // Asignar datos desde CharacterData
            playerUnit.unitName = kasai.id;
            playerUnit.level = kasai.level;
            playerUnit.currentHP = kasai.currentHP;
            playerUnit.maxHP = kasai.maxHP;
            playerUnit.attack = kasai.attack;
            playerUnit.defense = kasai.defense;
            playerUnit.speed = kasai.speed;
            playerUnit.currentXP = kasai.currentXP;
            playerUnit.xpToNextLevel = kasai.xpToNextLevel;
            playerUnit.baseAttackGrowth = kasai.baseAttackGrowth;
            playerUnit.baseDefenseGrowth = kasai.baseDefenseGrowth;
            playerUnit.baseSpeedGrowth = kasai.baseSpeedGrowth;




            Debug.Log("Jugador instanciado en: " + player.transform.position);

            if (cameraFollow != null)
            {
                cameraFollow.target = player.transform;
            }
            else
            {
                Debug.LogWarning("cameraFollow no está asignado en el inspector.");
            }
        }
    }


    void GenerateMap()
    {
        map = new TileType[width, height];

        // Llenar todo con paredes
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                map[x, y] = TileType.Wall;

        // Crear habitaciones
        for (int i = 0; i < roomCount; i++)
        {
            int w = Random.Range(roomMinSize, roomMaxSize);
            int h = Random.Range(roomMinSize, roomMaxSize);
            int x = Random.Range(1, width - w - 1);
            int y = Random.Range(1, height - h - 1);

            Rect newRoom = new Rect(x, y, w, h);

            bool overlaps = false;
            foreach (Rect room in rooms)
                if (newRoom.Overlaps(room))
                {
                    overlaps = true;
                    break;
                }

            if (!overlaps)
            {
                rooms.Add(newRoom);
                CreateRoom(newRoom);

                // Conectar habitaciones con túneles
                if (rooms.Count > 1)
                {
                    Vector2Int prevCenter = GetRoomCenter(rooms[rooms.Count - 2]);
                    Vector2Int currCenter = GetRoomCenter(newRoom);

                    CreateTunnel(prevCenter, currCenter);
                }
            }
        }

        

    }

    void CreateRoom(Rect room)
    {
        for (int x = (int)room.xMin; x < (int)room.xMax; x++)
            for (int y = (int)room.yMin; y < (int)room.yMax; y++)
                map[x, y] = TileType.Floor;
    }

    void CreateTunnel(Vector2Int a, Vector2Int b)
    {
        if (Random.value < 0.5f)
        {
            CreateHorizontalTunnel(a.x, b.x, a.y);
            CreateVerticalTunnel(a.y, b.y, b.x);
        }
        else
        {
            CreateVerticalTunnel(a.y, b.y, a.x);
            CreateHorizontalTunnel(a.x, b.x, b.y);
        }
    }

    void CreateHorizontalTunnel(int x1, int x2, int y)
    {
        for (int x = Mathf.Min(x1, x2); x <= Mathf.Max(x1, x2); x++)
            map[x, y] = TileType.Floor;
    }

    void CreateVerticalTunnel(int y1, int y2, int x)
    {
        for (int y = Mathf.Min(y1, y2); y <= Mathf.Max(y1, y2); y++)
            map[x, y] = TileType.Floor;
    }

    Vector2Int GetRoomCenter(Rect room)
    {
        return new Vector2Int((int)(room.x + room.width / 2), (int)(room.y + room.height / 2));
    }

    void RenderMap()
    {
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                GameObject tilePrefab = (map[x, y] == TileType.Floor) ? floorPrefab : wallPrefab;
                Instantiate(tilePrefab, new Vector3(x, y, 0), Quaternion.identity);
            }


    }

    Vector2Int GetSafeSpawnPosition()
    {
        Rect room = rooms[0];

        for (int x = Mathf.FloorToInt(room.xMin) + 1; x < Mathf.FloorToInt(room.xMax) - 1; x++)
        {
            for (int y = Mathf.FloorToInt(room.yMin) + 1; y < Mathf.FloorToInt(room.yMax) - 1; y++)
            {
                if (map[x, y] == TileType.Floor)
                {
                    Debug.Log("Spawn válido encontrado en: " + x + ", " + y);
                    return new Vector2Int(x, y);
                }
            }
        }

        Debug.LogWarning("No se encontró celda tipo piso, spawn fallback");
        return new Vector2Int(1, 1); // Coordenada de emergencia para evitar (0,0)
    }

    void PlaceStairs()
    {
        List<Vector2Int> validStairPositions = new List<Vector2Int>();
        Rect playerRoom = rooms[0];

        // Recolectar solo celdas de piso que NO estén en la sala del jugador
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x, y] == TileType.Floor)
                {
                    Vector2Int pos = new Vector2Int(x, y);
                    if (!playerRoom.Contains(new Vector2(pos.x + 0.5f, pos.y + 0.5f))) // +0.5 para coincidir con centro de celda
                    {
                        validStairPositions.Add(pos);
                    }
                }
            }
        }

        if (validStairPositions.Count > 0)
        {
            Vector2Int pos = validStairPositions[Random.Range(0, validStairPositions.Count)];
            Instantiate(stairPrefab, new Vector3(pos.x, pos.y, 0), Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("No se encontraron posiciones válidas fuera de la sala del jugador para colocar escaleras.");
        }
    }


    public void GenerateNewLevel()
    {
        Debug.Log(" Bajando al siguiente nivel... reseteando mapa.");

        // Borra el mapa guardado para forzar Start() a generar uno nuevo
        if (BattleLoader.Instance != null)
        {
            BattleLoader.Instance.savedMapData = null;
        }

        // Recarga la escena actual, lo que reiniciará Start()
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }


    void PlaceEnemies()
    {
        List<Vector2Int> validPositions = new List<Vector2Int>();

        // Recolectar todas las posiciones válidas de tipo piso
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x, y] == TileType.Floor)
                {
                    validPositions.Add(new Vector2Int(x, y));
                }
            }
        }

        for (int i = 0; i < enemiesPerLevel; i++)
        {
            if (validPositions.Count == 0) break;

            int index = Random.Range(0, validPositions.Count);
            Vector2Int pos = validPositions[index];
            validPositions.RemoveAt(index);

            int prefabIndex = Random.Range(0, enemyPrefabs.Length);
            GameObject enemyToSpawn = enemyPrefabs[prefabIndex];

            // Instanciar el enemigo en la posición deseada
            GameObject enemyInstance = Instantiate(enemyToSpawn, new Vector3(pos.x, pos.y, 0), Quaternion.identity);

            // Obtener o asignar identificador único
            EnemyIdentifier identifier = enemyInstance.GetComponent<EnemyIdentifier>();
            if (identifier == null)
            {
                identifier = enemyInstance.AddComponent<EnemyIdentifier>();
            }

            if (BattleLoader.Instance != null && BattleLoader.Instance.eliminatedEnemies.Contains(identifier.enemyID))
            {
                Debug.Log("Evitando reaparición del enemigo eliminado: " + identifier.enemyID);
                Destroy(enemyInstance); // Eliminarlo si fue derrotado antes
                i--; // Reintenta con otro enemigo
                continue;
            }

            // Agregar collider + trigger si es necesario
            Collider2D col = enemyInstance.GetComponent<Collider2D>();
            if (col == null) col = enemyInstance.AddComponent<BoxCollider2D>();
            col.isTrigger = true;

            // Agregar EnemyTrigger dinámicamente
            EnemyTrigger trigger = enemyInstance.AddComponent<EnemyTrigger>();
            trigger.enemyPrefab = enemyToSpawn;

            //Randomizar nivel de enemigo
            CombatUnit cu = enemyInstance.GetComponent<CombatUnit>();
            if (cu != null)
            {
                cu.level = Random.Range(1, 4); // Nivel entre 1 y 3
            }

        }
    }


    void PlaceChests()
    {
        List<Vector2Int> validPositions = new List<Vector2Int>();

        // Recolectar todas las celdas de tipo piso
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x, y] == TileType.Floor)
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

            Instantiate(chestPrefab, new Vector3(pos.x, pos.y, 0), Quaternion.identity);
        }
    }

    public void SaveCurrentMap()
    {
        if (BattleLoader.Instance == null) return;
        Debug.Log("Guardando mapa...");

        SavedMapData data = new SavedMapData();
        data.width = width;
        data.height = height;
        data.tileTypes = new TileType[width * height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                data.tileTypes[y * width + x] = map[x, y];
            }
        }

        foreach (var enemy in FindObjectsOfType<EnemyIdentifier>())
        {
            Vector2Int pos = Vector2Int.RoundToInt(enemy.transform.position);
            data.enemies.Add(new SavedEnemy
            {
                prefabName = enemy.gameObject.name.Replace("(Clone)", "").Trim(),
                position = pos,
                enemyID = enemy.enemyID
            });
        }

        foreach (var chest in GameObject.FindGameObjectsWithTag("Chest"))
        {
            data.chestPositions.Add(Vector2Int.RoundToInt(chest.transform.position));
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            data.playerPosition = Vector2Int.RoundToInt(player.transform.position);

        GameObject stairs = GameObject.FindGameObjectWithTag("Stairs");
        if (stairs != null)
            data.stairPosition = Vector2Int.RoundToInt(stairs.transform.position);

        BattleLoader.Instance.savedMapData = data;
    }


    void RestoreSavedMap(SavedMapData data)
    {
        width = data.width;
        height = data.height;

        map = new TileType[width, height];

        // Restaurar el layout del mapa (pisos y paredes)
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                map[x, y] = data.tileTypes[y * width + x];
            }
        }

        RenderMap();

        // Restaurar escaleras
        Instantiate(stairPrefab, new Vector3(data.stairPosition.x, data.stairPosition.y, 0), Quaternion.identity);

        // Restaurar cofres
        foreach (var pos in data.chestPositions)
        {
            GameObject chest = Instantiate(chestPrefab, new Vector3(pos.x, pos.y, 0), Quaternion.identity);
            chest.tag = "Chest"; // Asegura que mantenga su tag
        }

        // Restaurar enemigos no eliminados
        foreach (var enemyData in data.enemies)
        {
            if (BattleLoader.Instance.eliminatedEnemies.Contains(enemyData.enemyID))
            {
                Debug.Log("Ignorando enemigo derrotado: " + enemyData.enemyID);
                continue;
            }

            // Buscar el prefab por nombre
            GameObject prefab = FindEnemyPrefabByName(enemyData.prefabName);
            if (prefab == null)
            {
                Debug.LogWarning("No se encontró prefab para enemigo: " + enemyData.prefabName);
                continue;
            }

            GameObject enemy = Instantiate(prefab, new Vector3(enemyData.position.x, enemyData.position.y, 0), Quaternion.identity);

            // Asignar ID
            EnemyIdentifier identifier = enemy.GetComponent<EnemyIdentifier>();
            if (identifier == null) identifier = enemy.AddComponent<EnemyIdentifier>();
            identifier.enemyID = enemyData.enemyID;

            // Collider y trigger
            Collider2D col = enemy.GetComponent<Collider2D>();
            if (col == null) col = enemy.AddComponent<BoxCollider2D>();
            col.isTrigger = true;

            // Trigger de combate
            EnemyTrigger trigger = enemy.AddComponent<EnemyTrigger>();
            trigger.enemyPrefab = prefab;
        }

        // Restaurar jugador
        GameObject player = Instantiate(playerPrefab, new Vector3(data.playerPosition.x, data.playerPosition.y, 0), Quaternion.identity);
        player.tag = "Player";

        if (cameraFollow != null)
        {
            cameraFollow.target = player.transform;
        }
    }


    GameObject FindEnemyPrefabByName(string name)
    {
        foreach (GameObject prefab in enemyPrefabs)
        {
            if (prefab.name == name)
            {
                return prefab;
            }
        }
        return null;
    }









}
