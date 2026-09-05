using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitch : MonoBehaviour
{
    public int index;
    public void LoadGameScene(int index)
    {
        SceneManager.LoadScene(index);

    }
}
