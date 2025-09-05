using System;
using UnityEngine;

namespace HC.Data
{
    [Serializable]
    public class ServerData
    {
        public UserData userData;
        public FurnitureData furnitureData;

        public ServerData()
        {
            userData = new UserData();
            furnitureData = new FurnitureData();
        }
    }

    [Serializable]
    public class UserData
    {
        public int level;
        public int cash;
        public int coin;
        public int guideQuest_Progression;
    }
    [Serializable]
    public class FurnitureData
    {
        public string floor;
        public string sky;
        public string wall;
        public string wall_b;
        public string board;
        public string table1;
        public string table2;
        public string table3;
        public string table4;
        public string table5;
        public string table6;
        public string selfcorner;
        public string gate;
        public string drinking;
        public string counter;
        public string carpet;
        public string window;
        public string sink;
        public string shelf;
        public string refrigerator;
        public string plants;
        public string oven_01;
        public string oven_02;
        public string oven_03;
        public string oven_04;
        public string oven_05;
        public string oven_06;
        public string cushion_01;
        public string cushion_02;
        public string cushion_03;
        public string cushion_04;
        public string cushion_05;

        public FurnitureData() {
            //floor = "basica_floor_001";
            //sky = "basica_sky_001";
            //wall = "basica_walla_001";
            //wall_b = "basica_wallb_001";

            floor = "basica_floor_001";
            sky = "basica_sky_001";
            wall = "basica_walla_001";
            wall_b = "basica_wallb_001";
            board = "basica_board_001";
            table1 = "basica_table_001";
            table2 = "basicb_table_002";
            table3 = "clover_table_003";
            table4 = "";
            table5 = "";
            table6 = "";
            selfcorner = "basica_selfcorner_001";
            gate = "basica_gate_001";
            drinking = "basica_drinking_001";
            counter = "basica_counter_001";
            carpet = "basica_carpet_001";
            window = "basica_window_001";
            sink = "basica_sink_001";
            shelf = "basica_shelf_001";
            refrigerator = "basica_refrigerator_001";
            plants = "basica_plants_001";
            oven_01 = "basica_oven_001";
            oven_02 = "basicb_oven_002";
            oven_03 = "clover_oven_003";
            oven_04 = "";
            oven_05 = "";
            oven_06 = "";
            cushion_01 = "basica_cushion_001";
            cushion_02 = "basica_cushion_001";
            cushion_03 = "basicb_cushion_002";
            cushion_04 = "clover_cushion_003";
            cushion_05 = "clover_cushion_003";
        }
    }
}
