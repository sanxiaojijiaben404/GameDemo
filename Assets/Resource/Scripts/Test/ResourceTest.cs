using System.Collections.Generic;
using UnityEngine;
public class ResourceTest : MonoBehaviour
{
    public ResourseManager manager;
    void Start()
    {
        manager.AddResource(1001, 5);
        manager.Save();
        manager.Load();
        Debug.Log("布料数量:" + manager.GetResourceCount(1001));
        PlayerSaveData data = SaveSystem.Load();
        if(data != null )
        {
            foreach(ResourseData r in data.resourses)
            {
                Debug.Log(r.name + ":" + r.count);
            }
        }
    }
}
