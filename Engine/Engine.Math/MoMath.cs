//**************************************************
// Copyright©2018 何冠峰
// Licensed under the MIT license
//**************************************************
using System.Numerics;

namespace MotionEngine.Math
{
	public static class MoMath
	{
		public const float CosAngle20 = 0.9396926208f;
		public const float CompareEpsilon = 0.000001f;

		public static bool CompareApproximate(float f0, float f1, float epsilon = CompareEpsilon)
		{
			return System.MathF.Abs(f0 - f1) < epsilon;
		}
		public static bool CompareApproximate(double f0, double f1, float epsilon = CompareEpsilon)
		{
			return System.Math.Abs(f0 - f1) < epsilon;
		}
		public static bool IsNearlyZero(Vector3 v, float epsilon = CompareEpsilon)
		{
			return v.LengthSquared() < epsilon;
		}

		public static float Lerp(float from, float to, float t)
		{
			return (from * (1 - t)) + (to * t);
		}

		public static bool IsInfinity(Vector3 v)
		{
			return float.IsInfinity(v.X) || float.IsInfinity(v.Y) || float.IsInfinity(v.Z);
		}
		public static bool IsNaN(Vector3 v)
		{
			return float.IsNaN(v.X) || float.IsNaN(v.Y) || float.IsNaN(v.Z);
		}

		public static bool IsInfinity(Quaternion q)
		{
			return float.IsInfinity(q.X) || float.IsInfinity(q.Y) || float.IsInfinity(q.Z) || float.IsInfinity(q.W);
		}
		public static bool IsNaN(Quaternion q)
		{
			return float.IsNaN(q.X) || float.IsNaN(q.Y) || float.IsNaN(q.Z) || float.IsNaN(q.W);
		}

		public static float Rad2Deg(float radians)
		{
			return (float)(radians * 180 / System.Math.PI);
		}
		public static float Deg2Rad(float degrees)
		{
			return (float)(degrees * System.Math.PI / 180);
		}

		public static Vector3 Rad2Deg(Vector3 radians)
		{
			return new Vector3(
				(float)(radians.X * 180 / System.Math.PI),
				(float)(radians.Y * 180 / System.Math.PI),
				(float)(radians.Z * 180 / System.Math.PI));
		}
		public static Vector3 Deg2Rad(Vector3 degrees)
		{
			return new Vector3(
			   (float)(degrees.X * System.Math.PI / 180),
			   (float)(degrees.Y * System.Math.PI / 180),
			   (float)(degrees.Z * System.Math.PI / 180));
		}

		//NOTE : Net的数学库在max小于min的时候会报异常错误，所以推荐使用下面的方法
		public static double Clamp(double value, double min, double max)
		{
			if (value > max)
				return max;
			else if (value < min)
				return min;
			else
				return value;
		}
		public static float Clamp(float value, float min, float max)
		{
			if (value > max)
				return max;
			else if (value < min)
				return min;
			else
				return value;
		}
		public static int Clamp(int value, int min, int max)
		{
			if (value > max)
				return max;
			else if (value < min)
				return min;
			else
				return value;
		}
	}
}
