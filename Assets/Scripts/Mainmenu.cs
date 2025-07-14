using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void NewGame()
    {
        // Borrar datos anteriores
        //BattleLoader.Instance.savedMapData = null;
        SceneManager.LoadScene("WildFireBeta");  
    }

    public void LoadGame()
    {
        
        if (BattleLoader.Instance.savedMapData != null)
        {
            SceneManager.LoadScene("WildFireBeta");
        }
        else
        {
            Debug.LogWarning("No hay partida guardada.");
            
        }
    }

    public void QuitGame()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}
