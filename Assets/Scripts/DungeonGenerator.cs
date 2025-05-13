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

    private TileType[,] map;

    private List<Rect> rooms = new List<Rect>();

    void Start()
    {
        GenerateMap();
        RenderMap();
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
}
