using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HC.UI
{
    public class ProfileUIPopup : UIBase
    {
        #region define color
        readonly public string selectTextColor = "#5B3621";
        readonly public string defaultTextColor = "#CF926E";
        #endregion

        [SerializeField] HCButton profileButton;
        [SerializeField] TextMeshProUGUI profileText;
        [SerializeField] HCButton frameButton;
        [SerializeField] TextMeshProUGUI frameText;
        [SerializeField] HCButton cancelButton;
        [SerializeField] HCButton confirmButton;

        bool selectProfile = true;

        public override async UniTask OnOpen()
        {
            Bind();

            //await UniTask.Delay(5000);
        }

        void Bind()
        {
            profileButton.onClick.AddListener(OnProfile);
            frameButton.onClick.AddListener(OnFrame);
            cancelButton.onClick.AddListener(Close);
            confirmButton.onClick.AddListener(OnConfirm);
        }


        public override void OnClose()
        {
            profileButton.onClick.RemoveAllListeners();
            frameButton.onClick.RemoveAllListeners();
            cancelButton.onClick.RemoveAllListeners();
            confirmButton.onClick.RemoveAllListeners();

            base.OnClose();
        }

        void UIUpdate()
        {
            if(selectProfile)
            {
                profileButton.GetComponent<Image>().enabled = true;
                frameButton.GetComponent<Image>().enabled = false;
                Color color;
                if (ColorUtility.TryParseHtmlString(selectTextColor, out color))
                    profileText.color = color;
                if (ColorUtility.TryParseHtmlString(defaultTextColor, out color))
                    frameText.color = color;
            }else
            {
                profileButton.GetComponent<Image>().enabled = false;
                frameButton.GetComponent<Image>().enabled = true;
                Color color;
                if (ColorUtility.TryParseHtmlString(selectTextColor, out color))
                    frameText.color = color;
                if (ColorUtility.TryParseHtmlString(defaultTextColor, out color))
                    profileText.color = color;
            }
        }

        #region event
        void OnProfile()
        {
            selectProfile = true;
            UIUpdate();
        }
        void OnFrame()
        {
            selectProfile = false;
            UIUpdate();
        }
        void OnConfirm()
        {
            Close();
        }

        #endregion
    }
}

