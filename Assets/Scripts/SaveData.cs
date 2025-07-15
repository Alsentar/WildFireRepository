using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public int currentFloor;
    public string lastScene;

    // Solo guardaremos el primer personaje de la party (por ahora)
    public CharacterData kasaiData;
}
