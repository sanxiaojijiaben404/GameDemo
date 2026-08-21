using UnityEngine;
public class NetworkTest : MonoBehaviour
{
    public ResourceNetwork network;
    void Start()
    {
        ResourseData data = new ResourseData();
        data.id = 1001;
        data.name = "布料碎片";
        data.count = 1;
        network.AddResourceToServer(data);
    }

}
