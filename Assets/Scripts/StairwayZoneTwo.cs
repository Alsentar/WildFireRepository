using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StairwayZoneTwo : MonoBehaviour
{
    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasTriggered) return;

        if (collision.CompareTag("Player"))
        {
            hasTriggered = true;

            // Aumentar piso actual
            if (BattleLoader.Instance != null)
            {
                BattleLoader.Instance.currentFloor++;
                Debug.Log("Descendiendo al piso " + BattleLoader.Instance.currentFloor);
            }

            // Recargar la escena de ZoneTwo para generar nuevo piso
            SceneManager.LoadScene("ZoneTwo");
        }
    }
}
