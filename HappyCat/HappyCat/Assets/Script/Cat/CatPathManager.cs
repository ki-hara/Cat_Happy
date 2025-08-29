using GoogleSheet;
using HC.Game;
using System;
using System.Collections.Generic;
using System.Reflection;
using UGS;
using UnityEngine;
using UnityEngine.Diagnostics;

namespace HC.Utils
{
    public class CatPathManager
    {
        static readonly Vector3 tableOffset = new Vector3(0, 60, 0);

        static readonly Vector3 startPath1 = new Vector3(160, 870, 0);
        static readonly Vector3 startPath2 = new Vector3(250, 870, 0);
        static readonly Vector3 startPath3 = new Vector3(340, 870, 0);
        static readonly Vector3 waitPath = new Vector3(250, 260, 0);
        //static readonly Vector3 table1Path = new Vector3(-315, 3, 0);
        //static readonly Vector3 table2Path = new Vector3(0, 3, 0);
        //static readonly Vector3 table3Path = new Vector3(314, 3, 0);
        //static readonly Vector3 table4Path = new Vector3(-312.2f, -242.6f, 0);
        //static readonly Vector3 table5Path = new Vector3(0, -242.6f, 0);
        //static readonly Vector3 table6Path = new Vector3(314, -242.6f, 0);
        //static readonly Vector3 oven1Path = new Vector3(-330, 20.5f, 0);
        //static readonly Vector3 oven2Path = new Vector3(-4, 20.5f, 0);
        //static readonly Vector3 oven3Path = new Vector3(320, 20.5f, 0);
        //static readonly Vector3 oven4Path = new Vector3(-330, -290, 0);
        //static readonly Vector3 oven5Path = new Vector3(-4, -290, 0);
        //static readonly Vector3 oven6Path = new Vector3(320, -290, 0);

        //restaurant
        static readonly Vector3 randomPoint1_1 = new Vector3(-156, 110, 0);
        static readonly Vector3 randomPoint1_2 = new Vector3(-457, -176, 0);
        static readonly Vector3 randomPoint1_3 = new Vector3(-157, -176, 0);
        static readonly Vector3 randomPoint1_4 = new Vector3(154, -176, 0);
        static readonly Vector3 randomPoint1_5 = new Vector3(458, -176, 0);
        static readonly Vector3 randomPoint1_6 = new Vector3(-351, -509, 0);
        static readonly Vector3 randomPoint1_7 = new Vector3(-65, -528, 0);
        static readonly Vector3 randomPoint1_8 = new Vector3(-271, -847, 0);

        //kichen
        static readonly Vector3 randomPoint2_1 = new Vector3(913, 161, 0);
        static readonly Vector3 randomPoint2_2 = new Vector3(1233, 161, 0);
        static readonly Vector3 randomPoint2_3 = new Vector3(550, -198, 0);
        static readonly Vector3 randomPoint2_4 = new Vector3(957, -569, 0);
        static readonly Vector3 randomPoint2_5 = new Vector3(1133, -769, 0);
        static readonly Vector3 randomPoint2_6 = new Vector3(1503, -592, 0);

        //terrace
        static readonly Vector3 randomPoint3_1 = new Vector3(-1098, -763, 0);
        static readonly Vector3 randomPoint3_2 = new Vector3(-746, -436, 0);
        static readonly Vector3 randomPoint3_3 = new Vector3(-1103, -214, 0);

        //furniture
        static readonly Vector3 cushion1 = new Vector3(-833, -116, 0);
        static readonly Vector3 cushion2 = new Vector3(-1341, -222, 0);
        static readonly Vector3 cushion3 = new Vector3(-1039, -417, 0);
        static readonly Vector3 cushion4 = new Vector3(-1387, -593, 0);
        static readonly Vector3 cushion5 = new Vector3(-801, -663, 0);
        static readonly Vector3 selfconner = new Vector3(-210, -532, 0);
        static readonly Vector3 drinking = new Vector3(-261, -599, 0);
        static readonly Vector3 counter = new Vector3(-263, -234, 0);
        static readonly Vector3 sink = new Vector3(890, 279, 0);
        static readonly Vector3 refrigerator = new Vector3(1399, 292, 0);
        static readonly Vector3 plants = new Vector3(1331, -585, 0);


        public static Vector3 GetStartPosition(int index = -1)
        {
            List<Vector3> list = new List<Vector3>() { 
                startPath1,
                startPath2,
                startPath3,
            };

            if (index > 0) return list[index];
            return DataUtill.GetRandom(list);
        }

