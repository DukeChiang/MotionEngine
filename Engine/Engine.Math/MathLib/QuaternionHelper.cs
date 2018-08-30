//**************************************************
// Copyright©2018 何冠峰
// Licensed under the MIT license
//**************************************************
using System;
using System.Numerics;

namespace MotionEngine.Math
{
	public static class QuaternionHelper
	{
		public static Quaternion EulerToQuaternion(Vector3 eulerAngle)
		{
			//角度转弧度
			eulerAngle = MoMath.Deg2Rad(eulerAngle);

			float cX = MathF.Cos(eulerAngle.X / 2.0f);
			float sX = MathF.Sin(eulerAngle.X / 2.0f);

			float cY = MathF.Cos(eulerAngle.Y / 2.0f);
			float sY = MathF.Sin(eulerAngle.Y / 2.0f);

			float cZ = MathF.Cos(eulerAngle.Z / 2.0f);
			float sZ = MathF.Sin(eulerAngle.Z / 2.0f);

			Quaternion qX = new Quaternion(sX, 0, 0, cX);
			Quaternion qY = new Quaternion(0, sY, 0, cY);
			Quaternion qZ = new Quaternion(0, 0, sZ, cZ);

			Quaternion q = (qY * qX) * qZ;

#if DEBUG
			if (!MoMath.CompareApproximate(q.LengthSquared(), 1f))
			{
				string msg = string.Format($"EulerToQuaternion failed, {q.X} {q.Y} {q.Z} {q.W} sqrLength={q.LengthSquared()}");
				MoLog.Log(ELogType.Assert, msg);
			}
#endif

			return q;
		}
		public static Vector3 QuaternionToEuler(Quaternion quat)
		{
			Matrix3x3 m = QuaternionToMatrix(quat);
			Vector3 euler = MatrixToEuler(m);

			//弧度转角度
			return MoMath.Rad2Deg(euler);
		}
		public static bool LookRotationToQuaternion(Vector3 viewVec, Vector3 upVec, out Quaternion quat)
		{
			quat = Quaternion.Identity;

			// Generates a Right handed Quat from a look rotation. Returns if conversion was successful.
			if (!Matrix3x3.LookRotationToMatrix(viewVec, upVec, out Matrix3x3 m))
				return false;
			quat = MatrixToQuaternion(m);
			return true;
		}
		public static Quaternion NormalizeSafe(Quaternion q)
		{
			float mag = q.Length();
			if (mag < Vector3Helper.Epsilon)
				return Quaternion.Identity;
			else
				return Quaternion.Normalize(q);
		}
		public static Vector3 RotateVectorByQuat(Quaternion lhs, Vector3 rhs)
		{
			float x = lhs.X * 2.0F;
			float y = lhs.Y * 2.0F;
			float z = lhs.Z * 2.0F;
			float xx = lhs.X * x;
			float yy = lhs.Y * y;
			float zz = lhs.Z * z;
			float xy = lhs.X * y;
			float xz = lhs.X * z;
			float yz = lhs.Y * z;
			float wx = lhs.W * x;
			float wy = lhs.W * y;
			float wz = lhs.W * z;

			Vector3 res = new Vector3();
			res.X = (1.0f - (yy + zz)) * rhs.X + (xy - wz) * rhs.Y + (xz + wy) * rhs.Z;
			res.Y = (xy + wz) * rhs.X + (1.0f - (xx + zz)) * rhs.Y + (yz - wx) * rhs.Z;
			res.Z = (xz - wy) * rhs.X + (yz + wx) * rhs.Y + (1.0f - (xx + yy)) * rhs.Z;

			return res;
		}
		public static Quaternion AxisAngleToQuaternionSafe(Vector3 axis, float angle)
		{
			Quaternion q;
			float mag = axis.Length();
			if (mag > 0.000001F)
			{
				float halfAngle = angle * 0.5F;

				q.W = MathF.Cos(halfAngle);

				float s = MathF.Sin(halfAngle) / mag;
				q.X = s * axis.X;
				q.Y = s * axis.Y;
				q.Z = s * axis.Z;
				return q;
			}
			else
			{
				return Quaternion.Identity;
			}
		}

