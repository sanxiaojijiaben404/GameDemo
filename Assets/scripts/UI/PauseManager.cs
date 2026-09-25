using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;

    private int _pauseCount = 0;

    // CG锁 
    public bool isCutscenePlaying { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 开始CG，锁定所有暂停弹窗
    public void StartCutscene()
    {
        isCutscenePlaying = true;
    }

    // CG结束，解锁
    public void EndCutscene()
    {
        isCutscenePlaying = false;
    }

    public void RequestPause()
    {
        // 如果正在播放CG，直接退出，不允许打开暂停类窗口
        if (isCutscenePlaying)
        {
            Debug.Log("CG播放中，禁止打开暂停/背包面板");
            return;
        }
        _pauseCount++;
        UpdateTimeScale();
    }

    public void CancelPause()
    {
        _pauseCount--;
        if (_pauseCount < 0) _pauseCount = 0;
        UpdateTimeScale();
    }

    void UpdateTimeScale()
    {
        if (_pauseCount > 0)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    public bool IsPaused => _pauseCount > 0;
}
