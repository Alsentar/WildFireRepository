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
                int healAmount = 30;
                player.currentHP = Mathf.Min(player.maxHP, player.currentHP + healAmount);

                if (BattleLoader.Instance != null)
                {
                    BattleLoader.Instance.playerCurrentHP = player.currentHP;
                }

                Debug.Log("El jugador se curó al abrir el cofre. HP actual: " + player.currentHP);
            }

            //Destroy(gameObject); // opcional
        }
    }

}
