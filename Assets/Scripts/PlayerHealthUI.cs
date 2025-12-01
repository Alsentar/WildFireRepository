using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealthUI : MonoBehaviour
{
    public Slider healthSlider;
    public TextMeshProUGUI levelText; 
    public CombatUnit playerUnit;

    void Update()
    {
        if (playerUnit != null && healthSlider != null)
        {
            healthSlider.value = playerUnit.currentHP;
        }

        if (playerUnit != null && levelText != null)
        {
            levelText.text = "Nv " + playerUnit.level;
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

        if (levelText != null)
        {
            levelText.text = "Nv " + unit.level;
        }

        Debug.Log("La barra del jugador muestra: " + playerUnit.currentHP);
    }
}
