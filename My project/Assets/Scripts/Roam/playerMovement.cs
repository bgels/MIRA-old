using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class playerMovement : MonoBehaviour
{

    public float speed;
    public float sprintSpeed;
    private Vector2 moveDir;

    public Rigidbody2D rBody;

    public Animator animator;
    private bool movingNow;

    private float encounterTimer;

    public event Action OnEncountered;
    

    // Start is called before the first frame update
    void Start()
    {

        animator = gameObject.GetComponent<Animator>();
        speed = 5;
        sprintSpeed = 4;
        encounterTimer = 0;
    }

    // Update is called once per frame
    public void HandleUpdate()
    {

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
            speed = 5 + sprintSpeed;
            }
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
            speed = 5;
            }
            
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveY = Input.GetAxisRaw("Vertical");
            if (moveX != 0)
            {
                moveY = 0;
            }

            animator.SetFloat("mX", moveX);
            animator.SetFloat("mY", moveY);

            moveDir = new Vector2(moveX, moveY).normalized;
            if(moveDir != Vector2.zero)
            {
                movingNow = true;
            }
            else
            {
                movingNow = false;
            }
        animator.SetBool("isMoving", movingNow);

    }
    
    private void FixedUpdate()
    {

        move();

    }

    void move()
    {
        rBody.velocity = new Vector2(moveDir.x * speed, moveDir.y * speed);
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
