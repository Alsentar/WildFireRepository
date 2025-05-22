using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1f;

    private PlayerControls playerControls;
    private Vector2 movement;
    private Rigidbody2D rb;
    private Animator myAnimator;
    private SpriteRenderer mySpriteRender;
    private Vector2 lastDirection = Vector2.down; // Por defecto, mirando hacia abajo


    private void Awake()
    {
        playerControls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        mySpriteRender = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void Update()
    {
        PlayerInput();
    }

    private void FixedUpdate()
    {
        //AdjustPlayerFacingDirection();
        Move();
    }

    private void PlayerInput()
    {
        movement = playerControls.Movement.Move.ReadValue<Vector2>();

        // Determinar dirección cardinal dominante
        if (movement != Vector2.zero)
        {
            // Prioriza eje dominante
            if (Mathf.Abs(movement.x) > Mathf.Abs(movement.y))
            {
                if (movement.x > 0)
                    myAnimator.SetInteger("direction", 2); // Right
                else
                    myAnimator.SetInteger("direction", 1); // Left
            }
            else
            {
                if (movement.y > 0)
                    myAnimator.SetInteger("direction", 3); // Up
                else
                    myAnimator.SetInteger("direction", 0); // Down
            }
        }

        myAnimator.SetBool("isMoving", movement != Vector2.zero);
    }



    private void Move()
    {
        rb.MovePosition(rb.position + movement * (moveSpeed * Time.fixedDeltaTime));
    }

    private void AdjustPlayerFacingDirection()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 playerScreenPoint = Camera.main.WorldToScreenPoint(transform.position);

        if (mousePos.x < playerScreenPoint.x)
        {
            mySpriteRender.flipX = true;
        }
        else
        {
            mySpriteRender.flipX = false;
        }
    }

}
