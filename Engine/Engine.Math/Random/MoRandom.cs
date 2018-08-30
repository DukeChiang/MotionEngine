//**************************************************
// Copyright©2018 何冠峰
// Licensed under the MIT license
//**************************************************
using System;
using System.Numerics;

namespace MotionEngine.Math
{
	public static class MoRandom
	{
		//全局随机种子
		private static readonly MoRand gRand = new MoRand();

		/// <summary>
		/// return a random integer number between min [inclusive] and max [exclusive].
		/// </summary>
		public static int Range(int min, int max)
		{
			int dif;
			if (min < max)
			{
				dif = max - min;
				int t = (int)(gRand.Get() % dif);
				t += min;
				return t;
			}
			else if (min > max)
			{
				dif = min - max;
				int t = (int)(gRand.Get() % dif);
				t = min - t;
				return t;
			}
			else
			{
				return min;
			}
		}
		public static int Range(MoRand rand, int min, int max)
		{
			int dif;
			if (min < max)
			{
				dif = max - min;
				int t = (int)(rand.Get() % dif);
				t += min;
				return t;
			}
			else if (min > max)
			{
				dif = min - max;
				int t = (int)(rand.Get() % dif);
				t = min - t;
				return t;
			}
			else
			{
				return min;
			}
		}

		/// <summary>
		/// return a random float number between min and max.
		/// </summary>
		public static float Range(float min, float max)
		{
			float t = gRand.GetFloat();
			t = min * t + (1.0F - t) * max;
			return t;
		}
		public static float Range(MoRand rand, float min, float max)
		{
			float t = rand.GetFloat();
			t = min * t + (1.0F - t) * max;
			return t;
		}

		public static bool Chance100(MoRand rand, int value)
		{
			if (value <= 0)
				return false;
			if (value >= 100)
				return true;
			return Range(rand, 0, 100) <= value;
		}
		public static bool Chance10000(MoRand rand, int value)
		{
			if (value <= 0)
				return false;
			if(value >= 10000)
				return true;
			return Range(rand, 0, 10000) <= value;
		}

		public static bool Chance100(int value)
		{
			if (value <= 0)
				return false;
			if (value >= 100)
				return true;
			return Range(0, 100) <= value;
		}
		public static bool Chance10000(int value)
		{
			if (value <= 0)
				return false;
			if (value >= 10000)
				return true;
			return Range(0, 10000) <= value;
		}

		/// <summary>
		/// 获取平面上圆内随机点
		/// </summary>
		public static Vector3 RandomPointInCircle(float radius)
		{
			Vector2 temp = RandomUnitVector2();
			Vector3 result = new Vector3(temp.X, 0, temp.Y);
			return result * radius;
		}

		/// <summary>
		/// 获取平面上半圆内随机点
		/// </summary>
		public static Vector3 RandomPointInSemiCircle(float radius)
		{
			float a = Range(-MathF.PI * 0.5f, MathF.PI * 0.5f);
			float x = MathF.Cos(a);
			float y = MathF.Sin(a);
			Vector3 result = new Vector3(x, 0, y);
			return result * radius;
		}

		/// <summary>
		/// 获取平面上圆环内随机点
		/// </summary>
		public static Vector3 RandomPointInBetweenCircle(float minRadius, float maxRadius)
		{
			Vector2 v = RandomUnitVector2();
			float range = MathF.Pow(Random01(), 1.0F / 2.0F);
			v = v * (minRadius + (maxRadius - minRadius) * range);
			return new Vector3(v.X, 0, v.Y);
		}

		#region 基础方法
		private static float Random01()
		{
			return gRand.GetFloat();
		}
		public static Vector2 RandomPointInsideUnitCircle()
		{
			Vector2 v = RandomUnitVector2();
			v *= MathF.Pow(Random01(), 1.0F / 2.0F);
			return v;
		}
		public static Vector3 RandomPointInsideUnitSphere()
		{
			Vector3 v = RandomUnitVector3();
			v *= MathF.Pow(Random01(), 1.0F / 3.0F);
			return v;
		}
		public static Vector3 RandomUnitVector3()
		{
			float z = Range(-1.0f, 1.0f);
			float a = Range(0.0f, 2.0F * MathF.PI);

			float r = MathF.Sqrt(1.0f - z * z);
			float x = r * MathF.Cos(a);
			float y = r * MathF.Sin(a);

			return new Vector3(x, y, z);
		}
		public static Vector2 RandomUnitVector2()
		{
			float a = Range(0.0f, 2.0F * MathF.PI);
			float x = MathF.Cos(a);
			float y = MathF.Sin(a);
			return new Vector2(x, y);
		}
		#endregion
	}
}