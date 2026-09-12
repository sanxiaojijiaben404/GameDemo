using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections;
using System;
public class CGControl : MonoBehaviour
{
    [Header("开场CG")]
    public VideoPlayer vpOpening;
    public RawImage uiOpening;
    [Header("存档CG")]
    public VideoPlayer vpSave;
    public RawImage uiSave;
    public float saveCgDuration = 2f;
    [Header("结局CG")]
    public VideoPlayer vpEnding;
    public RawImage uiEnding;

    //结局后清空游戏数据
    public SaveManager saveManager;

    public ResourceSystemHost resourceSystem;
    public WorldStateManager worldStateManager;
    [Header("启动自动播放开场")]
    public bool autoPlayOpening = true;
    [Header("游戏主BGM音源")]
    public AudioSource mainBgmAudio;
    [Header("结局CG结束跳转场景索引，-1就是不跳转")]
    public int endingTargetSceneIndex = -1;
    private bool _isPlayingSaveCg = false;
    void Start()
    {
        if (autoPlayOpening)
        {
            PlayOpening();
        }
    }
    void Awake()
    {
        //注册播放完成事件
        vpOpening.loopPointReached += OnOpeningFinish;
        vpEnding.loopPointReached += OnEndingFinish;
        uiOpening.gameObject.SetActive(false);
        uiEnding.gameObject.SetActive(false);
    }
    void Update()
    {
        // 空格键跳过CG：跳到视频末尾，让原生事件处理结束，不暴力Stop
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (vpOpening.isPlaying)
            {
                vpOpening.time = vpOpening.length;
            }
            if (vpEnding.isPlaying)
            {
                vpEnding.time = vpEnding.length;
            }
        }
    }
    //调用：播放开场CG
    public void PlayOpening()
    {
        uiOpening.gameObject.SetActive(true);
        vpOpening.Play();
        // 暂停游戏BGM
        if (mainBgmAudio != null)
        {
            mainBgmAudio.Pause();
        }
    }
    //开场播放完毕回调
    void OnOpeningFinish(VideoPlayer player)
    {
        vpOpening.Stop();
        uiOpening.gameObject.SetActive(false);
        Debug.Log("开场CG结束，进入游戏主场景");
        // 恢复BGM
        if (mainBgmAudio != null)
        {
            mainBgmAudio.UnPause();
        }
        //SceneManager.LoadScene("MainGame");
    }
    //调用：播放结局CG
    public void PlayEnding()
    {
        uiEnding.gameObject.SetActive(true);
        vpEnding.Play();
        if (mainBgmAudio != null)
        {
            mainBgmAudio.Pause();
        }
    }
    void OnEndingFinish(VideoPlayer player)
    {
        vpEnding.Stop();
        // ❌先不要关闭 uiEnding！这里注释掉 SetActive(false)
        Debug.Log("结局CG播放完成，可以返回主菜单");
        // 恢复BGM
        if (mainBgmAudio != null)
        {
            mainBgmAudio.UnPause();
        }
        if (endingTargetSceneIndex >= 0)
        Debug.Log("结局CG播放完成，开始清理游戏数据");
        SaveManager saveManager =FindObjectOfType<SaveManager>();
        if (saveManager != null)
        {
            // 直接加载场景，旧场景（连同CG uiEnding）会一起销毁，不会闪旧画面
            SceneManager.LoadScene(endingTargetSceneIndex);
            saveManager.ClearGameData(success =>
            {
                if (success)
                {
                    Debug.Log("游戏数据清理完成，进入主菜单");
                    GoToEndingTargetScene();
                }
                else
                {
                    Debug.LogError("游戏数据清理失败");
                }
            });
        }
        else
        {
            // 不跳转场景的时候才关闭UI
            uiEnding.gameObject.SetActive(false);
            Debug.LogError("找不到SaveManager");
        }
    }
    //==== 修改这里：增加可选回调参数 ====
    public void PlaySaveCg(Action onComplete = null)
    {
        Debug.Log($"PlaySaveCg被调用，_isPlayingSaveCg={_isPlayingSaveCg}");
        if (_isPlayingSaveCg)
        {
            Debug.LogWarning("CG正在播放，拒绝重复调用");
            onComplete?.Invoke();
            return;
        }
        if (vpSave == null || uiSave == null)
        {
            Debug.LogError("vpSave或者uiSave为空！");
            onComplete?.Invoke();
            return;
        }
        Debug.Log("开始启动存档CG协程");
        StartCoroutine(PlaySaveCgCoroutine(onComplete));
    }
    //==== 协程接收回调参数 ====
    private IEnumerator PlaySaveCgCoroutine(Action onComplete)
    {
        _isPlayingSaveCg = true;
        if (mainBgmAudio != null)
        {
            mainBgmAudio.Pause();
        }
        uiSave.gameObject.SetActive(true);
        vpSave.Play();
        //等待视频加载准备完成
        while (!vpSave.isPrepared)
        {
            yield return null;
        }
        float videoRealLength = (float)vpSave.length;
        Debug.Log($"存档CG真实时长：{videoRealLength} 秒");
        float timer = 0f;
        while (timer < videoRealLength)
        {
            //空格可以跳过CG
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("空格跳过存档CG");
                break;
            }
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
        vpSave.Stop();
        uiSave.gameObject.SetActive(false);
        if (mainBgmAudio != null)
        {
            mainBgmAudio.UnPause();
        }
        _isPlayingSaveCg = false;
        Debug.Log("存档CG流程结束");
        //==== 全部结束后执行回调 ====
        onComplete?.Invoke();
    }

    private void GoToEndingTargetScene()
    {
        // 恢复BGM
        if (mainBgmAudio != null)
        {
            mainBgmAudio.UnPause();
        }
        if (endingTargetSceneIndex >= 0)
        {
            // 直接加载场景，旧场景会一起销毁
            SceneManager.LoadScene(endingTargetSceneIndex);
        }
        else
        {
            uiEnding.gameObject.SetActive(false);
        }
    }
}
