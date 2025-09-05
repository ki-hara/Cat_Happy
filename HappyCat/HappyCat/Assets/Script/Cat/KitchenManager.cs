using Cysharp.Threading.Tasks;
using HC.Data;
using HC.Event;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace HC.Game
{
    public class KitchenManager
    {
        static Dictionary<int, Food> foodDatas = new Dictionary<int, Food>();
        static Queue<Food> orders = new Queue<Food> ();
        public static void KitchenInit()
        {
            Bind();
        }
        public static void KitchenUpdate()
        {
            if(orders.Count > 0)
            {
                if(foodDatas.Count < 6)
                {
                    if (!string.IsNullOrEmpty(DataManager.ServerData.furnitureData.oven_01) && !foodDatas.ContainsKey(0)) StartCooking(0);
                    else if (!string.IsNullOrEmpty(DataManager.ServerData.furnitureData.oven_02) && !foodDatas.ContainsKey(1)) StartCooking(1);
                    else if (!string.IsNullOrEmpty(DataManager.ServerData.furnitureData.oven_03) && !foodDatas.ContainsKey(2)) StartCooking(2);
                    else if (!string.IsNullOrEmpty(DataManager.ServerData.furnitureData.oven_04) && !foodDatas.ContainsKey(3)) StartCooking(3);
                    else if (!string.IsNullOrEmpty(DataManager.ServerData.furnitureData.oven_05) && !foodDatas.ContainsKey(4)) StartCooking(4);
                    else if (!string.IsNullOrEmpty(DataManager.ServerData.furnitureData.oven_06) && !foodDatas.ContainsKey(5)) StartCooking(5);
                }
            }
        }
        public static void Close()
        {
            UnBind();
        }

        static async void StartCooking(int index)
        {
            var food = orders.Dequeue();
            foodDatas.Add(index, food);
            food.foodData.KetchenIndex = index;
            food.Moving(E_FOOD.KITCHEN);
            await UniTask.Delay(TimeSpan.FromSeconds(food.Data.time));
            food.Moving(E_FOOD.RESTAURANT);
            foodDatas.Remove(index);
            GameEvent.ServiceEvents.Emit(new FinishCookingEvent(food.foodData));
        }

        static void Bind()
        {
            GameEvent.ServiceEvents.On<StartCookingEvent>(OnCooking);
        }
        static void UnBind()
        {
            GameEvent.ServiceEvents.Off<StartCookingEvent>(OnCooking);
        }

        static async void OnCooking(StartCookingEvent e)
        {
            orders.Enqueue(e.Food);
        }
    }
}

