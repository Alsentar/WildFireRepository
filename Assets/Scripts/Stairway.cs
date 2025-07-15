using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Stairway : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Jugador tocó escalera , Generando nuevo nivel");

            // Aumentar el contador de piso
            if (BattleLoader.Instance != null)
            {
                BattleLoader.Instance.currentFloor++;
                Debug.Log("Piso actual: " + BattleLoader.Instance.currentFloor);

                // Si llegamos al piso 5, cambiar de escena a zona de descanso
                if (BattleLoader.Instance.currentFloor >= 5)
                {
                    SceneManager.LoadScene("RestAreaOne");
                    return;
                }
            }


            DungeonGenerator generator = FindObjectOfType<DungeonGenerator>();
            if (generator != null)
            {
                generator.GenerateNewLevel();
            }
        }
    }
}
