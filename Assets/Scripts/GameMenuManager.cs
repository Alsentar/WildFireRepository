using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenuManager : MonoBehaviour
{
    public GameObject gameMenuUI;
    private bool isMenuOpen = false;

    void Start()
    {
        if (gameMenuUI != null)
            gameMenuUI.SetActive(false); // Ocultar al iniciar
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenu();
        }
    }

    public void ToggleMenu()
    {
        isMenuOpen = !isMenuOpen;
        gameMenuUI.SetActive(isMenuOpen);
        Time.timeScale = isMenuOpen ? 0f : 1f;
    }

    public void OpenParty()
    {
        Time.timeScale = 1f;
        //SceneManager.LoadScene("PartyMenu");
    }

    public void OpenInventory()
    {
        Time.timeScale = 1f;
        //SceneManager.LoadScene("InventoryMenu");
    }

    public void OpenSettings()
    {
        // Futuro: Abrir submenú de ajustes
        Debug.Log("Ajustes aún no implementado.");
    }

    public void ReturnToMainMenu()
    {
        Debug.Log("Returnando al menu...");
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
