using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterData
{
    public string id;               // Identificador unico para cada personaje
    public string prefabName;      // Nombre del prefab en Resources
    public int level = 1;
    public int currentHP;
    public int maxHP;
    public int attack;
    public int defense;
    public int speed;
    public int currentXP = 0;
    public int xpToNextLevel = 100;

    public int baseAttackGrowth = 2;
    public int baseDefenseGrowth = 1;
    public int baseSpeedGrowth = 2;

    public void ApplyLevelUp()
    {
        level++;
        xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * 1.2f);
        attack += baseAttackGrowth;
        defense += baseDefenseGrowth;
        speed += baseSpeedGrowth;
        Debug.Log($"{id} subió a nivel {level}! ATK: {attack}, DEF: {defense}, SPD: {speed}");
    }

    public void RecalculateStats()
    {
        attack = baseAttackGrowth * (level - 1) + 10;
        defense = baseDefenseGrowth * (level - 1) + 2;
        speed = baseSpeedGrowth * (level - 1) + 3;
    }
}
