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
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        Debug.Log("【继续游戏】按钮被点击");
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    // 返回开始菜单，不再关闭程序
    public void QuitGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("StartScene");
    }
}

