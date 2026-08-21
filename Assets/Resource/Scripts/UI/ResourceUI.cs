using UnityEngine;
using TMPro;
//屏幕上方一直显示：布料碎片*x
public class ResourceUI : MonoBehaviour
{
    //背包管理器引用
    public ResourseManager manager;
    //用来显示文字的TMP文本组件
    public TMP_Text text;
    void Update()
    {
        //查询ID=1001（布料碎片）的资源数量
        int count = manager.GetResourceCount(1001);
        //更新文本显示
        text.text = "布料碎片*" + count;
    }
}
