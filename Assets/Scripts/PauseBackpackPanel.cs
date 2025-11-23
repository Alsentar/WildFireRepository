using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PauseBackpackPanel : MonoBehaviour
{
    [Header("Panel")]
    public GameObject backpackPanel;

    [Header("Item")]
    public Image itemImage;
    public Sprite swordSprite;

    [Header("Texto")]
    public TextMeshProUGUI itemDescriptionText;

    private void Start()
    {
        if (backpackPanel != null)
            backpackPanel.SetActive(false);
    }

    //  Toggle: abre/cierra el panel
    public void ToggleBackpackPanel()
    {
        if (backpackPanel == null)
        {
            Debug.LogWarning("[PauseBackpackPanel] backpackPanel es null, revisa la referencia en el inspector.");
            return;
        }

        bool newState = !backpackPanel.activeSelf;
        backpackPanel.SetActive(newState);

        if (newState)
        {
            RefreshBackpack();
        }
    }

    // Forzar cierre cuando se cierra el menú de pausa
    public void ForceClose()
    {
        if (backpackPanel != null)
            backpackPanel.SetActive(false);
    }

    private void RefreshBackpack()
    {
        // Por ahora, contenido estático.
        
    }
}
