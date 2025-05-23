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
    public List<DamageType> weaknesses;
    public List<DamageType> resistances;
    public List<Attack> availableAttacks = new List<Attack>();


    /**
    public enum DamageType
    {
        Physical,
        Fire,
        Ice,
        Lightning,
        Dark,
        Holy
    }

    **/



    public void TakeDamage(int baseDamage, DamageType type)
    {
        int finalDamage = baseDamage;

        if (weaknesses.Contains(type))
        {
            finalDamage *= 2;
            Debug.Log($"{unitName} es d�bil a {type}. �Da�o aumentado!");
        }
        else if (resistances.Contains(type))
        {
            finalDamage /= 2;
            Debug.Log($"{unitName} resiste {type}. Da�o reducido.");
        }

        finalDamage = Mathf.Max(1, finalDamage - defense);
        currentHP -= finalDamage;

        Debug.Log($"{unitName} recibi� {finalDamage} de da�o. Vida restante: {currentHP}");
    }


    public bool IsDead()
    {
        return currentHP <= 0;
    }
}
