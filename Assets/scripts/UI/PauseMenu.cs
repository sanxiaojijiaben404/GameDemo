using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PauseMenu : MonoBehaviour
{
    [Tooltip("暂停菜单根面板")]
    public GameObject pauseMenuPanel;
    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (PauseManager.Instance.isCutscenePlaying)
            {
                return;
            }
            Debug.Log("ESC按下");
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (pauseMenuPanel == null)
        {
            Debug.LogError("pauseMenuPanel没有赋值");
            return;
        }

        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        pauseMenuPanel.SetActive(true);
        PauseManager.Instance.RequestPause();
        isPaused = true;
    }

    public void ResumeGame()
    {
        Debug.Log("【继续游戏】按钮被点击");
        pauseMenuPanel.SetActive(false);
        PauseManager.Instance.CancelPause();
        isPaused = false;
    }

    // 返回开始菜单，不再关闭程序
    public void QuitGame()
    {
        PauseManager.Instance.CancelPause();
        SceneManager.LoadScene("StartScene");
    }
}

