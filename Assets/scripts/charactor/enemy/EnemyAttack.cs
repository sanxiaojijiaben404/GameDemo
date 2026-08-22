using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class EnemyAttack : AttackControl 
{
    public float damage;
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<AttackControl>().TakeDamage(damage);
            Debug.Log("Íæ¼ÒµôÑª");
        }
    }
}
