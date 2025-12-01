using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CombatUI : MonoBehaviour
{
    public Button attackButton;
    

    public GameObject attackOptionsPanel;
    public GameObject attackButtonPrefab; // Prefab de botón reutilizable
    public Transform buttonContainer;     // El panel padre donde instancias los botones

    public PlayerUnit playerUnit;
    public CombatManager combatManager;

    public Sprite fireButtonSprite;
    public Sprite iceButtonSprite;
    public Sprite physicalButtonSprite;
    public Sprite holyButtonSprite;

    void Start()
    {
        attackButton.onClick.AddListener(ShowAttackOptions);
        attackOptionsPanel.SetActive(false);
    }

    void ShowAttackOptions()
    {
        attackOptionsPanel.SetActive(true);

        // Limpiar botones anteriores
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }

        // Crear botones para cada ataque
        foreach (Attack atk in playerUnit.availableAttacks)
        {
            GameObject btn = Instantiate(attackButtonPrefab, buttonContainer);
            Debug.Log($"Instanciado botón para: {atk.name}");
            btn.GetComponentInChildren<TextMeshProUGUI>().text = atk.name;

            Image btnImage = btn.GetComponent<Image>();
            switch (atk.type)
            {
                case DamageType.Fire:
                    btnImage.sprite = fireButtonSprite;
                    break;
                case DamageType.Ice:
                    btnImage.sprite = iceButtonSprite;
                    break;
                case DamageType.Physical:
                    btnImage.sprite = physicalButtonSprite;
                    break;
                case DamageType.Holy:
                    btnImage.sprite = holyButtonSprite;
                    break;
                default:
                    Debug.LogWarning("Tipo de daño no soportado: " + atk.type);
                    break;
            }


            btn.GetComponent<Button>().onClick.AddListener(() => SelectAttack(atk));
        }
    }

    void SelectAttack(Attack atk)
    {
        attackOptionsPanel.SetActive(false);
        
        combatManager.PlayerAttack(atk);
    }

}
