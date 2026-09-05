using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;

public class AttackControl : MonoBehaviour
{
    [Header("属性")]
    [SerializeField] protected float maxHealth;
    [SerializeField] protected float currentHealth;

    [Header("无敌时间")]
    public bool invulnerable;
    public float invulnerableDuration;

    [Header("Player Gethit")]
    public Slider hpSlider;

    private Animation anim;

    public UnityEvent OnHurt;
    public UnityEvent OnDie;

    private void Start()
    {
        anim = GetComponent<Animation>();
        hpSlider.value = currentHealth / maxHealth;
    }
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
        if (currentHealth - damage > 0f)
        {
            currentHealth -= damage;
            StartCoroutine(nameof(InvulnerableCoroutine));//启动无敌状态的协程
            hpSlider.value = currentHealth / maxHealth;
            OnHurt?.Invoke();//这里用？可以防空
        }
        else
        {
            Die();
        }
    }

    public  virtual void Die()
    {
        currentHealth = 0f;
        hpSlider.value = 0;
        OnDie?.Invoke ();
        SceneManager.LoadScene("StartScene");
    }

    //无敌
   protected virtual IEnumerator InvulnerableCoroutine ()
    {
        invulnerable = true;
        //等待无敌时间
        yield return new WaitForSeconds(invulnerableDuration);
        invulnerable = false;
    }

    //增加回血方法
    public virtual void Heal(float amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        if (hpSlider != null)
        {
            hpSlider.value = currentHealth / maxHealth;
        }
        Debug.Log(
            "回血:"
            + amount
            + " 当前血量:"
            + currentHealth
        );
    }
}
