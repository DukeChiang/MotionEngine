//**************************************************
// Copyright©2018 何冠峰
// Licensed under the MIT license
//**************************************************
using System;

namespace MotionEngine
{
    public sealed class MoGaussRandom
    {
        private static UInt32 _seed = 61829450;

        /// <summary>
        /// 获取一个高斯正态分布随机数
        /// </summary>
        public static double Random()
        {
            double sum = 0;
            for (int i = 0; i < 3; i++)
            {
                UInt32 hold = _seed;
                _seed ^= _seed << 13;
                _seed ^= _seed >> 17;
                _seed ^= _seed << 5;
                Int32 r = (Int32)(hold + _seed);
                sum += r * (1.0 / 0x7FFFFFFF);
            }
            return sum; // [-3.0, 3.0]
        }

		/*
		public static void TEST()
		{
			int f3 = 0;
			int f3_f2 = 0;
			int f2_f1 = 0;
			int f1_f0 = 0;

			int z0_z1 = 0;
			int z1_z2 = 0;
			int z2_z3 = 0;
			int z3 = 0;

			for (int i = 0; i < 1000; i++)
			{
				double value = MoGaussRandom.Random();
				if (value < -3.0) f3++;
				if (value > -3.0 && value < -2.0) f3_f2++;
				if (value > -2.0 && value < -1.0) f2_f1++;
				if (value > -1.0 && value < 0) f1_f0++;
				if (value > 0 && value < 1) z0_z1++;
				if (value > 1 && value < 2) z1_z2++;
				if (value > 2 && value < 3) z2_z3++;
				if (value > 3) z3++;
			}

			Console.WriteLine("【小-3】" + f3);
			Console.WriteLine("【大-3 小-2】" + f3_f2);
			Console.WriteLine("【大-2 小-1】" + f2_f1);
			Console.WriteLine("【大-1 小0】" + f1_f0);

			Console.WriteLine("【大0 小1】" + z0_z1);
			Console.WriteLine("【大1 小2】" + z1_z2);
			Console.WriteLine("【大2 小3】" + z2_z3);
			Console.WriteLine("【大3】" + z3);
		}
		*/
	}
}