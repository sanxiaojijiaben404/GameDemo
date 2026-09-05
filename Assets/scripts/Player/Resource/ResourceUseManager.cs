using UnityEngine;
public class ResourceUseManager : MonoBehaviour
{
    public ResourceSystemHost resourceSystem;
    public AttackControl playerHealth;
    public MoveAndJump playerMovement;
    public PlayerAttackControl playerAttack;
    public ResourceDatabase database;
    public void UseResource(int id)
    {
        ResourceConfig config =resourceSystem.database.GetResource(id);
        if (config == null)
        {
            Debug.LogError("资源不存在");
            return;
        }
        resourceSystem.Manager.RequestRemoveResource(id,1,success =>
            {
                if (!success)
                {
                    Debug.Log("使用失败");
                    return;
                }
                ApplyEffect(id);
            }
        );  
    }
    private void ApplyEffect(int id)
    {
        ResourceConfig config=database.GetResource(id);
        switch(config.effectType)
        {
            case ResourceEffectType.Heal:
                if(playerHealth != null)
                {
                    playerHealth.Heal(config.effectValue);
                }
                break;
            case ResourceEffectType.SpeedUp:
                if (playerMovement != null)
                {
                    playerMovement.IncreaseMoveSpeed(config.effectValue,config.effectDuration);
                }
                    break;
            case ResourceEffectType.AttackUp:
                if(playerAttack != null)
                {
                    playerAttack.IncreaseAttackDamage(config.effectValue,config.effectDuration);
                }
                break;
        }
    }
}