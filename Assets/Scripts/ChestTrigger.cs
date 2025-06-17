using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ChestTrigger : MonoBehaviour
{
    private Animator animator;
    private bool isOpened = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isOpened && collision.CompareTag("Player"))
        {
            animator.SetTrigger("Open");
            isOpened = true;

            PlayerUnit player = collision.GetComponent<PlayerUnit>();
            if (player != null)
            {
                CharacterData kasaiData = BattleLoader.Instance.GetCharacter("Kasai");

                if (kasaiData != null)
                {

                    Debug.Log("Antes de curarse el jugador tenia una vida de: " + kasaiData.currentHP);
                    int healAmount = 30;
                    kasaiData.currentHP = Mathf.Min(kasaiData.maxHP, kasaiData.currentHP + healAmount);

                    //actualizamos al jugador instanciado también
                    player.currentHP = kasaiData.currentHP;

                    Debug.Log("El jugador se curó al abrir el cofre. HP actual: " + player.currentHP);

                }

                
            }

            //Destroy(gameObject); // opcional
        }
    }


}
