using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class MoveAndJump : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 5f;         
    public float jumpForce = 7f;

    //增加一个基础速度
    private float baseMoveSpeed;
    private Coroutine speedCoroutine;

    [Header("地面检测")]
    public Transform groundCheckPoint;   
    public float groundCheckRadius = 0.2f; 
    public LayerMask groundLayer;       
    private Rigidbody2D rb;             
    private float moveInput;            
    private bool isGrounded;

    private Animator anim;
    public bool isDead;
    //精灵渲染器
    private SpriteRenderer sprite;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponent<Animator>();
        //记录基础速度
        baseMoveSpeed = moveSpeed;
    }

    void Update()
    {
        anim.SetBool("isDead", isDead);
        if (isDead)
        {
            return;
        }

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
        if (Input.GetMouseButtonDown (0))
        {
            anim.SetTrigger("Attack");
        }
        
    }

    void FixedUpdate()
    {
        // 保持当前的 Y 轴速度，只改变水平 X 轴的速度
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
    }

    public void PlayerHurt ()
    {
        anim.SetTrigger("hurt");
    }

    //增加移动速度
    public void IncreaseMoveSpeed(float amount, float duration)
    {
        //如果之前已经有速度Buff，先停止之前的计时
        if (speedCoroutine != null)
        {
            StopCoroutine(speedCoroutine);
        }

        //重新开始Buff
        speedCoroutine = StartCoroutine(SpeedBuffCoroutine(amount, duration)
        );
    }
    //增加协程
    private IEnumerator SpeedBuffCoroutine(float amount, float duration)
    {
        moveSpeed = baseMoveSpeed + amount;
        Debug.Log("移动速度增加：" + amount + " 当前移动速度：" + moveSpeed);
        yield return new WaitForSeconds(duration);
        moveSpeed = baseMoveSpeed;
        Debug.Log("移动速度Buff结束，恢复基础速度：" + baseMoveSpeed);
        speedCoroutine = null;
    }

    public void PlayerDead()
    {
        isDead = true;
    }

}
