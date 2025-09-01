using HC.Resource;
using HC.Utils;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace HC.Game
{
    public class CatJoin
    {
        public bool Stop { get; set; }
        readonly float coolTimeMin = 10;
        readonly float coolTimeMax = 20;
        float coolTimeSec = 0;

        public void Update()
        {
            if (Stop) return;

            coolTimeSec -= Time.deltaTime;

            //Debug.Log(coolTimeSec);
            if(coolTimeSec <= 0)
            {
                coolTimeSec = UnityEngine.Random.Range(coolTimeMin, coolTimeMax);
                JoinCat();
            }
        }

        public void Load()
        {
            //waitTime
            float waitTimeHour = 10;
            int count = Mathf.CeilToInt(waitTimeHour / 2);

            Task.Run(async () => {
                for(int i = 0; i < count; i++)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(500));
                }
            });
        }

        public async void JoinCat(int catCode = -1)
        {
            Debug.Log("JoinCat");
            // CreateCat
            if(catCode == -1)
                catCode = UserUtill.GetCat().code;

            Debug.Log(catCode);
            var cat = await LoadAddressableManager.Create_Cat<Cat_Actor>(catCode);
        }
        public void Ads()
        {

        }
    }
}

