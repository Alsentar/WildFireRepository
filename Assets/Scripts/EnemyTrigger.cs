using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyTrigger : MonoBehaviour
{
    public GameObject enemyPrefab;

    private void OnTriggerEnter2D(Collider2D other)
    {

        Debug.Log($"Colisión detectada con: {other.name}");

        if (!other.CompareTag("Player")) return;

        // Guardar el prefab del enemigo
        Debug.Log($"[EnemyTrigger] Asignando enemigo a BattleLoader: {enemyPrefab?.name ?? "NULL"} ({enemyPrefab?.GetInstanceID()})");
        BattleLoader.Instance.enemyPrefab = enemyPrefab;
        Debug.Log($"[EnemyTrigger] Después de asignar -> BattleLoader.enemyPrefab: {BattleLoader.Instance.enemyPrefab?.name ?? "NULL"} ({BattleLoader.Instance.enemyPrefab?.GetInstanceID()})");


        // Buscar cualquier generador activo que implemente IZoneGenerator
        IZoneGenerator generator = FindObjectOfType<MonoBehaviour>() as IZoneGenerator;
        foreach (var mono in FindObjectsOfType<MonoBehaviour>())
        {
            if (mono is IZoneGenerator gen)
            {
                generator = gen;
                break;
            }
        }

        if (generator != null)
        {
            BattleLoader.Instance.playerPrefab = generator.playerPrefab;
            generator.SaveCurrentMap();
        }
        else
        {
            Debug.LogWarning("No se encontró ningún generador de zona que implemente IZoneGenerator.");
        }

        // Guardar ID del enemigo
        EnemyIdentifier id = GetComponent<EnemyIdentifier>();
        if (id != null)
            BattleLoader.Instance.defeatedEnemyID = id.enemyID;

        // Guardar HP del jugador si quieres
        PlayerUnit player = FindObjectOfType<PlayerUnit>();
        if (player != null)
        {
            // BattleLoader.Instance.playerCurrentHP = player.currentHP;
        }

        Debug.Log($"[BattleLoader] Preparando combate con: Enemy={enemyPrefab?.name ?? "NULL"} ({enemyPrefab?.GetInstanceID()})");


        SceneManager.LoadScene("CombatTest");
    }
}
