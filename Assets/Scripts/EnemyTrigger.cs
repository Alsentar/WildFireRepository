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
        BattleLoader.Instance.enemyPrefab = enemyPrefab;

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

        SceneManager.LoadScene("CombatTest");
    }
}
