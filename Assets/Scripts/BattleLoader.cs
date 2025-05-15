using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BattleLoader : MonoBehaviour
{
    public static BattleLoader Instance;

    public GameObject enemyPrefab; //  PREFAB del enemigo
    public GameObject playerPrefab; //  PREFAB del jugador

    public string defeatedEnemyID;

    public HashSet<string> eliminatedEnemies = new HashSet<string>();



    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
