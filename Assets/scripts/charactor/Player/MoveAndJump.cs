using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class PlayerMovement : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 5f;         
    public float jumpForce = 7f;         

    [Header("地面检测")]
    public Transform groundCheckPoint;   
    public float groundCheckRadius = 0.2f; 
    public LayerMask groundLayer;       

    private Rigidbody2D rb;             
    private float moveInput;            
    private bool isGrounded;

    private Animator anim;

    //精灵渲染器
    private SpriteRenderer sprite;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponent<Animator>();
        
    }

    void Update()
    {
        
        //  检测是否在地上
        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);

        // 获取键盘输入 
        moveInput = Input.GetAxisRaw("Horizontal");

        //  检测跳跃按键 (空格键)
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        if (moveInput > 0)
        {
            anim.SetBool("Running", true);
            sprite.flipX = false;
        }
        else if (moveInput < 0)
        {
            anim.SetBool("Running", true);
            sprite.flipX = true;
        }
        else
        {
            anim.SetBool("Running", false);
        }

        //攻击动画
        if (Input.GetMouseButton (0))
        {
            anim.SetTrigger("Attack");
        }
    }

    void FixedUpdate()
    {
        // 保持当前的 Y 轴速度，只改变水平 X 轴的速度
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
    }

  

}
