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
        }
    }
}
