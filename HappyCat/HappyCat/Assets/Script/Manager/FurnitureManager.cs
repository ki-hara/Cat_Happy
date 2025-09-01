using HC.Data;
using HC.Resource;
using UnityEngine;

namespace HC.Game
{
    public class FurnitureManager
    {
        //bg
        public static SpriteRenderer Floor { get; set; }
        public static SpriteRenderer Sky { get; set; }
        public static SpriteRenderer Wall { get; set; }
        public static SpriteRenderer Wall_Partition { get; set; }

        //restaurant
        public static SpriteRenderer Board { get; set; }
        public static SpriteRenderer Table1 { get; set; }
        public static SpriteRenderer Table2 { get; set; }
        public static SpriteRenderer Table3 { get; set; }
        public static SpriteRenderer Table4 { get; set; }
        public static SpriteRenderer Table5 { get; set; }
        public static SpriteRenderer Table6 { get; set; }
        public static SpriteRenderer Selfcorner { get; set; }
        public static SpriteRenderer Gate { get; set; }
        public static SpriteRenderer Drinking { get; set; }
        public static SpriteRenderer Counter { get; set; }

        //kitchen
        public static SpriteRenderer Carpet { get; set; }
        public static SpriteRenderer Window { get; set; }
        public static SpriteRenderer Sink { get; set; }
        public static SpriteRenderer Shelf { get; set; }
        public static SpriteRenderer Refrigerator { get; set; }
        public static SpriteRenderer Plants { get; set; }
        public static SpriteRenderer Oven_01 { get; set; }
        public static SpriteRenderer Oven_02 { get; set; }
        public static SpriteRenderer Oven_03 { get; set; }
        public static SpriteRenderer Oven_04 { get; set; }
        public static SpriteRenderer Oven_05 { get; set; }
        public static SpriteRenderer Oven_06 { get; set; }

        //terrace
        public static SpriteRenderer Cushion_01 { get; set; }
        public static SpriteRenderer Cushion_02 { get; set; }
        public static SpriteRenderer Cushion_03 { get; set; }
        public static SpriteRenderer Cushion_04 { get; set; }
        public static SpriteRenderer Cushion_05 { get; set; }

        public static void FurnitureInit()
        {
            FurnitureObjectSetting();
            FurnitureDataSetting(DataManager.ServerData.furnitureData);
        }

        private static void FurnitureObjectSetting()
        {
            Transform BG = GameObject.Find("BG").GetComponent<Transform>();

            Floor = BG.Find("floor").GetComponent<SpriteRenderer>();
            Sky = BG.Find("sky").GetComponent<SpriteRenderer>();
            Wall = BG.Find("wall").GetComponent<SpriteRenderer>();
            Wall_Partition = BG.Find("wall_b").GetComponent<SpriteRenderer>();

            Transform restaurnat = BG.Find("BG_Restaurant");

            Board = restaurnat.Find("board").GetComponent<SpriteRenderer>();
            Table1 = restaurnat.Find("table_01").GetComponent<SpriteRenderer>();
            Table2 = restaurnat.Find("table_02").GetComponent<SpriteRenderer>();
            Table3 = restaurnat.Find("table_03").GetComponent<SpriteRenderer>();
            Table4 = restaurnat.Find("table_04").GetComponent<SpriteRenderer>();
            Table5 = restaurnat.Find("table_05").GetComponent<SpriteRenderer>();
            Table6 = restaurnat.Find("table_06").GetComponent<SpriteRenderer>();
            Selfcorner = restaurnat.Find("selfcorner").GetComponent<SpriteRenderer>();
            Gate = restaurnat.Find("gate").GetComponent<SpriteRenderer>();
            Drinking = restaurnat.Find("drinking").GetComponent<SpriteRenderer>();
            Counter = restaurnat.Find("counter").GetComponent<SpriteRenderer>();

            Transform kitchen = BG.Find("BG_Kitchen");

            Carpet = kitchen.Find("carpet").GetComponent<SpriteRenderer>();
            Window = kitchen.Find("window").GetComponent<SpriteRenderer>();
            Sink = kitchen.Find("sink").GetComponent<SpriteRenderer>();
            Shelf = kitchen.Find("shelf").GetComponent<SpriteRenderer>();
            Refrigerator = kitchen.Find("refrigerator").GetComponent<SpriteRenderer>();
            Plants = kitchen.Find("plants").GetComponent<SpriteRenderer>();
            Oven_01 = kitchen.Find("oven_01").GetComponent<SpriteRenderer>();
            Oven_02 = kitchen.Find("oven_02").GetComponent<SpriteRenderer>();
            Oven_03 = kitchen.Find("oven_03").GetComponent<SpriteRenderer>();
            Oven_04 = kitchen.Find("oven_04").GetComponent<SpriteRenderer>();
            Oven_05 = kitchen.Find("oven_05").GetComponent<SpriteRenderer>();
            Oven_06 = kitchen.Find("oven_06").GetComponent<SpriteRenderer>();

            Transform terrace = BG.Find("BG_Terrace");

            Cushion_01 = terrace.Find("cushion_01").GetComponent<SpriteRenderer>();
            Cushion_02 = terrace.Find("cushion_02").GetComponent<SpriteRenderer>();
            Cushion_03 = terrace.Find("cushion_03").GetComponent<SpriteRenderer>();
            Cushion_04 = terrace.Find("cushion_04").GetComponent<SpriteRenderer>();
            Cushion_05 = terrace.Find("cushion_05").GetComponent<SpriteRenderer>();

        }

