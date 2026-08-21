using UnityEngine;

[CreateAssetMenu(fileName = "ApiSettings", menuName = "Game/Api Settings")]
public class ApiSettings : ScriptableObject
{
    public string baseUrl; // 在 Inspector 里填你的服务器地址
}