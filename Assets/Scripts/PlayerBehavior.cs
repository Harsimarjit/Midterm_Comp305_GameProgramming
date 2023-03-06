using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    [Header("Player Movemnet Properties")]
    public float horizontalForce;
    public float maxSpeed;
    public float verticalForce;
    public float airFactor;
    public Transform groundPoint;
    public float groundRadius;
    public LayerMask groundLayerMask;
    public bool isGrounded;
    public bool isDucking;
    public PlayerAnimationState animationState;

    private Rigidbody2D rigidbody2D;
    private Animator animator;

    public static bool chestTrigger = false;
    private bool onceTrigger = true;
    public GameObject OpenChestObj;
    public GameObject CloseChestObj;
    public GameObject starObj;
    public Transform starSpawnPoint;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        float z = Input.GetAxis("Vertical");
        Duck(z);

        OpenChest();
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundPoint.position, groundRadius, groundLayerMask);

        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxis("Jump");

        Flip(x);
        Move(x);
        Jump(y);
        AirCheck();
    }
    private void OpenChest()
    {
        if (Input.GetKeyDown(KeyCode.E) && chestTrigger && onceTrigger)
        {
            OpenChestObj.SetActive(true);
            CloseChestObj.SetActive(false);
            StartCoroutine("SpawnStar", 0.1f);
            onceTrigger= false;
        }
    }
    IEnumerator SpawnStar(float waitTime)
    {
        while (true)
        {
            Instantiate(starObj, starSpawnPoint);
            starObj.GetComponent<Rigidbody2D>().AddForce(Vector2.up * verticalForce * 2f, ForceMode2D.Impulse);
            yield return new WaitForSeconds(waitTime);
        }
    }
    private void Duck(float z)
    {
        if (z != 0f)
            isDucking = true;
        else
             isDucking = false;
        animator.SetBool("isDucking", isDucking);
    }
    private void Move(float x)
    {
        rigidbody2D.AddForce(Vector2.right * x * horizontalForce * ((isGrounded) ? 1 : airFactor));

        rigidbody2D.velocity = new Vector2(Mathf.Clamp(rigidbody2D.velocity.x, -maxSpeed, maxSpeed), rigidbody2D.velocity.y);

        animator.SetFloat("xSpeed", rigidbody2D.velocity.x);
        animator.SetFloat("ySpeed", rigidbody2D.velocity.y * -1);

        if (isGrounded)
        {
            if (x != 0.0f)
            {
              //  animationState = PlayerAnimationState.RUN;
               // animator.SetInteger("AnimationState", (int)animationState);

            }
            else
            {
                //  animationState = PlayerAnimationState.IDLE;
                //  animator.SetInteger("AnimationState", (int)animationState);
                animator.SetBool("isGrounded", true);
            }
        }
    }
    private void Jump(float y)
    {
        if ((isGrounded) && (y > 0))
        {
            rigidbody2D.AddForce(Vector2.up * verticalForce, ForceMode2D.Impulse);
        }
    }
    private void AirCheck()
    {
        if (!isGrounded)
        {
            //play jump animation
            //animationState = PlayerAnimationState.JUMP;
            //  animator.SetInteger("AnimationState", (int)animationState);
            animator.SetBool("isGrounded", false);
        }
    }
    private void Flip(float x)
    {
        if (x != 0)
        {
            transform.localScale = new Vector3((x > 0) ? 1 : -1, 1, 1);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(groundPoint.position, groundRadius);
    }
}
