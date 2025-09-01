using TMPro;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.UI;

public class OneButtonPopup : UIBase
{
    [SerializeField]
    private Button block;

    [SerializeField]
    private TextMeshProUGUI text;
    [SerializeField]
    private Button checkButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        block.onClick.AddListener(OnConfirm);
        checkButton.onClick.AddListener(OnConfirm);
    }

    public void SetText(string _text)
    {
        text.text = _text;
    }
    void OnConfirm()
    {
        Close();
    }
    void OnClose()
    {
        Close();
    }
}
