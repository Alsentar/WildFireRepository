using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CombatUnit : MonoBehaviour
{
    public string unitName;
    public int maxHP;
    public int currentHP;
    public int attack;
    public int defense;
    public int speed;

    public void TakeDamage(int damage)
    {
        int finalDamage = Mathf.Max(1, damage - defense);
        currentHP -= finalDamage;
        Debug.Log($"{unitName} took {finalDamage} damage! Remaining HP: {currentHP}");
    }

    public bool IsDead()
    {
        return currentHP <= 0;
    }
}
