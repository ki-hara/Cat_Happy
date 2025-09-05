using HC.Utils;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace HC.Data
{
    public class DataManager
    {
        private static LocalSaveData saveData;
        private static ServerData serverData;
        public static LocalSaveData SaveData
        {
            get
            {
                if (saveData == null)
                {
                    SetSaveData();
                }
                return saveData;
            }
        }
        public static ServerData ServerData
        {
            get
            {
                if (serverData == null)
                {
                    SetServerData();
                }
                return serverData;
            }
        }

        public static void Init()
        {
            LocalDataLoad();
            TempServerDataLoad();
        }

        private static void SetSaveData()
        {
            saveData = new LocalSaveData(LocalDataSave);
            LocalDataSave();
        }
        private static void SetServerData()
        {
            serverData = new ServerData();
            TempServerDataSave();
        }

        private static void LocalDataSave()
        {
            if (saveData == null) return;

            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Create(Application.persistentDataPath + "/LocalData.dat");
                bf.Serialize(file, saveData);
                file.Close();
                HC_Debug.Log("Local data saved!");
            }
            catch (Exception e)
            {
                HC_Debug.Log(e.Message);
            }
        }
        private static void LocalDataLoad()
        {
            if(File.Exists(Application.persistentDataPath + "/LocalData.dat") == false)
            {
                saveData = null;
                SetSaveData();
                return;
            }

            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + "/LocalData.dat", FileMode.Open);
                saveData = (LocalSaveData)bf.Deserialize(file);
                file.Close();
                HC_Debug.Log("Local Data Loeaded!");
            }
            catch (Exception e)
            {
                SetSaveData();
                HC_Debug.Log(e.Message);
                return;
            }
        }

        //private static void TempServerDataLoad()
        //{
        //    UserData userData = new UserData();
        //    userData.level = 1;
        //    userData.cash = 0;
        //    userData.coin = 0;
        //    userData.guideQuest_Progression = 0;
        //    serverData.userData = userData;

        //    FurnitureData furnitureData = new FurnitureData();
        //    furnitureData.floor = "basica_floor_001";
        //    furnitureData.sky = "basica_sky_001";
        //    furnitureData.wall = "basica_walla_001";
        //    furnitureData.wall_b = "basica_wallb_001";
        //    furnitureData.board = "basica_board_001";
        //    furnitureData.table1 = "basica_table_001";
        //    furnitureData.table2 = "basicb_table_002";
        //    furnitureData.table3 = "clover_table_003";
        //    furnitureData.table4 = "";
        //    furnitureData.table5 = "";
        //    furnitureData.table6 = "";
        //    furnitureData.selfcorner = "basica_selfcorner_001";
        //    furnitureData.gate = "basica_gate_001";
        //    furnitureData.drinking = "basica_drinking_001";
        //    furnitureData.counter = "basica_counter_001";
        //    furnitureData.carpet = "basica_carpet_001";
        //    furnitureData.window = "basica_window_001";
        //    furnitureData.sink = "basica_sink_001";
        //    furnitureData.shelf = "basica_shelf_001";
        //    furnitureData.refrigerator = "basica_refrigerator_001";
        //    furnitureData.plants = "basica_plants_001";
        //    furnitureData.oven_01 = "basica_oven_001";
        //    furnitureData.oven_02 = "basicb_oven_002";
        //    furnitureData.oven_03 = "clover_oven_003";
        //    furnitureData.oven_04 = "";
        //    furnitureData.oven_05 = "";
        //    furnitureData.oven_06 = "";
        //    furnitureData.cushion_01 = "basica_cushion_001";
        //    furnitureData.cushion_02 = "basica_cushion_001";
        //    furnitureData.cushion_03 = "basicb_cushion_002";
        //    furnitureData.cushion_04 = "clover_cushion_003";
        //    furnitureData.cushion_05 = "clover_cushion_003";
        //    serverData.furnitureData = furnitureData;
        //}

        public static void TempServerDataSave()
        {
            if (serverData == null) return;

            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Create(Application.persistentDataPath + "/TempServerData.dat");
                bf.Serialize(file, serverData);
                file.Close();
                HC_Debug.Log("Server data saved!");
            }
            catch (Exception e)
            {
                HC_Debug.Log(e.Message);
            }
        }
        public static void TempServerDataLoad()
        {
            if (File.Exists(Application.persistentDataPath + "/LocalData.dat") == false)
            {
                serverData = null;
                TempServerDataSave();
                return;
            }

            try
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + "/TempServerData.dat", FileMode.Open);
                serverData = (ServerData)bf.Deserialize(file);
                file.Close();
                HC_Debug.Log("Server Data Loeaded!");
            }
            catch (Exception e)
            {
                SetSaveData();
                HC_Debug.Log(e.Message);
                return;
            }
        }
    }
}