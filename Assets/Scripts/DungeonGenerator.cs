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
    public int minChests = 2;              // Cantidad mínima de cofres por mapa
    public int maxChests = 3;              // Cantidad máxima de cofres por mapa




    public CameraFollow cameraFollow;



    private TileType[,] map;

    private List<Rect> rooms = new List<Rect>();

    void Start()
    {
        GenerateMap();
        RenderMap();
        PlaceStairs();
        PlaceEnemies();
        PlaceChests(); 

        // Colocar al jugador en el centro de la primera habitación
        if (rooms.Count > 0)
        {
            Vector2Int startPos = GetSafeSpawnPosition();
            Debug.Log("Instanciando jugador en posición: " + startPos);
            GameObject player = Instantiate(playerPrefab, new Vector3(startPos.x, startPos.y, 0), Quaternion.identity);
            Debug.Log("Jugador instanciado en: " + player.transform.position);

            // Asignar la cámara al jugador instanciado
            if (cameraFollow != null)
            {
                cameraFollow.target = player.transform;
            }
            else
            {
                Debug.LogWarning(" cameraFollow no está asignado en el inspector.");
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
        List<Vector2Int> floorPositions = new List<Vector2Int>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x, y] == TileType.Floor)
                {
                    floorPositions.Add(new Vector2Int(x, y));
                }
            }
        }

        if (floorPositions.Count > 0)
        {
            Vector2Int pos = floorPositions[Random.Range(0, floorPositions.Count)];
            Instantiate(stairPrefab, new Vector3(pos.x, pos.y, 0), Quaternion.identity);
        }
    }

    public void GenerateNewLevel()
    {
        // Destruir todos los objetos del mapa (excepto la cámara y este generador)
        foreach (GameObject obj in FindObjectsOfType<GameObject>())
        {
            if (obj != this.gameObject && obj.name != "Main Camera")
            {
                Destroy(obj);
            }
        }

        rooms.Clear();
        map = new TileType[width, height];

        GenerateMap();
        RenderMap();
        PlaceStairs();
        PlaceEnemies();
        PlaceChests();

        // Spawnear jugador de nuevo
        Vector2Int startPos = GetSafeSpawnPosition();
        GameObject player = Instantiate(playerPrefab, new Vector3(startPos.x, startPos.y, 0), Quaternion.identity);

        // Reasignar cámara
        if (cameraFollow != null)
        {
            cameraFollow.target = player.transform;
        }
    }

    void PlaceEnemies()
    {
        List<Vector2Int> validPositions = new List<Vector2Int>();

        // Recolectar todas las posiciones de tipo piso (Floor)
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

        // Spawnear enemigos en posiciones aleatorias
        for (int i = 0; i < enemiesPerLevel; i++)
        {
            if (validPositions.Count == 0) break;

            // Elegir una posición aleatoria y eliminarla para no repetir
            int index = Random.Range(0, validPositions.Count);
            Vector2Int pos = validPositions[index];
            validPositions.RemoveAt(index);

            // Elegir un prefab aleatorio de la lista
            int prefabIndex = Random.Range(0, enemyPrefabs.Length);
            GameObject enemyToSpawn = enemyPrefabs[prefabIndex];

            Instantiate(enemyToSpawn, new Vector3(pos.x, pos.y, 0), Quaternion.identity);
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







}