		private static Matrix3x3 QuaternionToMatrix(Quaternion q)
		{

#if DEBUG
			// If q is guaranteed to be a unit quaternion, s will always be 1.  
			// In that case, this calculation can be optimized out.
			if (!MoMath.CompareApproximate(q.LengthSquared(), 1.0F))
			{
				string msg = string.Format($"QuaternionToMatrix conversion failed, because input Quaternion is invalid {q.X} {q.Y} {q.Z} {q.W} sqrLength={q.LengthSquared()}");
				MoLog.Log(ELogType.Assert, msg);
			}
#endif

			// Precalculate coordinate products
			float x = q.X * 2.0F;
			float y = q.Y * 2.0F;
			float z = q.Z * 2.0F;
			float xx = q.X * x;
			float yy = q.Y * y;
			float zz = q.Z * z;
			float xy = q.X * y;
			float xz = q.X * z;
			float yz = q.Y * z;
			float wx = q.W * x;
			float wy = q.W * y;
			float wz = q.W * z;

			// Calculate 3x3 matrix from orthonormal basis
			Matrix3x3 m = Matrix3x3.Identity;

			m.Data[0] = 1.0f - (yy + zz);
			m.Data[1] = xy + wz;
			m.Data[2] = xz - wy;

			m.Data[3] = xy - wz;
			m.Data[4] = 1.0f - (xx + zz);
			m.Data[5] = yz + wx;

			m.Data[6] = xz + wy;
			m.Data[7] = yz - wx;
			m.Data[8] = 1.0f - (xx + yy);

			return m;
		}
		private static Quaternion MatrixToQuaternion(Matrix3x3 kRot)
		{
			Quaternion q = new Quaternion();

			// Algorithm in Ken Shoemake's article in 1987 SIGGRAPH course notes
			// article "Quaternionf Calculus and Fast Animation".

#if DEBUG
			float det = kRot.GetDeterminant();
			if (!MoMath.CompareApproximate(det, 1.0F, .005f))
			{
				MoLog.Log(ELogType.Assert, "MatrixToQuaternion det assert.");
			}
#endif

			float fTrace = kRot.Get(0, 0) + kRot.Get(1, 1) + kRot.Get(2, 2);
			float fRoot;

			if (fTrace > 0.0f)
			{
				// |w| > 1/2, may as well choose w > 1/2
				fRoot = MathF.Sqrt(fTrace + 1.0f);  // 2w
				q.W = 0.5f * fRoot;
				fRoot = 0.5f / fRoot;  // 1/(4w)
				q.X = (kRot.Get(2, 1) - kRot.Get(1, 2)) * fRoot;
				q.Y = (kRot.Get(0, 2) - kRot.Get(2, 0)) * fRoot;
				q.Z = (kRot.Get(1, 0) - kRot.Get(0, 1)) * fRoot;
			}
			else
			{
				// |w| <= 1/2
				int[] s_iNext = new int[3] { 1, 2, 0 };
				int i = 0;
				if (kRot.Get(1, 1) > kRot.Get(0, 0))
					i = 1;
				if (kRot.Get(2, 2) > kRot.Get(i, i))
					i = 2;
				int j = s_iNext[i];
				int k = s_iNext[j];

				fRoot = MathF.Sqrt(kRot.Get(i, i) - kRot.Get(j, j) - kRot.Get(k, k) + 1.0f);
				float[] apkQuat = new float[3] { q.X, q.Y, q.Z };

#if DEBUG
				if (fRoot < Vector3Helper.Epsilon)
				{
					MoLog.Log(ELogType.Assert, "MatrixToQuaternion fRoot assert.");
				}
#endif

				apkQuat[i] = 0.5f * fRoot;
				fRoot = 0.5f / fRoot;
				q.W = (kRot.Get(k, j) - kRot.Get(j, k)) * fRoot;
				apkQuat[j] = (kRot.Get(j, i) + kRot.Get(i, j)) * fRoot;
				apkQuat[k] = (kRot.Get(k, i) + kRot.Get(i, k)) * fRoot;

				q.X = apkQuat[0];
				q.Y = apkQuat[1];
				q.Z = apkQuat[2];
			}
			q = Quaternion.Normalize(q);

			return q;
		}
		private static Vector3 MatrixToEuler(Matrix3x3 matrix)
		{
			// from http://www.geometrictools.com/Documentation/EulerAngles.pdf
			// YXZ order
			Vector3 v = Vector3.Zero;
			if (matrix.Data[7] < 0.999F) // some fudge for imprecision
			{
				if (matrix.Data[7] > -0.999F) // some fudge for imprecision
				{
					v.X = MathF.Asin(-matrix.Data[7]);
					v.Y = MathF.Atan2(matrix.Data[6], matrix.Data[8]);
					v.Z = MathF.Atan2(matrix.Data[1], matrix.Data[4]);
					MakePositive(v);
				}
				else
				{
					// WARNING.  Not unique.  YA - ZA = atan2(r01,r00)
					v.X = MathF.PI * 0.5F;
					v.Y = MathF.Atan2(matrix.Data[3], matrix.Data[0]);
					v.Z = 0.0F;
					MakePositive(v);
				}
			}
			else
			{
				// WARNING.  Not unique.  YA + ZA = atan2(-r01,r00)
				v.X = -MathF.PI * 0.5F;
				v.Y = MathF.Atan2(-matrix.Data[3], matrix.Data[0]);
				v.Z = 0.0F;
				MakePositive(v);
			}

			return v; //返回的是弧度值
		}
		private static Vector3 MakePositive(Vector3 euler)
		{
			const float negativeFlip = -0.0001F;
			const float positiveFlip = (MathF.PI * 2.0F) - 0.0001F;

			if (euler.X < negativeFlip)
				euler.X += 2.0f * MathF.PI;
			else if (euler.X > positiveFlip)
				euler.X -= 2.0f * MathF.PI;

			if (euler.Y < negativeFlip)
				euler.Y += 2.0f * MathF.PI;
			else if (euler.Y > positiveFlip)
				euler.Y -= 2.0f * MathF.PI;

			if (euler.Z < negativeFlip)
				euler.Z += 2.0f * MathF.PI;
			else if (euler.Z > positiveFlip)
				euler.Z -= 2.0f * MathF.PI;

			return euler;
		}

		/// <summary>
		/// check Quaternion is invalid. if value is NaN or +/- infinity
		/// </summary>
		public static void CheckInvalid(Quaternion q)
		{
			if (MoMath.IsInfinity(q))
				MoLog.Log(ELogType.Assert, "Quaternion isInfinity");

			if (MoMath.IsNaN(q))
				MoLog.Log(ELogType.Assert, "Quaternion isNaN");
		}
	}
}