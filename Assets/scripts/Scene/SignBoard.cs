using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class SignBoard : MonoBehaviour
{
    [Header("提示文字")]
    public string tipText;

    [Header("UI提示文本组件")]
    public TMP_Text uiTipText;

    private bool _isShow = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag ("Player")&& !_isShow)
        {
            ShowTip();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other .CompareTag ("Player"))
        {
            HideTip();
        }
    }

    void ShowTip()
    {
        _isShow = true;
        uiTipText.text = tipText;
        uiTipText.gameObject.SetActive(true);
    }
    void HideTip()
    {
        _isShow = false;
        uiTipText.gameObject.SetActive(false);
    }
}
