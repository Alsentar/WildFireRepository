using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatUI : MonoBehaviour
{
    public Button attackButton;
    public PlayerUnit playerUnit;

    void Start()
    {
        attackButton.onClick.AddListener(OnAttackButtonPressed);
        attackButton.interactable = false; // por defecto
    }

    void Update()
    {
        // Activa o desactiva botón según si el jugador puede actuar
        if (playerUnit != null)
        {
            attackButton.interactable = playerUnit.canAct;
        }
    }

    void OnAttackButtonPressed()
    {
        if (playerUnit != null && playerUnit.canAct)
        {
            playerUnit.canAct = false;
            playerUnit.combatManager.PlayerAttack();
        }
    }
}
