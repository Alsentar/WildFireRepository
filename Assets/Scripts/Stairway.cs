using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stairway : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Jugador tocó escalera , Generando nuevo nivel");

            DungeonGenerator generator = FindObjectOfType<DungeonGenerator>();
            if (generator != null)
            {
                generator.GenerateNewLevel();
            }
        }
    }
}
