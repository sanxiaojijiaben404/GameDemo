using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
public class MoveAndJump : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 5f;
    public float jumpForce = 20f;
    //增加一个基础速度
    private float baseMoveSpeed;
    private Coroutine speedCoroutine;

    [Header("跳跃手感参数")]
    public float gravityUp = 2.6f;
    public float gravityDown = 4.2f;
    public float coyoteTime = 0.15f;
    public float jumpBuffer = 0.15f;
    public float jumpCutMultiplier = 0.4f;

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

    //跳跃计时器变量
    private float coyoteTimer;
    private float jumpBufferTimer;
    private bool jumpRelease;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponent<Animator>();
        //记录基础速度
        baseMoveSpeed = moveSpeed;

        //把刚体原生重力缩放置1，交给代码控制重力
        rb.gravityScale = 1f;
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

        //土狼时间计时,就是你走到平台边缘掉下去，0.15 秒内按空格，依然可以跳起来
        if (isGrounded)
            coyoteTimer = coyoteTime;
        else
            coyoteTimer -= Time.deltaTime;

        //跳跃输入缓冲
        if (Input.GetButtonDown("Jump"))
            jumpBufferTimer = jumpBuffer;
        else
            jumpBufferTimer -= Time.deltaTime;

        //松开空格标记，用于小跳 
        if (Input.GetButtonUp("Jump"))
            jumpRelease = true;


        // 获取键盘输入 
        moveInput = Input.GetAxisRaw("Horizontal");

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
        if (Input.GetMouseButtonDown(0))
        {
            anim.SetTrigger("Attack");
        }

    }

    void FixedUpdate()
    {
        // 保持当前的 Y 轴速度，只改变水平 X 轴的速度
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        //执行跳跃
        if (jumpBufferTimer > 0f && coyoteTimer > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpBufferTimer = 0;
            jumpRelease = false;
        }

        //松开空格削减跳跃高度
        if (jumpRelease && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * jumpCutMultiplier);
            jumpRelease = false;
        }

        //区分上升、下落重力
        if (rb.velocity.y > 0)
        {
            rb.gravityScale = gravityUp;
        }
        else
        {
            rb.gravityScale = gravityDown;
        }
    }

    public void PlayerHurt()
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
