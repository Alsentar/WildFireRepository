using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyIdentifier : MonoBehaviour
{
    public string enemyID; // �nico por enemigo

    void Awake()
    {
        if (string.IsNullOrEmpty(enemyID))
        {
            enemyID = System.Guid.NewGuid().ToString(); // Asigna un ID �nico si no tiene uno
        }
    }
}
