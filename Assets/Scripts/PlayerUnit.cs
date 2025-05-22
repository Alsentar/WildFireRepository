using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : CombatUnit
{
    public bool canAct = false;
    public CombatManager combatManager;

    public List<Attack> availableAttacks = new List<Attack>();

    /**

    void Update()
    {
        if (canAct && Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Jugador ataca con SPACE");
            canAct = false;
            //combatManager.PlayerAttack();
        }
    }

    **/
}
