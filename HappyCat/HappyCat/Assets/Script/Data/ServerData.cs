using System;
using UnityEngine;

namespace HC.Data
{
    public class ServerData
    {
        public UserData userData;
        public FurnitureData furnitureData;
    }


    public class UserData
    {
        public int level;
        public int cash;
        public int coin;
        public int guideQuest_Progression;
    }

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
    }
}
