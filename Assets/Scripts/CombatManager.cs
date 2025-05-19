using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class CombatManager : MonoBehaviour
{
    public Transform playerSpawn;
    public Transform enemySpawn;

    private PlayerUnit playerUnit;
    private CombatUnit enemyUnit;

    private bool isPlayerTurn = true;

    void Start()
    {
        if (BattleLoader.Instance.enemyPrefab == null || BattleLoader.Instance.playerPrefab == null)
        {
            Debug.LogError("Prefabs no definidos en BattleLoader.");
            return;
        }

        GameObject playerObj = Instantiate(BattleLoader.Instance.playerPrefab, playerSpawn.position, Quaternion.identity);
        GameObject enemyObj = Instantiate(BattleLoader.Instance.enemyPrefab, enemySpawn.position, Quaternion.identity);

        // Aumentar tamaño solo para combate
        playerObj.transform.localScale = new Vector3(9f, 8f, 1f); 
        enemyObj.transform.localScale = new Vector3(9f, 8f, 1f);

        playerUnit = playerObj.GetComponent<PlayerUnit>();
        enemyUnit = enemyObj.GetComponent<CombatUnit>();

        // UI: vincular barra de vida
        EnemyHealthUI healthUI = FindObjectOfType<EnemyHealthUI>();
        if (healthUI != null)
        {
            healthUI.Initialize(enemyUnit);
        }

        playerUnit.combatManager = this;

        CombatUI ui = FindObjectOfType<CombatUI>();
        if (ui != null)
        {
            ui.playerUnit = playerUnit;
        }


        StartTurn();
    }


    void StartTurn()
    {
        if (isPlayerTurn)
        {
            Debug.Log("Turno del jugador. Presiona ESPACIO para atacar.");
            playerUnit.canAct = true;
        }
        else
        {
            Debug.Log("Turno del enemigo.");
            Invoke(nameof(EnemyAttack), 2f); // ataque automático con retraso
        }
    }

    public void PlayerAttack()
    {
        enemyUnit.TakeDamage(playerUnit.attack);
        CheckBattleOutcome();
        isPlayerTurn = false;
        StartTurn(); // pasa al enemigo
    }

    void EnemyAttack()
    {
        playerUnit.TakeDamage(enemyUnit.attack);
        CheckBattleOutcome();
        isPlayerTurn = true;
        StartTurn();
    }

    void CheckBattleOutcome()
    {
        if (enemyUnit.IsDead())
        {
            Debug.Log("¡Has ganado!");
            BattleLoader.Instance.eliminatedEnemies.Add(BattleLoader.Instance.defeatedEnemyID);
            SceneManager.LoadScene("WildFireBeta");
        }
        else if (playerUnit.IsDead())
        {
            Debug.Log("Has sido derrotado...");
            SceneManager.LoadScene("WildFireBeta"); // o pantalla de derrota
        }
    }

}
