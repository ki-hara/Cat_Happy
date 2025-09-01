using HC.Config;
using HC.Data;
using HC.Event;
using HC.Network;
using HC.Scene;
using HC.Utils;
using System;
using System.Threading.Tasks;
using UGS;
using UnityEngine;
using UnityEngine.UI;

public class LoginPage : MonoBehaviour
{
    public Button touchToStartButton;

    [SerializeField]
    private Image progressBar;
    private float progress = 0;
    private bool loadComplete = false;

    private void Awake()
    {
        UIManager.Init();
    }
    void Start()
    {
        //Screen.orientation = ScreenOrientation.Portrait;
        Bind();
        GoogleLogin();

        Config.SetCamera(E_Camera.loadingPage);

        UIManager.ShowUI("OneButtonPopup");
    }
    void Bind()
    {
        NetworkEvent.ServiceEvents.On<GoogleLoginComplete>(OnLogin);
        touchToStartButton.onClick.AddListener(OnNext);
    }
    private void OnDestroy()
    {
        NetworkEvent.ServiceEvents.Off<GoogleLoginComplete>(OnLogin);
        touchToStartButton.onClick.RemoveAllListeners();
    }

    private void Update()
    {
        LoadingProgress();
        CheckLoad();
    }

    void CheckLoad()
    {
        if (loadComplete) return;

        if(progress >= 1)
        {
            loadComplete = true;
            OnLoading();
        }
    }

    void GoogleLogin()
    {
        
        NetworkDataManager.Init();
        NetworkDataManager.GoogleLogin.SignIn();
    }

    async void DataLoad()
    {
        UnityGoogleSheet.LoadAllData();
        DataManager.Init();

        while (progress < 1)
        {
            await DummyProgress();
        }
    }

    async Task DummyProgress()
    {
        await Task.Delay(TimeSpan.FromMilliseconds(20));
        progress += 0.05f;
        return;
    }
    void LoadingProgress()
    {
        progressBar.fillAmount = progress;
    }

    #region event
    void OnLogin(GoogleLoginComplete e)
    {
        if (e.success) DataLoad();
        else HC_Debug.Log("google login fail");
    }
    void OnLoading()
    {
        touchToStartButton.gameObject.SetActive(true);
    }
    void OnNext()
    {
        SceneLoader.GetSceneLoader.SceneLoad(SceneKind.GameScene);
    }
    #endregion
}
