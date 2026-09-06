using UnityEngine;
public class ResourceUseManager : MonoBehaviour
{
    public ResourceSystemHost resourceSystem;
    public AttackControl playerHealth;
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
        switch (id)
        {
            //布料碎片
            case 1001:
                playerHealth.Heal(40);
                break;
            //疗愈草药
            case 1002:
                playerHealth.Heal(20);
                break;
            //移速草药
            case 1003:
                Debug.Log("增加移动速度");
                break;
            //攻击草药
            case 1004:
                Debug.Log("增加攻击力");
                break;
            default:
                Debug.Log("无效果资源");
                break;
        }
    }
}