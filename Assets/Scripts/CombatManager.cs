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

        CharacterData kasaiData = BattleLoader.Instance.GetCharacter("Kasai");

        if (kasaiData == null)
        {
            Debug.LogError("No se encontró a Kasai en la party.");
            return;
        }

        // Cargar prefab desde Resources
        GameObject kasaiPrefab = Resources.Load<GameObject>(kasaiData.prefabName);
        if (kasaiPrefab == null)
        {
            Debug.LogError("No se pudo cargar el prefab: " + kasaiData.prefabName);
            return;
        }

        GameObject playerObj = Instantiate(kasaiPrefab, playerSpawn.position, Quaternion.identity);
        playerUnit = playerObj.GetComponent<PlayerUnit>();

        // Asignar los stats desde CharacterData
        playerUnit.unitName = kasaiData.id;
        playerUnit.level = kasaiData.level;
        playerUnit.currentHP = kasaiData.currentHP;
        playerUnit.maxHP = kasaiData.maxHP;
        playerUnit.attack = kasaiData.attack;
        playerUnit.defense = kasaiData.defense;
        playerUnit.speed = kasaiData.speed;
        playerUnit.currentXP = kasaiData.currentXP;
        playerUnit.xpToNextLevel = kasaiData.xpToNextLevel;
        playerUnit.baseAttackGrowth = kasaiData.baseAttackGrowth;
        playerUnit.baseDefenseGrowth = kasaiData.baseDefenseGrowth;
        playerUnit.baseSpeedGrowth = kasaiData.baseSpeedGrowth;

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
            Debug.Log("Turno del jugador.");
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
        int totalPower = selectedAttack.power + playerUnit.attack; // Combinar ataque base + stat
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

            


            int xpEarned = 20 + enemyUnit.level * 10; // Fórmula simple de XP
            playerUnit.GainXP(xpEarned);

            CharacterData kasai = BattleLoader.Instance.GetCharacter("Kasai");
            if (kasai != null)
            {
                kasai.currentHP = playerUnit.currentHP;
                kasai.level = playerUnit.level;
                kasai.currentXP = playerUnit.currentXP;
                kasai.xpToNextLevel = playerUnit.xpToNextLevel;
                kasai.attack = playerUnit.attack;
                kasai.defense = playerUnit.defense;
                kasai.speed = playerUnit.speed;
            }

            Debug.Log($"Jugador ganó {xpEarned} XP.");

            
            BattleLoader.Instance.eliminatedEnemies.Add(BattleLoader.Instance.defeatedEnemyID);

            // GANAR XP
            

            Debug.Log("Guardando vida actual del jugador: " + playerUnit.currentHP);

            SceneManager.LoadScene("WildFireBeta");
        }
    }


}
