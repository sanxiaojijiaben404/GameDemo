using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyState
{
    Idle,//待机
    Patrol,//巡逻
    Chase,//追逐
    Attack,//攻击
    GetHit,//受击
    Death,//死亡
}

public class EnemyBase : MonoBehaviour
{
    [Header("Enemy 基础属性")]
    public float HPMax = 100f;
    public float HPNow = 100f;
    public float AttackDamage = 10f;

    [Header("Enemy 待机")]
    public float idleTime = 2f;
    public SpriteRenderer sr;
    public Animator enemyAnimator;

    [Header("Enemy 攻击")]
    public bool isAttacking = false;
    public bool canAttack = true;
    public float AttackRange = 1.5f;
    [HideInInspector]public float AttackTime = 1f;//攻击时间
    public float AttackWindup = 1f;//攻击前摇时间


    [Header("Enemy 移动")]
    public EnemyState currentState = EnemyState.Patrol;
    public Transform left;
    public Transform right;
    public Rigidbody2D rb;
    public bool isRight = true;
    public float speed = 1f;
    public bool canMove = true;
    public float chaseSpeed = 1.75f;
    public float patrolSpeed = 1f;

    [Header("Enemy 追击")]
    public GameObject player;
    public virtual void Start()
    {
        ChangeState(EnemyState.Patrol);
    }

    [Header("Enemy 受击")]
    public float GetHitTime = 0.5f;


