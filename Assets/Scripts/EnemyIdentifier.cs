using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyIdentifier : MonoBehaviour
{
    public string enemyID; // único por enemigo

    void Awake()
    {
        if (string.IsNullOrEmpty(enemyID))
        {
            enemyID = System.Guid.NewGuid().ToString(); // Asigna un ID único si no tiene uno
        }
    }
}
