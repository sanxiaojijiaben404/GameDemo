using UnityEngine;

public class EnemyTer : MonoBehaviour
{

    public EnemyBase enemyBase;
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            enemyBase.FindPlayer(collision.gameObject);
            // Add your logic here for when the player enters the trigger area
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            enemyBase.OutPlayer();
            // Add your logic here for when the player exits the trigger area
        }
    }
}
