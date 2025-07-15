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
        if (SaveSystem.SaveFileExists())
        {
            SaveSystem.LoadGame(); // Esto moverá al jugador a RestAreaOne
        }
        else
        {
            Debug.Log("No hay partida guardada.");
            
        }
    }


    public void QuitGame()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}
