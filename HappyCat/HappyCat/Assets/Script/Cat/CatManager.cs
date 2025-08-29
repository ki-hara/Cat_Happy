using HC.Data;
using HC.Event;
using HC.Resource;
using HC.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace HC.Game
{
    public class CatManager
    {
        static List<Cat_Actor> cats = new List<Cat_Actor>();
        static Queue<Cat_Actor> waitCats = new Queue<Cat_Actor>();
        static Dictionary<int, Cat_Actor> tableCats = new Dictionary<int, Cat_Actor>();
        static CatJoin joinCat;
        public static void CatInit()
        {
            joinCat = new CatJoin();

            Bind();
        }
        public static void CatUpdate()
        {
            joinCat?.Update();
            CheckWaitCat();
        }
        public static void Close()
        {
            UnBind();
        }
        static void Bind()
        {
            GameEvent.ServiceEvents.On<JoinCatEvent>(OnJoinCat);
            GameEvent.ServiceEvents.On<TableDestinationEvent>(OnDestination);
            GameEvent.ServiceEvents.On<FinishEatingEvent>(OnFinishEating);
        }
        static void UnBind()
        {
            GameEvent.ServiceEvents.Off<JoinCatEvent>(OnJoinCat);
            GameEvent.ServiceEvents.Off<TableDestinationEvent>(OnDestination);
            GameEvent.ServiceEvents.Off<FinishEatingEvent>(OnFinishEating);
        }
        private static void OnJoinCat(JoinCatEvent e)
        {
            waitCats.Enqueue(e.Cat);
            e.Cat.SetDestination(E_DESTINATION.WAIT, CatPathManager.GetWaitPosition(waitCats.Count - 1));
        }
        static async void OnDestination(TableDestinationEvent e)
        {
            Food food = await LoadAddressableManager.Create_Food<Food>();
            food.Init(10001, e.Tableindex); //임시 고유코드
        }

        static void OnFinishEating(FinishEatingEvent e)
        {
            Cat_Actor cat = tableCats[e.TableIndex];
            cats.Add(cat);
            tableCats.Remove(e.TableIndex);

            cat.Wander.WanderStart();
        }

        static void CheckWaitCat()
        {
            if (tableCats.Count >= 6 || waitCats.Count == 0) return;

            if (!string.IsNullOrEmpty(DataManager.ServerData.furnitureData.table1) && !tableCats.ContainsKey(0)) CatGoTable(0);
            else if (!string.IsNullOrEmpty(DataManager.ServerData.furnitureData.table2) && !tableCats.ContainsKey(1)) CatGoTable(1);
            else if (!string.IsNullOrEmpty(DataManager.ServerData.furnitureData.table3) && !tableCats.ContainsKey(2)) CatGoTable(2);
            else if (!string.IsNullOrEmpty(DataManager.ServerData.furnitureData.table4) && !tableCats.ContainsKey(3)) CatGoTable(3);
            else if (!string.IsNullOrEmpty(DataManager.ServerData.furnitureData.table5) && !tableCats.ContainsKey(4)) CatGoTable(4);
            else if (!string.IsNullOrEmpty(DataManager.ServerData.furnitureData.table6) && !tableCats.ContainsKey(5)) CatGoTable(5);
            
        }

        static async void CatGoTable(int index)
        {
            if (waitCats.Peek().GetState() != E_ANIMATION.IDLE) return;
            var cat = waitCats.Dequeue();
            UpdateWaitCat();
            tableCats.Add(index, cat);
            cat.SetTableDestination(new FoodData(index, 0));
        }
        static void UpdateWaitCat()
        {
            var array = waitCats.ToArray();
            for(int i = 0; i < array.Length; i++)
            {
                var cat = array[i];
                cat.SetDestination(E_DESTINATION.WAIT, CatPathManager.GetWaitPosition(i));
            }
        }
    }
}