        private static async void FurnitureDataSetting(FurnitureData info)
        {
            Floor.sprite = await LoadAddressableManager.LoadImage_Furniture(info.floor);
            Sky.sprite = await LoadAddressableManager.LoadImage_Furniture(info.sky);
            Wall.sprite = await LoadAddressableManager.LoadImage_Furniture(info.wall);
            Wall_Partition.sprite = await LoadAddressableManager.LoadImage_Furniture(info.wall_b);
            Board.sprite = await LoadAddressableManager.LoadImage_Furniture(info.board);
            Table1.sprite = await LoadAddressableManager.LoadImage_Furniture(info.table1);
            Table2.sprite = await LoadAddressableManager.LoadImage_Furniture(info.table2);
            Table3.sprite = await LoadAddressableManager.LoadImage_Furniture(info.table3);
            Table4.sprite = await LoadAddressableManager.LoadImage_Furniture(info.table4);
            Table5.sprite = await LoadAddressableManager.LoadImage_Furniture(info.table5);
            Table6.sprite = await LoadAddressableManager.LoadImage_Furniture(info.table6);
            Selfcorner.sprite = await LoadAddressableManager.LoadImage_Furniture(info.selfcorner);
            Gate.sprite = await LoadAddressableManager.LoadImage_Furniture(info.gate);
            Drinking.sprite = await LoadAddressableManager.LoadImage_Furniture(info.drinking);
            Counter.sprite = await LoadAddressableManager.LoadImage_Furniture(info.counter);
            Carpet.sprite = await LoadAddressableManager.LoadImage_Furniture(info.carpet);
            Window.sprite = await LoadAddressableManager.LoadImage_Furniture(info.window);
            Sink.sprite = await LoadAddressableManager.LoadImage_Furniture(info.sink);
            Shelf.sprite = await LoadAddressableManager.LoadImage_Furniture(info.shelf);
            Refrigerator.sprite = await LoadAddressableManager.LoadImage_Furniture(info.refrigerator);
            Plants.sprite = await LoadAddressableManager.LoadImage_Furniture(info.plants);
            Oven_01.sprite = await LoadAddressableManager.LoadImage_Furniture(info.oven_01);
            Oven_02.sprite = await LoadAddressableManager.LoadImage_Furniture(info.oven_02);
            Oven_03.sprite = await LoadAddressableManager.LoadImage_Furniture(info.oven_03);
            Oven_04.sprite = await LoadAddressableManager.LoadImage_Furniture(info.oven_04);
            Oven_05.sprite = await LoadAddressableManager.LoadImage_Furniture(info.oven_05);
            Oven_06.sprite = await LoadAddressableManager.LoadImage_Furniture(info.oven_06);
            Cushion_01.sprite = await LoadAddressableManager.LoadImage_Furniture(info.cushion_01);
            Cushion_02.sprite = await LoadAddressableManager.LoadImage_Furniture(info.cushion_02);
            Cushion_03.sprite = await LoadAddressableManager.LoadImage_Furniture(info.cushion_03);
            Cushion_04.sprite = await LoadAddressableManager.LoadImage_Furniture(info.cushion_04);
            Cushion_05.sprite = await LoadAddressableManager.LoadImage_Furniture(info.cushion_05);
        }

        public static Vector3 GetOvenPosition(int index)
        {
            switch (index)
            {
                case 0: return Oven_01.transform.position;
                case 1: return Oven_02.transform.position;
                case 2: return Oven_03.transform.position;
                case 3: return Oven_04.transform.position;
                case 4: return Oven_05.transform.position;
                case 5: return Oven_06.transform.position;
                default: return Vector3.zero;

            }
        }
        public static Vector3 GetTablePosition(int index)
        {
            switch (index)
            {
                case 0: return Table1.transform.position;
                case 1: return Table2.transform.position;
                case 2: return Table3.transform.position;
                case 3: return Table4.transform.position;
                case 4: return Table5.transform.position;
                case 5: return Table6.transform.position;
                default: return Vector3.zero;

            }
        }
    }
}

