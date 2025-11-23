using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CombatBackgroundToggle : MonoBehaviour
{
    public GameObject FondoUno;
    public GameObject FondoDos;

    void Start()
    {
        if (BattleLoader.Instance == null)
        {
            Debug.LogWarning("BattleLoader is null, cannot set background.");
            return;
        }

        if (BattleLoader.Instance.currentZone == 1)
        {
            FondoUno.SetActive(true);
            FondoDos.SetActive(false);
        }
        else if (BattleLoader.Instance.currentZone == 2)
        {
            FondoUno.SetActive(false);
            FondoDos.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Invalid zoneIndex in battle loader.");
        }
    }
}
