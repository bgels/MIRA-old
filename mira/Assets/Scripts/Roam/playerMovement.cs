using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class playerMovement : MonoBehaviour
{

    public float speed;
    public float sprintSpeed;
    private Vector2 moveDir;

    public LayerMask interactableLayer;

    public Rigidbody2D rBody;

    public Animator animator;
    private bool movingNow;

    private float encounterTimer;

    public event Action OnEncountered;

    public



    // Start is called before the first frame update
    void Start()
    {

        animator = gameObject.GetComponent<Animator>();
        speed = 5;
        sprintSpeed = 3;
        encounterTimer = 0;
    }

    // Update is called once per frame
    public void HandleUpdate()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        if (moveX != 0)
        {
            moveY = 0; // Prioritize horizontal movement
        }

        if (moveX != 0 || moveY != 0)
        {
            animator.SetFloat("mX", moveX);
            animator.SetFloat("mY", moveY);
            moveDir = new Vector2(moveX, moveY).normalized;
            movingNow = true;
        }
        else
        {
            movingNow = false;
            moveDir = Vector2.zero; // Reset moveDir to stop movement
        }

        animator.SetBool("isMoving", movingNow);

        // If not moving, keep the last movement direction
        if (!movingNow)
        {
            animator.SetFloat("lastMX", animator.GetFloat("mX"));
            animator.SetFloat("lastMY", animator.GetFloat("mY"));
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Interact();
        }
    }

    void Interact()
    {
        var facingDir = new Vector3(animator.GetFloat("mX"), animator.GetFloat("mY"));
        var interactPos = transform.position += facingDir;

        //Debug.DrawLine(transform.position, interactPos, Color.green, .5f);
        var collider = Physics2D.OverlapCircle(interactPos, .3f, interactableLayer);
        if(collider!= null)
        {
            Debug.Log("Npc?");
        }

    }

    private void FixedUpdate()
    {

        move();

    }

    void move()
    {
        float currentSpeed = speed;
        if(Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed += sprintSpeed;
        }
        rBody.velocity = new Vector2(moveDir.x * currentSpeed, moveDir.y * currentSpeed);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        
        if(collision.tag == "encounterTiles")
        {
            encounterTimer += Time.deltaTime;
            if (UnityEngine.Random.Range(1,101) <= 3 && encounterTimer >= 4)
            {
                print("ENCOUNTER!");
                encounterTimer = 0;
                animator.SetBool("isMoving", false);
                moveDir = new Vector2(0, 0).normalized;
                OnEncountered();
            }
        }
    }   

}   
