using Cysharp.Threading.Tasks;
using HC.Event;
using HC.Utils;
using UnityEngine;

public class TopMenu : UIBase
{
    [SerializeField] HCButton profileButton;
    [SerializeField] HCButton coinButton;
    [SerializeField] HCButton cashButton;
    [SerializeField] HCButton shopButton;
    [SerializeField] HCButton dropboxButton;

    [SerializeField] GameObject morePanel;
    [SerializeField] HCButton settingsButton;
    [SerializeField] HCButton calenderButton;
    [SerializeField] HCButton colection_bookButton;
    [SerializeField] HCButton cameraButton;
    [SerializeField] HCButton mailButton;
    public override UniTask OnOpen()
    {
        Bind();
        return base.OnOpen();
    }

    void Bind()
    {
        profileButton.onClick.AddListener(OnProfile);
        coinButton.onClick.AddListener(OnCoin);
        cashButton.onClick.AddListener(OnCash);
        shopButton.onClick.AddListener(OnShop);
        dropboxButton.onClick.AddListener(OnDropBox);

        settingsButton.onClick.AddListener(OnSettings);
        calenderButton.onClick.AddListener(OnCalender);
        colection_bookButton.onClick.AddListener(OnColectBook);
        cameraButton.onClick.AddListener(OnCamera);
        mailButton.onClick.AddListener(OnMail);
    }

    public override void OnClose()
    {
        profileButton.onClick.RemoveAllListeners();
        coinButton.onClick.RemoveAllListeners();
        cashButton.onClick.RemoveAllListeners();
        shopButton.onClick.RemoveAllListeners();
        dropboxButton.onClick.RemoveAllListeners();

        settingsButton.onClick.RemoveAllListeners();
        calenderButton.onClick.RemoveAllListeners();
        colection_bookButton.onClick.RemoveAllListeners();
        cameraButton.onClick.RemoveAllListeners();
        mailButton.onClick.RemoveAllListeners();


        base.OnClose();
    }

    #region event
    void OnProfile() {
        UIManager.ShowUI("ProfilePopup");
    }
    void OnCoin() {
    }
    void OnCash() { }
    void OnShop() {
    }
    void OnDropBox() {
        morePanel.SetActive(!morePanel.activeSelf);
    }
    void OnSettings() { }
    void OnCalender() { }
    void OnColectBook() { }
    void OnCamera() { }
    void OnMail() { }
    #endregion
}
