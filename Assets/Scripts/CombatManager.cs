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

        if (BattleLoader.Instance != null)
        {
            if (BattleLoader.Instance.playerCurrentHP >= 0)
            {
                playerUnit.currentHP = BattleLoader.Instance.playerCurrentHP;
                Debug.Log("Se restauró la vida desde BattleLoader: " + playerUnit.currentHP);
            }
            else
            {
                playerUnit.currentHP = playerUnit.maxHP;
                BattleLoader.Instance.playerCurrentHP = playerUnit.maxHP;
                Debug.Log("Primer combate, vida a full: " + playerUnit.currentHP);
            }


            Debug.Log("Vida restaurada del jugador: " + playerUnit.currentHP);
        }



        enemyUnit = enemyObj.GetComponent<CombatUnit>();

        // UI: vincular barra de vida
        EnemyHealthUI healthUI = FindObjectOfType<EnemyHealthUI>();
        if (healthUI != null)
        {
            healthUI.Initialize(enemyUnit);
        }

        
        PlayerHealthUI playerUI = FindObjectOfType<PlayerHealthUI>();
        if (playerUI != null)
        {
            playerUI.Initialize(playerUnit);
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

    public void PlayerAttack(Attack selectedAttack)
    {
        Debug.Log($"El jugador usó {selectedAttack.name}");

        enemyUnit.TakeDamage(selectedAttack.power, selectedAttack.type);
        CheckBattleOutcome();
        isPlayerTurn = false;
        StartTurn();
    }


    void EnemyAttack()
    {
        if (enemyUnit.availableAttacks == null || enemyUnit.availableAttacks.Count == 0)
        {
            Debug.LogWarning($"{enemyUnit.unitName} no tiene ataques configurados. Usando ataque genérico.");
            playerUnit.TakeDamage(enemyUnit.attack, DamageType.Physical); // fallback
        }
        else
        {
            // Elegir un ataque al azar
            Attack chosenAttack = enemyUnit.availableAttacks[Random.Range(0, enemyUnit.availableAttacks.Count)];

            Debug.Log($"{enemyUnit.unitName} usó {chosenAttack.name} contra {playerUnit.unitName}");
            playerUnit.TakeDamage(chosenAttack.power, chosenAttack.type);
        }

        CheckBattleOutcome();
        isPlayerTurn = true;
        StartTurn();
    }


    void CheckBattleOutcome()
    {
        if (playerUnit.currentHP <= 0)
        {
            Debug.Log("El jugador ha muerto. Cargando escena de derrota...");
            SceneManager.LoadScene("GameOverScene");
            return;
        }

        if (enemyUnit.currentHP <= 0)
        {
            Debug.Log("El enemigo ha sido derrotado.");

            if (BattleLoader.Instance != null)
            {
                BattleLoader.Instance.playerCurrentHP = playerUnit.currentHP;
            }
            BattleLoader.Instance.eliminatedEnemies.Add(BattleLoader.Instance.defeatedEnemyID);


            Debug.Log("Guardando vida actual del jugador: " + playerUnit.currentHP);
            SceneManager.LoadScene("WildFireBeta");
        }
    }


}
