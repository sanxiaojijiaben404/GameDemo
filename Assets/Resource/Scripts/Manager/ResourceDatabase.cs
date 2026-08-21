using UnityEngine;
public class ResourceDatabase : MonoBehaviour
{
    //所有资源配置
    public ResourceConfig[] resources;
    //通过ID查找资源
    public ResourceConfig GetResource(int id)
    {
        foreach (ResourceConfig resource in resources)
        {
            if (resource.id == id)
            {
                return resource;
            }
        }
        return null;
    }

}
