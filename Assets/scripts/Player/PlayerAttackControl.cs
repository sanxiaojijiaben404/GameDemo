using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class PlayerAttackControl : AttackControl 
{
    [Header("近战攻击")]
    public float meleeAttackDamage;
    public Vector2 attackSize = new Vector2(1f, 1f);//攻击范围
    private Vector2 AttackAreaPos;
    public float offsetX = 1f;
    public float offsetY = 1f;

    //增加攻击力基础
    private float baseAttackDamage;
    private Coroutine attackCoroutine;

    private SpriteRenderer spriteRenderer;
    public LayerMask Enemy;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void MeleeAttackAnimEvent(float isAttack)
   {
        Debug.Log($"攻击开始时 spriteRenderer = {spriteRenderer}");
        AttackAreaPos = transform.position;
        offsetX = spriteRenderer.flipX ? -Mathf.Abs(offsetX) : Mathf.Abs(offsetX);//是否翻转
        AttackAreaPos.x += offsetX;
        AttackAreaPos.y += offsetY;
        //检测一个矩形区域内，有没有碰撞体，只返回第一个碰到的物体
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(AttackAreaPos ,attackSize ,0f,Enemy);//中心点，半径，旋转
    
        foreach (Collider2D hitCollider in hitColliders)
        {
            if (hitCollider == null) continue;

            // 从碰撞体往上父物体搜索 EnemyBase
            EnemyBase enemy = hitCollider.GetComponentInParent<EnemyBase>();

            if (enemy != null)
            {
                enemy.GetHit(meleeAttackDamage * isAttack);
                Debug.Log("敌人受伤");
            }
        }
    }

    //增加攻击力
    public void IncreaseAttackDamage(float amount, float duration)
    {
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
        }
        attackCoroutine = StartCoroutine(AttackBuffCoroutine(amount, duration)
        );
    }
    private IEnumerator AttackBuffCoroutine(float amount, float duration)
    {
        meleeAttackDamage = baseAttackDamage + amount;
        Debug.Log("攻击力增加：" + amount + " 当前攻击力：" + meleeAttackDamage);
        yield return new WaitForSeconds(duration);
        meleeAttackDamage = baseAttackDamage;
        Debug.Log("攻击力Buff结束，恢复基础攻击力：" + baseAttackDamage);
        attackCoroutine = null;
    }

    //绘图用于测试
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue  ;
        Gizmos.DrawWireCube(AttackAreaPos ,attackSize);
    }
}
