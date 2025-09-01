using HC.Data;
using System;
using UnityEngine;

namespace HC.Network
{
    public class TempAPI
    {
        static public UserData GetUserData()
        {
            DataManager.TempServerDataLoad();
            return DataManager.ServerData.userData;
        }
        static public UserData SetUserData(UserData userData)
        {
            DataManager.ServerData.userData = userData;
            DataManager.TempServerDataSave();
            return userData;
        }

        static public FurnitureData GetFurnitureData()
        {
            DataManager.TempServerDataLoad();
            return DataManager.ServerData.furnitureData;
        }
        static public FurnitureData SetFurnitureData(FurnitureData furnitureData)
        {
            DataManager.ServerData.furnitureData = furnitureData;
            DataManager.TempServerDataSave();
            return DataManager.ServerData.furnitureData;
        }
    }
}
