using UnityEngine;
public class ResourcePickup : MonoBehaviour
{
    //拾取物对应的资源ID，Inspector面板配置，默认1001
    public int resourceID = 1001;
    //拾取后获得的资源数量
    public int amount = 1;
    //背包管理器引用
    public ResourseManager manager;
    //2D触发器进入事件
    private void OnTriggerEnter2D(Collider2D other)
    {
        //判断进入触发区域的物体标签是否为Player玩家
        if(other.CompareTag("Player"))
        {
            //调用背包资源管理器，增加对应资源
            manager.AddResource(resourceID, amount);
            //拾取完成，销毁地面上这个资源物体
            Destroy(gameObject);
        }
    }
}
