using System;
//泛型集合命名空间
using System.Collections.Generic;
[Serializable]
//服务器返回到资源列表数据
public class ResourceListData
{
    //资源泛型列表，存放多条ResourceData对象
    public List<ResourseData> resources = new List<ResourseData>();
}
