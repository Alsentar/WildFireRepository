using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class EnemyTrigger : MonoBehaviour
{
    public GameObject enemyPrefab; // Asignado desde DungeonGenerator
    

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Guardar el prefab del enemigo
            BattleLoader.Instance.enemyPrefab = enemyPrefab;

            // Guardar el prefab del jugador desde DungeonGenerator
            BattleLoader.Instance.playerPrefab = FindObjectOfType<DungeonGenerator>().playerPrefab;

            EnemyIdentifier id = GetComponent<EnemyIdentifier>();
            if (id != null)
            {
                BattleLoader.Instance.defeatedEnemyID = id.enemyID;
            }

            FindObjectOfType<DungeonGenerator>().SaveCurrentMap();
            UnityEngine.SceneManagement.SceneManager.LoadScene("CombatTest");
        }
    }





}
