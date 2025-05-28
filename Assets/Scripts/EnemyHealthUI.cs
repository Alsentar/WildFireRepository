using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyHealthUI : MonoBehaviour
{
    public Slider healthSlider;
    public CombatUnit enemyUnit;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI levelText; // NUEVO

    void Update()
    {
        if (enemyUnit != null && healthSlider != null)
        {
            healthSlider.value = enemyUnit.currentHP;
        }

        if (enemyUnit != null && levelText != null)
        {
            levelText.text = "Nv " + enemyUnit.level;
        }
    }

    public void Initialize(CombatUnit unit)
    {
        if (nameText != null)
        {
            nameText.text = unit.unitName;
        }

        if (levelText != null)
        {
            levelText.text = "Nv " + unit.level;
        }

        enemyUnit = unit;

        if (healthSlider != null)
        {
            healthSlider.maxValue = unit.maxHP;
            healthSlider.value = unit.currentHP;
        }
    }
}
