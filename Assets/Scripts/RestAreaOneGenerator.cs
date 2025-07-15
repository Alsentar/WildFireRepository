using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestAreaGenerator : MonoBehaviour
{
    public int width = 10;
    public int height = 10;

    public GameObject floorPrefab;
    public GameObject wallPrefab;
    public GameObject playerPrefab;      
    public CameraFollow cameraFollow;

    

    void Start()
    {
        GenerateFixedRoom();
    }

    void GenerateFixedRoom()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 position = new Vector3(x, y, 0f);
                bool isEdge = (x == 0 || y == 0 || x == width - 1 || y == height - 1);

                GameObject prefabToPlace = isEdge ? wallPrefab : floorPrefab;
                Instantiate(prefabToPlace, position, Quaternion.identity);
            }
        }

        // Instanciar jugador en el centro de la sala
        Vector3 spawnPosition = new Vector3(width / 2f, height / 2f, 0f);
        GameObject player = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);

        //Punto de guardado
        SaveSystem.SaveGame();



        if (cameraFollow != null)
        {
            cameraFollow.target = player.transform;
        }

    }
}
