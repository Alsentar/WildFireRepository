using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PausePartyPanel : MonoBehaviour
{
    [Header("Panel")]
    public GameObject partyPanel;

    [Header("Retrato")]
    public Image portraitImage;
    public Sprite kasaiPortrait;

    [Header("Textos de stats")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI defenseText;
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI xpText;

    private void Start()
    {
        if (partyPanel != null)
            partyPanel.SetActive(false);
    }

    //  Toggle: si está abierto lo cierra, si está cerrado lo abre
    public void TogglePartyPanel()
    {
        if (partyPanel == null)
        {
            Debug.LogWarning("[PausePartyPanel] partyPanel es null, revisa la referencia en el inspector.");
            return;
        }

        bool newState = !partyPanel.activeSelf;
        partyPanel.SetActive(newState);

        if (newState)
        {
            RefreshStats();
        }
    }

    // Para forzar que se cierre cuando se cierre el menú de pausa con ESC
    public void ForceClose()
    {
        if (partyPanel != null)
            partyPanel.SetActive(false);
    }

    private void RefreshStats()
    {
        if (BattleLoader.Instance == null)
        {
            Debug.LogWarning("[PausePartyPanel] BattleLoader.Instance es null.");
            return;
        }

        CharacterData kasai = BattleLoader.Instance.GetCharacter("Kasai");
        if (kasai == null)
        {
            Debug.LogWarning("[PausePartyPanel] No se encontró a Kasai en la party.");
            return;
        }

        if (portraitImage != null && kasaiPortrait != null)
            portraitImage.sprite = kasaiPortrait;

        if (nameText != null)
            nameText.text = kasai.id;

        if (levelText != null)
            levelText.text = $"Nivel {kasai.level}";

        if (hpText != null)
            hpText.text = $"HP: {kasai.currentHP} / {kasai.maxHP}";

        if (attackText != null)
            attackText.text = $"ATQ: {kasai.attack}";

        if (defenseText != null)
            defenseText.text = $"DEF: {kasai.defense}";

        if (speedText != null)
            speedText.text = $"VEL: {kasai.speed}";

        if (xpText != null)
            xpText.text = $"XP: {kasai.currentXP} / {kasai.xpToNextLevel}";
    }
}
