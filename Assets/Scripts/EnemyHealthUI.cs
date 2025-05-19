using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthUI : MonoBehaviour
{
    public Slider healthSlider;
    public CombatUnit enemyUnit;

    void Update()
    {
        if (enemyUnit != null && healthSlider != null)
        {
            healthSlider.value = enemyUnit.currentHP;
        }
    }

    public void Initialize(CombatUnit unit)
    {
        enemyUnit = unit;
        if (healthSlider != null)
        {
            healthSlider.maxValue = unit.maxHP;
            healthSlider.value = unit.currentHP;
        }
    }
}