        public static Vector3 GetWaitPosition(int index)
        {
            var pos = waitPath;
            pos.Set(pos.x, pos.y + (index * 50), pos.z);
            return pos;
        }

        public static Vector3 GetTablePosition(int index)
        {
            var a = FurnitureManager.Table1.transform;
            switch (index) { 
                case 0: return FurnitureManager.Table1.transform.position + tableOffset;
                case 1: return FurnitureManager.Table2.transform.position + tableOffset;
                case 2: return FurnitureManager.Table3.transform.position + tableOffset;
                case 3: return FurnitureManager.Table4.transform.position + tableOffset;
                case 4: return FurnitureManager.Table5.transform.position + tableOffset;
                case 5: return FurnitureManager.Table6.transform.position + tableOffset;
                default: return Vector3.zero;
            }
        }
        public static Vector3 GetTipPosition()
        {
            return counter;
        }

        public static Vector3 RandomPosition()
        {
            var list = new List<string>();
            list.Add("R");
            list.Add("K");
            list.Add("T");
            switch (DataUtill.GetRandom(list))
            {
                case "R":
                    {
                        var l = new List<Vector3>();
                        l.Add(randomPoint1_1);
                        l.Add(randomPoint1_2);
                        l.Add(randomPoint1_3);
                        l.Add(randomPoint1_4);
                        l.Add(randomPoint1_5);
                        l.Add(randomPoint1_6);
                        l.Add(randomPoint1_7);
                        l.Add(randomPoint1_8);
                        var p = DataUtill.GetRandom(l);
                        var r = new Vector3(UnityEngine.Random.Range(-20f, 20f), UnityEngine.Random.Range(-20f, 20f), 0);
                        return p + r;
                    }
                case "K":
                    {
                        var l = new List<Vector3>();
                        l.Add(randomPoint2_1);
                        l.Add(randomPoint2_2);
                        l.Add(randomPoint2_3);
                        l.Add(randomPoint2_4);
                        l.Add(randomPoint2_5);
                        l.Add(randomPoint2_6);
                        var p = DataUtill.GetRandom(l);
                        var r = new Vector3(UnityEngine.Random.Range(-20f, 20f), UnityEngine.Random.Range(-20f, 20f), 0);
                        return p + r;
                    }
                case "T":
                    {
                        var l = new List<Vector3>();
                        l.Add(randomPoint3_1);
                        l.Add(randomPoint3_2);
                        l.Add(randomPoint3_3);
                        var p = DataUtill.GetRandom(l);
                        var r = new Vector3(UnityEngine.Random.Range(-20f, 20f), UnityEngine.Random.Range(-20f, 20f), 0);
                        return p + r;
                    }
            }
            //아무거나 리턴
            return randomPoint1_5;
        }
        public static FurnitureInfo FurniturePosition()
        {
            var list = new List<string>();
            list.Add("R");
            list.Add("K");
            list.Add("T");
            switch (DataUtill.GetRandom(list))
            {
                case "R":
                    {
                        var l = new List<FurnitureInfo>();
                        l.Add(new FurnitureInfo(selfconner, 4));
                        l.Add(new FurnitureInfo(drinking, 4));
                        return DataUtill.GetRandom(l);
                    }
                case "K":
                    {
                        var l = new List<FurnitureInfo>();
                        l.Add(new FurnitureInfo(sink, 1));
                        l.Add(new FurnitureInfo(refrigerator, 1));
                        l.Add(new FurnitureInfo(plants, 4));
                        return DataUtill.GetRandom(l);
                    }
                case "T":
                    {
                        var l = new List<FurnitureInfo>();
                        l.Add(new FurnitureInfo(cushion1, 5));
                        l.Add(new FurnitureInfo(cushion2, 5));
                        l.Add(new FurnitureInfo(cushion3, 5));
                        l.Add(new FurnitureInfo(cushion4, 5));
                        l.Add(new FurnitureInfo(cushion5, 5));
                        return DataUtill.GetRandom(l);
                        
                    }
            }
            return null;
        }
        //private static Vector3 GetExitPosition()
        //{

        //}
        //private static Vector3 GetTipPosition()
        //{

        //}
    }
    public class FurnitureInfo{
        public Vector3 position;
        public int furnitureType;

        public FurnitureInfo(Vector3 position, int furnitureType)
        {
            this.position = position;
            this.furnitureType = furnitureType;
        }
    }
}


