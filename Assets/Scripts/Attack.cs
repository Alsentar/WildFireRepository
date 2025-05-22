using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum DamageType
{
    Physical,
    Fire,
    Ice,
    Lightning,
    Dark,
    Holy
}

[System.Serializable]
public class Attack
{
    public string name;
    public int power;
    public DamageType type;
    public int manaCost;

    public Attack(string name, int power, DamageType type, int manaCost = 0)
    {
        this.name = name;
        this.power = power;
        this.type = type;
        this.manaCost = manaCost;
    }

    public override string ToString()
    {
        return $"{name} ({type}) - Power: {power}, Mana: {manaCost}";
    }
}

