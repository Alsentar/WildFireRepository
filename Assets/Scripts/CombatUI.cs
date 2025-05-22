using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CombatUI : MonoBehaviour
{
    public Button attackButton;
    //public PlayerUnit playerUnit;

    public GameObject attackOptionsPanel;
    public GameObject attackButtonPrefab; // Prefab de botón reutilizable
    public Transform buttonContainer;     // El panel padre donde instancias los botones

    public PlayerUnit playerUnit;
    public CombatManager combatManager;

    void Start()
    {
        attackButton.onClick.AddListener(ShowAttackOptions);
        attackOptionsPanel.SetActive(false);
    }

    void ShowAttackOptions()
    {
        attackOptionsPanel.SetActive(true);

        // Limpiar botones anteriores
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }

        // Crear botones para cada ataque
        foreach (Attack atk in playerUnit.availableAttacks)
        {
            GameObject btn = Instantiate(attackButtonPrefab, buttonContainer);
            Debug.Log($"Instanciado botón para: {atk.name}");
            btn.GetComponentInChildren<TextMeshProUGUI>().text = atk.name;
            btn.GetComponent<Button>().onClick.AddListener(() => SelectAttack(atk));
        }
    }

    void SelectAttack(Attack atk)
    {
        attackOptionsPanel.SetActive(false);
        //playerUnit.canAct = false;
        combatManager.PlayerAttack(atk);
    }

}
