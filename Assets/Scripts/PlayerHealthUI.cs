using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerHealthUI : MonoBehaviour
{
    public Slider healthSlider;
    public CombatUnit playerUnit;

    void Update()
    {
        if (playerUnit != null && healthSlider != null)
        {
            healthSlider.value = playerUnit.currentHP;
        }
    }

    public void Initialize(CombatUnit unit)
    {
        playerUnit = unit;
        if (healthSlider != null)
        {
            healthSlider.maxValue = unit.maxHP;
            healthSlider.value = unit.currentHP;
        }

        Debug.Log("La barra del jugador muestra: " + playerUnit.currentHP);

    }
}
