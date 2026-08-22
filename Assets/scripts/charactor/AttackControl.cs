using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class AttackControl : MonoBehaviour
{
    [Header("属性")]
    [SerializeField] protected float maxHealth;
    [SerializeField] protected float currentHealth;

    [Header("无敌时间")]
    public bool invulnerable;
    public float invulnerableDuration;

    protected virtual void OnEnable()
    {
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(float damage)
    {
        if (invulnerable)
        {
            return;
        }
        
        StartCoroutine(nameof(InvulnerableCoroutine));//启动无敌状态的协程
        currentHealth -= damage;
        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public  virtual void Die()
    {
        currentHealth = 0f;
        Destroy(this.gameObject);
    }

    //无敌
   protected virtual IEnumerator InvulnerableCoroutine ()
    {
        invulnerable = true;
        //等待无敌时间
        yield return new WaitForSeconds(invulnerableDuration);
        invulnerable = false;
    }
}
