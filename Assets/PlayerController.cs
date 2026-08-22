using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("移动")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    
    [Header("组件")]
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator anim;
    
    [Header("地面检测")]
    public Transform groundCheck;      // 脚下检测点
    public float checkRadius = 0.2f;   // 检测半径
    public LayerMask groundLayer;      // 地面图层
    private bool isGrounded;
    
    private float moveInput;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }
    
    void Update()
    {
        // 获取输入
        moveInput = Input.GetAxis("Horizontal");  // A/D 或 左右箭头
        
        // 跳跃
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
        
        // 翻转角色
        if (moveInput > 0)
            sr.flipX = false;   // 朝右
        else if (moveInput < 0)
            sr.flipX = true;    // 朝左
        
        // 动画
        if (anim != null)
        {
            anim.SetFloat("speed", Mathf.Abs(moveInput));
            anim.SetBool("isGrounded", isGrounded);
        }
    }
    
    void FixedUpdate()
    {
        // 移动
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
        
        // 地面检测
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
    }
    
    // 可视化地面检测范围
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
        }
    }
}