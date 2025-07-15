using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BattleLoader : MonoBehaviour
{
    public static BattleLoader Instance;

    public GameObject enemyPrefab; //  PREFAB del enemigo
    public GameObject playerPrefab; //  PREFAB del jugador

    public string defeatedEnemyID;
    public SavedMapData savedMapData;
    public int currentFloor = 1;
    public int currentZone = 1;





    public List<CharacterData> party = new List<CharacterData>();

    public CharacterData GetCharacter(string id)
    {
        return party.Find(c => c.id == id);
    }





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

        if (party.Count == 0)
        {
            CharacterData kasai = new CharacterData
            {
                id = "Kasai",
                prefabName = "Prefabs/KasaiPrefab",  
                level = 1,
                maxHP = 100,
                currentHP = 100,
                attack = 10,
                defense = 2,
                speed = 3,
                currentXP = 0,
                xpToNextLevel = 100,
                baseAttackGrowth = 2,
                baseDefenseGrowth = 1,
                baseSpeedGrowth = 2
            };

            party.Add(kasai);
        }


    }
}