    // Update is called once per frame
    public virtual void Update()
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                IdleUpdate();
                break;
            case EnemyState.Patrol:
                PatrolUpdate();
                break;
            case EnemyState.Chase:
                ChaseUpdate();
                break;
            case EnemyState.Attack:
                AttackUpdate();
                break;
            case EnemyState.GetHit:
                GetHitUpdate();
                break;
            case EnemyState.Death:
                DeathUpdate();
                break;
        }
    }

    public virtual void FixedUpdate()
    {
        if (canMove)
        {
            rb.velocity = new Vector2(isRight ? speed : -speed, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }


#region 状态机
    //待机
    public virtual void IdleEnter()
    {
        canMove = false;
        enemyAnimator.SetBool("IsRun",false);
        Invoke(nameof(Idle2Patrol), idleTime);
    }

    public virtual void IdleUpdate()
    {

    }

    public virtual void IdleExit()
    {
        CancelInvoke(nameof(Idle2Patrol));
    }    
    //巡逻
    public virtual void PatrolEnter()
    {
        canMove = true;
        speed = patrolSpeed;
        enemyAnimator.SetBool("IsRun",true);
    }

    public virtual void PatrolUpdate()
{
    // 原有的巡逻点边界判断
    if (isRight && transform.position.x >= right.position.x)
    {
        ChangeState(EnemyState.Idle);
    }
    else if (!isRight && transform.position.x <= left.position.x)
    {
        ChangeState(EnemyState.Idle);
    }

    // 前方障碍物检测
    float checkDistance = 0.5f; // 射线长度，根据敌人尺寸调整
    Vector2 direction = isRight ? Vector2.right : Vector2.left;
    Vector2 origin = transform.position;

    // 使用 LayerMask 只检测障碍物层（避免检测到敌人自身）
    LayerMask obstacleMask = LayerMask.GetMask("Obstacle"); // 需要提前创建 Obstacle 层
    RaycastHit2D hit = Physics2D.Raycast(origin, direction, checkDistance, obstacleMask);

    if (hit.collider != null)
    {
        // 如果检测到障碍物，进入 Idle 状态，之后 Idle2Patrol 会自动翻转方向
        ChangeState(EnemyState.Idle);
    }
}
    public virtual void PatrolExit()
    {
        enemyAnimator.SetBool("IsRun",false);
    }    
    //追击
    public virtual void ChaseEnter()
    {
        canMove = true;
        speed = chaseSpeed;
        enemyAnimator.SetBool("IsChase",true);
    }

    public virtual void ChaseUpdate()
    {
        if (player != null)
        {
            if (transform.position.x < player.transform.position.x)
            {
                isRight = true;
                sr.flipX = false;
            }
            else
            {
                isRight = false;
                sr.flipX = true;
            }
            if (Mathf.Abs(transform.position.x - player.transform.position.x) <= AttackRange)
            {   
                if (player.transform.position.x <= transform.position.x && !isRight)
                {
                    ChangeState(EnemyState.Attack);
                }
                else if (player.transform.position.x >= transform.position.x && isRight)
                {
                    ChangeState(EnemyState.Attack);
                }
                
            }
        }
    }

    public virtual void ChaseExit()
    {
       enemyAnimator.SetBool("IsChase",false);

      
    }   
    //攻击
    public virtual void AttackEnter()
    {
        canMove = false;
        isAttacking = false;
        canAttack = true;
       
    }

    public virtual void AttackUpdate()
    {
        if (canAttack)
        {
            isAttacking = true;
            canAttack = false;
            enemyAnimator.SetTrigger("Idle");
            Invoke(nameof(PerformAttack), AttackWindup);
        }
        if (Mathf.Abs(transform.position.x - player.transform.position.x) > AttackRange || (player.transform.position.x <= transform.position.x && isRight) || (player.transform.position.x >= transform.position.x && !isRight))
        {
            ChangeState(EnemyState.Chase);
        }
        
    }

    public virtual void AttackExit()
    {
        isAttacking = false;
        canAttack = true;
        CancelInvoke(nameof(PerformAttack));
        CancelInvoke(nameof(AttackCooldown));
    }   
    // 受击
    public virtual void GetHitEnter()
    {
        canMove = false;

    }

    public virtual void GetHitUpdate()
    {
        
    }

    public virtual void GetHitExit()
    {
        canMove = true;
    }   

    //死亡
    public virtual void DeathEnter()
    {

    }
    public virtual void DeathUpdate()
    {

    }
    public virtual void DeathExit()
    {

    }
#endregion

    public virtual void ChangeState(EnemyState newState)
    {
        //退出当前状态
        switch (currentState)
        {
            case EnemyState.Idle:
                IdleExit();
                break;
            case EnemyState.Patrol:
                PatrolExit();
                break;
            case EnemyState.Chase:
                ChaseExit();
                break;
            case EnemyState.Attack:
                AttackExit();
                break;
            case EnemyState.GetHit:
                GetHitExit();
                break;
            case EnemyState.Death:
                DeathExit();
                break;
        }
        //进入新状态    
        currentState = newState;
        switch (currentState)
        {
            case EnemyState.Idle:
                IdleEnter();
                break;
            case EnemyState.Patrol:
                PatrolEnter();
                break;
            case EnemyState.Chase:
                ChaseEnter();
                break;
            case EnemyState.Attack:
                AttackEnter();
                break;
            case EnemyState.GetHit:
                GetHitEnter();
                break;
            case EnemyState.Death:
                DeathEnter();
                break;
        }
    }

    public virtual void Idle2Patrol()
    {
        isRight = !isRight;
        //animation
        sr.flipX = !isRight;
        ChangeState(EnemyState.Patrol);       
    }

    public virtual void FindPlayer(GameObject mainPlayer)
    {
        player = mainPlayer;
        ChangeState(EnemyState.Chase);
    }
    public virtual void OutPlayer()
    {
        ChangeState(EnemyState.Patrol);
    }

    public virtual void PerformAttack()
    {
        enemyAnimator.SetTrigger("Attack1");
        Invoke(nameof(AttackCooldown), AttackTime);

    }

    public virtual void AttackCooldown()
    {
        isAttacking = false;
        canAttack = true;
    }

    public virtual void AttackPlayer()
    {
        // 在这里执行伤害结算逻辑
        // 例如检测玩家是否在攻击范围内，然后让玩家扣血
        if (player != null && Mathf.Abs(transform.position.x - player.transform.position.x) <= AttackRange)
            {
            // 调用玩家受伤方法
            player.GetComponent<PlayerAttackControl>()?.TakeDamage(10);
        }
    }

    public virtual void GetHit(float damage)
    {
        if (currentState != EnemyState.Death)
        {
            // 扣血逻辑
            HPNow -= damage;
            // if (HPNow <= 0)
            // {
            //   ChangeState(EnemyState.Death);
            // }
            // else
            // {
                ChangeState(EnemyState.GetHit);
            // }
        }


    }
}
