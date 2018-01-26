using System;
using System.Collections.Generic;

namespace Sunnet.Framework.Core.Tool
{
    public class RandomTool
    {
        static Random random = null;


        public static void resetRandom()
        {
            random = null;
        }
        public static void GetRandomNum(int[] arrRandom)
        {
            int len = arrRandom.Length;
            InitRandom();
            for (var i = 0; i < len - 1; i++)
            {
                var index = (int)Math.Floor(Convert.ToDouble(random.NextDouble() * (len - i)));
                var temp = arrRandom[index];
                arrRandom[index] = arrRandom[len - i - 1];
                arrRandom[len - i - 1] = temp;
            }
        }
        private static void InitRandom()
        {
            if (random == null)
            {
                long tick = DateTime.Now.Ticks;
                random = new Random((int)(tick & 0xffffffffL) | (int)(tick >> 32));
            }
        }
        public static void GetRandomNum(List<int> randomList)
        {
            int len = randomList.Count;

            InitRandom();

            for (var i = 0; i < len - 1; i++)
            {
                var index = (int)Math.Floor(Convert.ToDouble(random.NextDouble() * (len - i)));
                var temp = randomList[index];
                randomList[index] = randomList[len - i - 1];
                randomList[len - i - 1] = temp;
            }
        }

        public static void GetRandomNum(List<string> randomList)
        {
            int len = randomList.Count;
            InitRandom();
            for (var i = 0; i < len - 1; i++)
            {
                var index = (int)Math.Floor(Convert.ToDouble(random.NextDouble() * (len - i)));
                var temp = randomList[index];
                randomList[index] = randomList[len - i - 1];
                randomList[len - i - 1] = temp;
            }
        }
    }
}
