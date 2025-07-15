using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public static class SaveSystem
{
    private static string savePath = Application.persistentDataPath + "/save.json";

    public static void SaveGame()
    {
        SaveData data = new SaveData();

        data.currentFloor = BattleLoader.Instance.currentFloor;
        data.lastScene = "RestAreaOne"; // podemos cambiar esto si hay más zonas luego
        data.kasaiData = BattleLoader.Instance.GetCharacter("Kasai");

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);

        Debug.Log("Juego guardado en: " + savePath);
    }

    public static bool SaveFileExists()
    {
        return File.Exists(savePath);
    }

    public static void LoadGame()
    {
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("No se encontró archivo de guardado.");
            return;
        }

        string json = File.ReadAllText(savePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        // Restaurar estado en BattleLoader
        BattleLoader.Instance.currentFloor = data.currentFloor;

        // Restaurar datos del personaje principal
        CharacterData kasai = BattleLoader.Instance.GetCharacter("Kasai");
        if (kasai != null && data.kasaiData != null)
        {
            kasai.level = data.kasaiData.level;
            kasai.currentHP = data.kasaiData.currentHP;
            kasai.maxHP = data.kasaiData.maxHP;
            kasai.attack = data.kasaiData.attack;
            kasai.defense = data.kasaiData.defense;
            kasai.speed = data.kasaiData.speed;
            kasai.currentXP = data.kasaiData.currentXP;
            kasai.xpToNextLevel = data.kasaiData.xpToNextLevel;
        }

        // Cargar escena desde la que guardó
        UnityEngine.SceneManagement.SceneManager.LoadScene(data.lastScene);
    }
}

