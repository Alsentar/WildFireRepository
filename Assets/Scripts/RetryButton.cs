using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class RetryButton : MonoBehaviour
{
    public string sceneToLoad = "WildFireBeta";

    public void RetryGame()
    {
        // Limpiar datos persistentes del combate anterior
        if (BattleLoader.Instance != null)
        {
            BattleLoader.Instance.eliminatedEnemies.Clear();
            BattleLoader.Instance.savedMapData = null;
            BattleLoader.Instance.playerCurrentHP = -1;
            BattleLoader.Instance.playerPrefab = null;
            BattleLoader.Instance.enemyPrefab = null;

        }

        


        // Cargar la escena principal
        SceneManager.LoadScene(sceneToLoad);
    }
}
