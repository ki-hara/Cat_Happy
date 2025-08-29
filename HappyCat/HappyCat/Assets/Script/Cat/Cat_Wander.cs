using Cysharp.Threading.Tasks;
using HC.Data;
using HC.Event;
using HC.Resource;
using HC.Utils;
using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using UGS;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace HC.Game
{
    public class Cat_Wander
    {
        Cat_Actor actor;
        int wanderCount = 0;

        public Cat_Wander(Cat_Actor actor)
        {
            this.actor = actor;
        }
        public void WanderStart()
        {
            float ERate = wanderCount * 0.5f;
            float RWeight = 0.5f - (ERate * 0.5f);
            float IWeight = 0.5f - (ERate * 0.5f);
            float EWeight = ERate;
            List<WeightedItem<string>> list = new List<WeightedItem<string>>();
            list.Add(new WeightedItem<string>("RendomWander", RWeight));
            list.Add(new WeightedItem<string>("InteractionFurniture", IWeight));
            list.Add(new WeightedItem<string>("ExitWander", EWeight));

            switch (DataUtill.GetWeightedRandom(list))
            {
                case "RendomWander":
                    RendomWander();
                    break;
                case "InteractionFurniture":
                    InteractionFurniture();
                    break;
                case "ExitWander":
                    ExitWander();
                    break;
            }

            wanderCount++;
        }
        public void RendomWander()
        {
            actor.SetDestination(E_DESTINATION.MOVEPOINT, CatPathManager.RandomPosition());
        }
        public void InteractionFurniture()
        {
            var info = CatPathManager.FurniturePosition();
            E_DESTINATION type = (E_DESTINATION)info.furnitureType;
            actor.SetDestination(type, info.position);
        }
        public void ExitWander()
        {
            var list = new List<string>();
            list.Add("TIP");
            list.Add("EXIT");
            var r = DataUtill.GetRandom(list);
            if (r == "TIP") actor.SetDestination(E_DESTINATION.TIP, CatPathManager.GetTipPosition());
            else Exit();
        }
        public async void Wait(int ms)
        {
            await UniTask.Delay(ms);
            WanderStart();
        }
        public async void Tip()
        {
            //팁
            await UniTask.Delay(1000);
            Exit();
        }
        public void Exit()
        {
            actor.SetDestination(E_DESTINATION.EXIT, CatPathManager.GetStartPosition(1));
        }
    }
}
