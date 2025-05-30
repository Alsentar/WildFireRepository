using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1f;

    public bool allowManualControl = true;


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
        if (!allowManualControl)
        {
          
            return;
        }

        movement = playerControls.Movement.Move.ReadValue<Vector2>();

        // Dirección
        if (movement != Vector2.zero)
        {
            if (Mathf.Abs(movement.x) > Mathf.Abs(movement.y))
            {
                myAnimator.SetInteger("direction", movement.x > 0 ? 2 : 1);
            }
            else
            {
                myAnimator.SetInteger("direction", movement.y > 0 ? 3 : 0);
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
