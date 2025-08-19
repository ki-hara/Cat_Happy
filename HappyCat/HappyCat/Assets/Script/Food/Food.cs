using Cysharp.Threading.Tasks;
using DG.Tweening;
using HC.Data;
using HC.Event;
using HC.Resource;
using HC.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace HC.Game
{
    public class Food : MonoBehaviour
    {
        [SerializeField] GameObject questionObj;
        [SerializeField] Image foodImage;
        HCButton button;

        public FoodTable.Data Data { get; set; }

        public FoodData foodData;
        public void Init(int code, int index)
        {
            Data = UGSManager.GetData<FoodTable.Data>(code);
            button = GetComponent<HCButton>();
            Bind();

            foodData = new FoodData(index, 0);

            questionObj.SetActive(true);
            button.enabled = true;

            SetPosition();
            LoadImage();
        }

        public void Moving(E_FOOD foodType)
        {
            questionObj.SetActive(false);
            var targetPos = foodType == E_FOOD.KITCHEN ? FurnitureManager.GetOvenPosition(foodData.KetchenIndex) : FurnitureManager.GetTablePosition(foodData.TableIndex);
            //Vector3 d2Pos = Camera.main.WorldToScreenPoint(targetPos);
            //transform.DOMove(d2Pos, 0.5f);
            transform.DOMove(targetPos, 0.5f);
        }

        public async void LoadImage()
        {
            await LoadAddressableManager.LoadImage_Food(Data.icon);
            gameObject.SetActive(true);
        }
        void SetPosition()
        {
            var tablePos = CatPathManager.GetTablePosition(foodData.TableIndex);
            tablePos += new Vector3(60,150,0);

            //Vector3 d2Pos = Camera.main.WorldToScreenPoint(tablePos);
            //transform.position = d2Pos;

            transform.position = tablePos;
        }

        public async void Finish()
        {
            gameObject.SetActive(false);
            await UnBind();
            GameObject.Destroy(gameObject);
        }

        void Bind()
        {
            button.onClick.AddListener(OnOrder);
            GameEvent.ServiceEvents.On<FinishEatingEvent>(OnFinish);
        }

        void OnOrder()
        {
            GameEvent.ServiceEvents.Emit(new StartCookingEvent(this));
            button.enabled = false;
        }
        void OnFinish(FinishEatingEvent e)
        {
            if (e.TableIndex != foodData.TableIndex) return;

            Finish();
        }
        async UniTask UnBind()
        {
            button.onClick.RemoveAllListeners();
            await UniTask.Delay(100); // FinishEatingEvent 이벤트 실행중에 제거하면 오류발생
            GameEvent.ServiceEvents.Off<FinishEatingEvent>(OnFinish);
        }
    }
}

