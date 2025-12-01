using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class RetryButton : MonoBehaviour
{
    public string sceneToLoad = "WildFireBeta";
    public string mainMenuScene = "Mainmenu";

    public void RetryGame()
    {
        if (BattleLoader.Instance != null)
        {
            BattleLoader.Instance.eliminatedEnemies.Clear();
            BattleLoader.Instance.savedMapData = null;
            BattleLoader.Instance.enemyPrefab = null;

            // Restaurar el estado base de cada futuro personaje de la party
            foreach (CharacterData character in BattleLoader.Instance.party)
            {
                character.currentHP = character.maxHP;
                character.currentXP = 0;
                character.level = 1;
                character.xpToNextLevel = 100;

                character.attack = 10 + (character.baseAttackGrowth * (character.level - 1));
                character.defense = 2 + (character.baseDefenseGrowth * (character.level - 1));
                character.speed = 3 + (character.baseSpeedGrowth * (character.level - 1));
            }
        }

        SceneManager.LoadScene(sceneToLoad);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }


}
