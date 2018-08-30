//**************************************************
// Copyright©2018 何冠峰
// Licensed under the MIT license
//**************************************************
using System;
using System.Numerics;

namespace MotionEngine.Math
{
    public static class Vector3Helper
    {
		internal const float Epsilon = 0.00001F;

		public static float Angle(Vector3 lhs, Vector3 rhs)
		{
			float lhsMag = lhs.Length();
			float rhsMag = rhs.Length();

			// both vectors are non-zero
			if (lhsMag > Epsilon && rhsMag > Epsilon)
			{
				Vector3 lhsNorm = lhs / lhsMag;
				Vector3 rhsNorm = rhs / rhsMag;
				float dot = Vector3.Dot(lhsNorm, rhsNorm);
				dot = MoMath.Clamp(dot, -1f, 1f); //如果MathF.Acos()传入的值大于1f，该方法会返回NAN
				float radians = MathF.Acos(dot);
				return MoMath.Rad2Deg(radians);
			}
			else
			{
				return 0;
			}
		}
		public static Vector3 NormalizeSafe(Vector3 v)
		{
			float mag = v.Length();
			if (mag < Epsilon)
				return Vector3.Zero;
			else
				return Vector3.Normalize(v);
		}

		/// <summary>
		/// check Vector3 is invalid. if value is NaN or +/- infinity
		/// </summary>
		public static void CheckInvalid(Vector3 v)
		{
			if (MoMath.IsInfinity(v))
				MoLog.Log(ELogType.Assert, "Vector3 isInfinity");

			if (MoMath.IsNaN(v))
				MoLog.Log(ELogType.Assert, "Vector3 isNaN");
		}
	}
}