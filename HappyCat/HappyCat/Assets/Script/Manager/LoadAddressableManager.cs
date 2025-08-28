using HC.Game;
using HC.Utils;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.U2D;

namespace HC.Resource
{
    public class LoadAddressableManager
    {
        //cache
        static GameObject rootCat;

        //parent
        static string UIPath = "Assets/HC_Resources/Prefabs/UI/";
        static string PopupPath = "Assets/HC_Resources/Prefabs/Popup/";
        static string catPrefabPath = "Assets/HC_Resources/Prefabs/Cat/Cat.prefab";
        static string catAnimationPath = "Assets/HC_Resources/Art/Cat/Animator/";
        static string imageFurniturePath = "Assets/HC_Resources/Art/Atlas/BG.spriteatlasv2";
        static string foodPrefab = "Assets/HC_Resources/Prefabs/Food/Food.prefab";
        static string imagefoodPath = "Assets/HC_Resources/Art/Atlas/Food.spriteatlasv2";
        static string audioPath = "Audio/";

        private static async Task<T> Load<T>(string path)
        {
            var handle = await Addressables.LoadAssetAsync<T>(path).Task;

            return handle;
        }

        public static async Task<T> Load_AnimController<T>(string name)
        {
            string path = $"{catAnimationPath}AC_{name}";

            return await Load<T>(path);
        }

        public static async Task<T> LoadUI<T>(string path)
        {
            var handle = Addressables.LoadAssetAsync<T>($"{UIPath}{path}.prefab");
            await handle.Task;

            if(handle.Status == AsyncOperationStatus.Succeeded)
            {
                return handle.Result;
            }
            else 
                return default;
        }
        public static async Task<T> LoadPopup<T>(string path)
        {
            var handle = Addressables.LoadAssetAsync<T>($"{PopupPath}{path}.prefab");
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                return handle.Result;
            }
            else
                return default;
        }

        public static async Task<T> Create_Cat<T>(int code)
        {
            if (rootCat == null) rootCat = GameObject.Find("Cats");

            var p = await Load<GameObject>(catPrefabPath);
            var cat = GameObject.Instantiate(p);
            cat.GetComponent<Cat_Actor>().Init(code);
            cat.SetActive(true);
            
            cat.transform.SetParent(rootCat.transform);
            return cat.GetComponent<T>();
        }

        public static async Task<Sprite> LoadImage_Furniture(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            var sa = await Load<SpriteAtlas>(imageFurniturePath);
            return sa.GetSprite(name);
        }
        public static async Task<Sprite> LoadImage_Food(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;
            var sa = await Load<SpriteAtlas>(imagefoodPath);
            return sa.GetSprite(name);
        }

        public static async Task<T> Create_Food<T>()
        {
            var p = await Load<GameObject>(foodPrefab);
            var food = GameObject.Instantiate(p);
            food.transform.SetParent(UIManager.WorldRoot.transform);
            food.SetActive(false);
            return food.GetComponent<T>();
        }
        public static async Task<T> LoadAudio<T>(string name)
        {
            var p = await Load<T>($"{audioPath}{name}");
            return p;
        }
    }
}

